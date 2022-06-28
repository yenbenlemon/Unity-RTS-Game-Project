using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPurchaser
{
	int _minBuildingPosture = 1;

	Vector3 _plateauCentre;
	float _keyBuildingRadius = 5.0f;
	int _wallSize = 10;
	int _wallGap = 2;

	Hashtable _enemyBuildings;

	Hashtable _buildingIndexLookup;
	List<string> _keyBuildingCodes;

	List<Vector3> _wallPositions;
	Hashtable _walls;

	public bool NoKeyBuildings { get => _enemyBuildings.Count < _minBuildingPosture; }

	public Hashtable EnemyBuildings { get => _enemyBuildings; }
	List<string> _EnemyBuildingCodes { get => _enemyBuildings.Keys.Cast<string>().ToList(); }
	List<int> _BuildingCodes { get => _buildingIndexLookup.Keys.Cast<int>().ToList(); }

	Building _FirstKeyBuilding { get => (Building)_enemyBuildings[_EnemyBuildingCodes[0]]; }

	List<Vector3> _OccupiedWallPositions { get => _walls.Keys.Cast<Vector3>().ToList(); }
	List<Vector3> _AvailableWallPositions { get => (_wallPositions.Except(_OccupiedWallPositions)).ToList(); }

	int _BuildingIndex(string buildingCode) { return (int)_buildingIndexLookup[buildingCode]; }
	BuildingData _BuildingData(string buildingCode) { return Globals.BUILDING_DATA[_BuildingIndex(buildingCode)]; }
	int _BuildingCost(string buildingCode) { return _BuildingData(buildingCode).Cost; }
	
	Building _KeyBuilding(string buildingCode) { return (Building)_enemyBuildings[buildingCode]; }
	Building _Wall(Vector3 buildingPosition) { return (Building)_walls[buildingPosition]; }

	/*
	 *	Function:	CheckForStructuresToDestroy
	 *	Purpose:	Check and remove the destroyed unit if it exists in one of the enemy AI's collections
	 *	In:			unitDestroyed (The destroyed unit for which to look)
	 */
	public void CheckForStructuresToDestroy(GameObject unitDestroyed)
	{
		_CheckForDestroyedBuildings(unitDestroyed);
		_CheckForDestroyedWalls(unitDestroyed);
	}

	/*
	 *	Function:	_CheckForDestroyedBuildings
	 *	Purpose:	Check and remove the destroyed unit if it exists in the key enemy buildings collections
	 *	In:			unitDestroyed (The destroyed unit for which to look)
	 */
	void _CheckForDestroyedBuildings(GameObject unitDestroyed)
	{
		Hashtable existingBuildings = new Hashtable();

		foreach (string buildingIdentifier in _enemyBuildings.Keys)
		{
			GameObject building = _KeyBuilding(buildingIdentifier).Transform.gameObject;
			if (building != unitDestroyed)
			{
				Debug.Log(buildingIdentifier + " Exists");
				existingBuildings.Add(buildingIdentifier, _KeyBuilding(buildingIdentifier));
			}
		}

		_enemyBuildings = existingBuildings;
	}

	/*
	 *	Function:	_CheckForDestroyedWalls
	 *	Purpose:	Check and remove the destroyed wall if it exists in the walls collections
	 *	In:			unitDestroyed (The destroyed unit for which to look)
	 */
	void _CheckForDestroyedWalls(GameObject unitDestroyed)
	{
		Hashtable existingWalls = new Hashtable();

		foreach (Vector3 buildingIdentifier in _walls.Keys)
		{
			GameObject building = _Wall(buildingIdentifier).Transform.gameObject;
			if (building != unitDestroyed)
			{
				Debug.Log(buildingIdentifier + " Exists");
				existingWalls.Add(buildingIdentifier, _Wall(buildingIdentifier));
			}
		}

		_walls = existingWalls;
	}

	// Constructor
	public BuildingPurchaser(Vector3 plateauCentre)
	{
		_plateauCentre = plateauCentre;

		_enemyBuildings = new Hashtable();
		_buildingIndexLookup = new Hashtable();
		_keyBuildingCodes = new List<string>();

		_wallPositions = new List<Vector3>();
		_walls = new Hashtable();

		_InitializeWallPositions();
		_InitializeBuildingLookups();
	}

	/*
	 *	Function:	_InitializeWallPositions
	 *	Purpose:	Initialize a collection of all possible wall positions
	 */
	void _InitializeWallPositions()
	{
		for (int z = -_wallSize; z <= _wallSize; ++z)
		{
			for (int x = -_wallSize; x <= _wallSize; ++x)
			{
				if ((_wallSize == Mathf.Abs(x) || _wallSize == Mathf.Abs(z)) && Mathf.Abs(x) > _wallGap && Mathf.Abs(z) > _wallGap)
				{
					_wallPositions.Add(new Vector3(_plateauCentre.x + x, _plateauCentre.y, _plateauCentre.z + z));
				}
			}
		}
	}

	/*
	 *	Function:	_InitializeBuildingLookups
	 *	Purpose:	Initialize look ups for key buildings and building code to building data index
	 */
	void _InitializeBuildingLookups()
	{
		for (int i = 0; i < Globals.BUILDING_DATA.Length; ++i)
		{
			string buildingCode = Globals.BUILDING_DATA[i].Code;

			if (buildingCode != "Command Base")
			{
				_buildingIndexLookup.Add(buildingCode, i);

				if (buildingCode != "Wall")
				{
					_keyBuildingCodes.Add(buildingCode);
				}
			}
		}
	}

	/*
	 *	Function:	_MissingKeyBuilding
	 *	Purpose:	Key buildings that have not yet been built by the AI
	 *	Return:		string (Building code if the AI has yet to build a key building, null otherwise)
	 */
	string _MissingKeyBuilding()
	{
		foreach (string keyBuildingCode in _keyBuildingCodes)
		{
			if (!_EnemyBuildingCodes.Contains(keyBuildingCode))
			{
				return keyBuildingCode;
			}
		}

		return null;
	}

	/*
	 *	Function:	BuildingAvailableForPurchase
	 *	Purpose:	Check if there are buildings that have not yet been built by the AI
	 *	Return:		bool (True if has yet to build a key building, false otherwise)
	 */
	public bool BuildingAvailableForPurchase()
	{
		string missingBuilding = _MissingKeyBuilding();

		return missingBuilding != null;
	}

	/*
	 *	Function:	FundsAvailableForBuildingPurchase
	 *	Purpose:	Check if tenemy AI has teh funds to purchase the building
	 *	In:			resources (The amount of funds held by the enemy AI)
	 *	Return:		bool (True if the enemy AI can build a missing key building, false otherwise)
	 */
	public bool FundsAvailableForBuildingPurchase(int resources)
	{
		string missingBuilding = _MissingKeyBuilding();

		return (resources >= _BuildingCost(missingBuilding));
	}

	/*
	 *	Function:	_ConstructBuilding
	 *	Purpose:	Instatiate a building, set it's position in world space and place it
	 *	In:			buildingCode (The building the enemy AI wants to build)
	 *	In:			buildingPosition (The place where the enemy AI wants to build the building)
	 *	Return:		Building (The building instantiated)
	 */
	Building _ConstructBuilding(string buildingCode, Vector3 buildingPosition)
	{
		Building building = new Building(_BuildingData(buildingCode));
		building.Transform.GetComponent<BuildingManager>().Initialize(building);

		building.SetPosition(buildingPosition);
		building.Place();

		return building;
	}

	/*
	 *	Function:	_BuyAndPlaceBuilding
	 *	Purpose:	Construct the building, remove its fog of war aperture and set it's team to enemy
	 *	In:			buildingCode (The building the enemy AI wants to build)
	 *	In:			position (The place where the enemy AI wants to build the building)
	 *	Out:		building (the buiilding constructed)
	 *	Return:		int (The cost of the building)
	 */
	int _BuyAndPlaceBuilding(string buildingCode, Vector3 position, out Building building)
	{
		building = _ConstructBuilding(buildingCode, position);

		building.Transform.GetChild(0).gameObject.SetActive(false);
		building.Transform.GetComponent<Team>().unitTeam = Team.TeamType.Enemy;

		return building.Cost;
	}

	/*
	 *	Function:	_ChooseBuildingPosition
	 *	Purpose:	Choose the position for a key building
	 *	Return:		Vector3 (The postion to place a building)
	 */
	Vector3 _ChooseBuildingPosition()
	{
		Vector3 nextDirection = Vector3.Normalize(new Vector3(Random.value, 0, Random.value));

		if (_enemyBuildings.Count > 0)
		{
			Vector3 firstKeyPosition = _FirstKeyBuilding.Transform.position - _plateauCentre;
			nextDirection =  Vector3.Normalize(new Vector3(firstKeyPosition.x, 0, firstKeyPosition.z));
		}

		Vector3 position = _plateauCentre - (nextDirection * _keyBuildingRadius);
		
		return position;
	}

	/*
	 *	Function:	PurchaseBuilding
	 *	Purpose:	Choose a building, a position, place the building and remove the cost from the enemy AI's funds
	 *	In:			resourcesAvailable (The resources held by the enemy AI)
	 *	Return:		int (The balance of the enemy AI's funds)
	 */
	public int PurchaseBuilding(int resourcesAvailable)
	{
		string missingBuildingCode = _MissingKeyBuilding();
		Vector3 position = _ChooseBuildingPosition();

		Building building;
		int cost = _BuyAndPlaceBuilding(missingBuildingCode, position, out building);
		_enemyBuildings.Add(missingBuildingCode, building);

		return resourcesAvailable - cost;
	}

	/*
	 *	Function:	CanBuildWall
	 *	Purpose:	Check if the enemy AI has the funds and a place to build a wall
	 *	In:			resourcesAvailable (The resources held by the enemy AI)
	 *	Return:		bool (True if the enemy AI has the funds and a place to build a wall)
	 */
	public bool CanBuildWall(int resourcesAvailable)
	{
		return (resourcesAvailable >= Globals.BUILDING_DATA[_BuildingIndex("Wall")].Cost) && _AvailableWallPositions.Count > 0;
	}

	/*
	 *	Function:	BuildWall
	 *	Purpose:	Build a wall at a given position and reduce the enemy AI's funds
	 *	In:			resourcesAvailable (The resources held by the enemy AI)
	 *	Return:		int (The balance of the enemy AI's funds)
	 */
	public int BuildWall(int resourcesAvailable)
	{
		Vector3 position = _AvailableWallPositions[Random.Range(0, _AvailableWallPositions.Count)];

		Building building;
		int cost = _BuyAndPlaceBuilding("Wall", position, out building);
		_walls.Add(position, building);

		return resourcesAvailable - cost;
	}
}
