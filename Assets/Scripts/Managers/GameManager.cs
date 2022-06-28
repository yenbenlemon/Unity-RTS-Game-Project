using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.AI.Navigation;

public class GameManager : EventManager
{
	// GameManager Instance
	public static GameManager Instance;

    // Events
    public static event ResourceHandler OnResourceUpdate;
    public static event ResourceHandler OnPayResources;
    public static event TrainUnitHandler OnTrainUnit;
    public static event UnitSelectHandler OnUnitSelected;
    public static event UnitDeselectHandler OnDeselectAll;
    public static event FocusSelectHandler OnFocusSelected;
    public static event GameStateChangeHandler OnGameStateChange;


	public NavMeshSurface goblinNavMesh;
	public NavMeshSurface tankNavMesh;

    // Reward Item Prefabs
    public GameObject[] rewardItems;
	public UnitSpawner unitSpawner;

    // Used to keep track of specific units
    public GameObject cameraFocus;
    public GameObject cameraFoW;

    // For selecting and keeping track of player units
    public List<GameObject> pUnitList = new List<GameObject>();
    public List<GameObject> pSelectedList = new List<GameObject>();

    // List of enemy units
    public List<GameObject> eUnitList = new List<GameObject>();

    [Header("Resource Info")]
    public int resourceAmount = 1000;
    public int resourceReward = 100;
    public int resourceTimeDelay = 5;

	int _costs = 0;

    float timer;
    bool _buildingMode = false;

	[Header("AI Info")]
	AIManager _aiManager;

    // Allows us to know what the currently selected single unit is
	enum SelectedType
	{
		NONE,
		STATIC,
		DYNAMIC
	};
	SelectedType _selectedType = SelectedType.NONE;

    // Allows us to set the game state which will fire an event to allow other processes to know the state as well
    public enum GameState
    {
        PLAY,
        PAUSE,
        WIN,
        LOSE
    };
    public GameState _gameState = GameState.PLAY;

    // Our currently selected unit
    GameObject selectedUnit;

	public bool BuildingMode { get => _buildingMode; set => _buildingMode = value; }

	// Awake is called when GameManager is Instantiated
	void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		_aiManager = GameObject.Find("AI Manager").GetComponent<AIManager>();
		cameraFoW.SetActive(false);
		cameraFoW.SetActive(true);
	}

    // Start is called before the first frame update
    void Start()
    {
        // Inform listeners of resources
		OnPayResources?.Invoke(resourceAmount);
        OnResourceUpdate?.Invoke(resourceAmount);
    }

    private void OnEnable()
    {
        // Subscribe to mouse clicks
        MouseInputManager.OnLeftClickInput += MouseInputManager_OnLeftClickInput;
        MouseInputManager.OnRightClickInput += MouseInputManager_OnRightClickInput;
        MouseInputManager.OnMiddleClickInput += MouseInputManager_OnMiddleClickInput;
    }

    private void OnDisable()
    {
        // Unsubscribe
        MouseInputManager.OnLeftClickInput -= MouseInputManager_OnLeftClickInput;
        MouseInputManager.OnRightClickInput -= MouseInputManager_OnRightClickInput;
        MouseInputManager.OnMiddleClickInput -= MouseInputManager_OnMiddleClickInput;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer >= resourceTimeDelay)
        {
            // Reset Timer
            timer = 0.0f;

            // Update resources
            UpdateResources(resourceReward);
			OnPayResources?.Invoke(resourceReward);
        }

		if (_costs < 0)
		{
            UpdateResources(_costs);
			_costs = 0;
        }
    }

    // Attempt raycasts with specific mouse buttons presses
    void MouseInputManager_OnLeftClickInput(Vector3 mouseVector) { RayCastAttempt(mouseVector, 0); }
    void MouseInputManager_OnRightClickInput(Vector3 mouseVector) { RayCastAttempt(mouseVector, 1); }
    void MouseInputManager_OnMiddleClickInput(Vector3 mouseVector) { RayCastAttempt(mouseVector, 2); }

    public void UpdateResources(int amount)
    {
        // Update the amount and invoke the change
        resourceAmount += amount;
        OnResourceUpdate?.Invoke(resourceAmount);
    }

    public void Health_OnUnitCreated(GameObject unit)
    {
        // Add to our player or enemy list
        if (unit.GetComponent<Team>().unitTeam == Team.TeamType.Player) { pUnitList.Add(unit); }
        else if (unit.GetComponent<Team>().unitTeam == Team.TeamType.Enemy) { eUnitList.Add(unit); }
    }

    public void Health_OnUnitDestroyed(GameObject unit)
    {
        // If a base was destroyed, the game is over
        if (unit.GetComponentInChildren<Health>().isBase)
        {
            Team.TeamType team = unit.GetComponent<Team>().unitTeam;

            if (team == Team.TeamType.Player) { SceneManager.LoadScene("Lose Screen"); }//ChangeGameState(GameState.WIN);
            else if (team == Team.TeamType.Enemy) { SceneManager.LoadScene("Win Screen"); }//ChangeGameState(GameState.LOSE);
        }

        // Call for potential bonus item generations
        if(unit.GetComponent<Team>().unitTeam == Team.TeamType.Enemy && unit.GetComponent<Team>().unitType == Team.UnitType.Dynamic)
        {
            SpawnReward(new Vector3(unit.transform.position.x, 1, unit.transform.position.z));
        }

        // Remove from our unit list
        if (pUnitList.Contains(unit)) pUnitList.Remove(unit);
        if (pSelectedList.Contains(unit)) pSelectedList.Remove(unit);
        if (eUnitList.Contains(unit)) eUnitList.Remove(unit);
    }

	/*
	 *	Function:	BuildingPlacer_OnPlacingBuilding
	 *	Purpose:	Set placing building flag to true
	 *	In:			isPlacingBuilding (True if the player is placing a building false otherwise)
	 */
    void BuildingPlacer_OnPlacingBuilding(bool isPlacingBuilding)
    {
        _buildingMode = isPlacingBuilding;
    }

	/*
	 *	Function:	IsOverUI
	 *	Purpose:	Check to see if mouse is over a UI element to prevent raycasting
	 *	Return:		bool (True if over UI false otherwise) 
	 */
    bool IsOverUI()
    {
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }

	/*
	 *	Function:	RayCastAttempt
	 *	Purpose:	Attempt to hit an object with raycast
	 *	In:			mPos (Player cursor position)
	 *	In:			clickType (0 = left click, 1 = right click and 2 = centre click)
	 */
    void RayCastAttempt(Vector3 mPos, int clickType)
    {
        if (!IsOverUI())
        {
            // Create our raycast
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(mPos);

            bool isHit = false;

            // Check for hit
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                // If we hit an object
                if (hit.collider)
                {
                    isHit = true;

                    // Grab the object
                    selectedUnit = hit.transform.gameObject;

                    // If the GameObject has the ability to be selected
                    if (selectedUnit.GetComponent<Selectable>())
                    {
                        // Let listeners know we have selected a unit
                        OnUnitSelected?.Invoke(selectedUnit);

                        switch (clickType)
                        {
                            case 0: // We may have selected a unit
								Debug.Log("Unit clicked");
                                CheckUnitClick(selectedUnit);
                                break;
                            case 1: // Right click functionality if necessary
                                break;
                            case 2: // We want to focus on the unit or building
                                FocusOnUnit(selectedUnit);
                                break;
                            default: break;
                        }
                    }
                    else
					{
                        // Make sure we only move with left click
						if(_selectedType == SelectedType.DYNAMIC && clickType == 0)
						{
							_aiManager.MoveUnits(pSelectedList, hit.point);
						}
						else DeselectAll(); // No units were selected, so deselect everything
					}
                }
            }
            if (!isHit) DeselectAll(); // Nothing was selected at all
        }
    }

	/*
	 *	Function:	BoxDragger_OnDragSelection
	 *	Purpose:	Select units within the players dragged box
	 *	In:			selectRect (The bounded rect within which to check if units are selected)
	 */
    public void BoxDragger_OnDragSelection(Rect selectRect)
    {
        // We want nothing selected at the start
        DeselectAll();

        foreach (GameObject unit in pUnitList)
        {
            // If our rect is cast and has units within it's bounds, select them
            if (selectRect.Contains(Camera.main.WorldToScreenPoint(unit.transform.position)))
            {
                if(unit.GetComponent<Team>().unitType == Team.UnitType.Dynamic) SelectUnit(unit);
            }
        }

        // We only select dynamic units this way
        if (pSelectedList.Count > 0) _selectedType = SelectedType.DYNAMIC;
    }

	/*
	 *	Function:	FocusOnUnit
	 *	Purpose:	Focus camer on the unit centre clicked by the player
	 *	In:			unit (Unit on which to focus)
	 */
	public void FocusOnUnit(GameObject unit)
	{
		OnFocusSelected?.Invoke(unit);
	}

	/*
	 *	Function:	CheckUnitClick
	 *	Purpose:	Check if this is a unit player can select
	 *	In:			gObj (GameObject selected by player)
	 */
    void CheckUnitClick(GameObject gObj)
    {
        // Don't select units if we are building
        if (!_buildingMode)
        {
            // Get team
            Team team = gObj.GetComponent<Team>();

            // If selecting a static object, only one may be selected at a time
            if (_selectedType == SelectedType.STATIC)
            {
                DeselectAll();
            }

            // We have a dynamic player unit that can be selected
            if (team != null && team.unitTeam == Team.TeamType.Player && team.unitType == Team.UnitType.Dynamic)
            {
                // If holding shift, we can select addition units
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (pSelectedList.Contains(gObj)) DeselectUnit(gObj);
                    else SelectUnit(gObj);
                }
                else
                {
                    DeselectAll();
                    SelectUnit(gObj);
                }
				_selectedType = SelectedType.DYNAMIC;
            }
            else if (team != null && team.unitTeam == Team.TeamType.Player && team.unitType == Team.UnitType.Static)
            {
                _selectedType = SelectedType.STATIC;
                DeselectAll();
                SelectUnit(gObj);
            }
            else DeselectAll();
        }
    }

	/*
	 *	Function:	SelectUnit
	 *	Purpose:	Add unit to selected list, activate selected orb and broadcast selection
	 *	In:			gObj (GameObject selected by player)
	 */
    void SelectUnit(GameObject gObj)
    {
        // Add to selection group and activate selection orb
        pSelectedList.Add(gObj);
        gObj.GetComponent<Selectable>().selectionOrb.SetActive(true);
        OnUnitSelected?.Invoke(gObj);
    }

	/*
	 *	Function:	DeselectUnit
	 *	Purpose:	Remove unit from selected list and deactivate selected orb
	 *	In:			gObj (GameObject to be deselected)
	 */
    void DeselectUnit(GameObject gObj)
    {
        pSelectedList.Remove(gObj);
        gObj.GetComponent<Selectable>().selectionOrb.SetActive(false);

		if (pSelectedList.Count == 0) _selectedType = SelectedType.NONE;
    }

	/*
	 *	Function:	DeselectAll
	 *	Purpose:	Remove all units from selected list, deactivate all units selected orb and broadcast all units deselection
	 */
    void DeselectAll()
    {
		_selectedType = SelectedType.NONE;
        // Clears our list
        foreach (GameObject gObj in pSelectedList)
        {
            gObj.GetComponent<Selectable>().selectionOrb.SetActive(false);
        }
        pSelectedList.Clear();
		OnDeselectAll?.Invoke();
    }

	/*
	 *	Function:	TrainUnit
	 *	Purpose:	Broadcast data index of unit to train
	 *	In:			unitDataIndex (Data index of unit to train)
	 */
	public void TrainUnit(int unitDataIndex)
	{
		OnTrainUnit?.Invoke(unitDataIndex);
	}

	/*
	 *	Function:	SpawnUnit
	 *	Purpose:	Spawn a unit at a given position with a default player team
	 *	In:			spawnPosition (Position at which to spawn unit)
	 *	In:			unitDataIndex (Data index of unit to spawn)
	 *	In:			team (Team to which to assign unit, defaulted to player)
	 *	Return:		Unit (Unit spawned)
	 */
    public Unit SpawnUnit(Vector3 spawnPosition, int unitDataIndex, Team.TeamType team = Team.TeamType.Player)
    {
        return unitSpawner.SpawnUnit(spawnPosition, unitDataIndex, team);
    }

	/*
	 *	Function:	RegisterCostsIncurred
	 *	Purpose:	Keep track of player costs
	 *	In:			costs (Costs incurred by the player)
	 */
    public void RegisterCostsIncurred(int costs)
	{
		_costs += costs;
	}

	/*
	 *	Function:	BakeNavMeshes
	 *	Purpose:	Bake the nav meshes used for unit path finding
	 */
	public void BakeNavMeshes()
	{
		goblinNavMesh.BuildNavMesh();
		tankNavMesh.BuildNavMesh();
	}

	/*
	 *	Function:	ChangeGameState
	 *	Purpose:	Change the game state and broadcast the next state
	 *	In:			state (The new game state to which to change the current game state)
	 */
    void ChangeGameState(GameState state)
    {
        _gameState = state;
        OnGameStateChange?.Invoke(state);
    }

    // Try to spawn a bonus item
	/*
	 *	Function:	SpawnReward
	 *	Purpose:	Spawn reward drop when a unit is destroyed
	 *	In:			pos (Position at which to drop the unit)
	 */
    public void SpawnReward(Vector3 pos)
    {
        // Prevent some logic from preventing cleanup at game end
        if (!gameObject.scene.isLoaded) return;
        
        int rand1 = Random.Range(0, 100);

        // 25% chance of item spawning
        if (rand1 > 74)
        {
            // Type of item to spawn
            int rand2 = Random.Range(0, 3);

            switch (rand2)
            {
                case 0:
                    Instantiate(rewardItems[0], pos, Quaternion.identity);
                    break;
                case 1:
                    Instantiate(rewardItems[1], pos, Quaternion.identity);
                    break;
                case 2:
                    Instantiate(rewardItems[2], pos, Quaternion.identity);
                    break;
                default:
                    break;
            }
        }
    }
}
