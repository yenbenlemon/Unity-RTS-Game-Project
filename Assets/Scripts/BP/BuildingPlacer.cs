using UnityEngine;

public class BuildingPlacer : EventManager
{
	Building _buildingBlueprint = null;

	Ray _ray;
	RaycastHit _raycastHit;
	Vector3 _lastPlacementPosition;

	int _availableResources = 0;

	void OnEnable()
    {
        KeyboardInputManager.OnBuildInput		+= _PrepareBuildingBlueprint;
        KeyboardInputManager.OnCancelBuildInput	+= _CancelBuildingBlueprint;
		MouseInputManager.OnLeftClickInput		+= _PlaceValidBuilding;
		MouseInputManager.OnBuildInput			+= _PositionBlueprint;

		// Resource listener
        GameManager.OnResourceUpdate 			+= GameManager_OnResourceUpdate;  
	}

	void OnDisable()
    {
		// Remove listeners
        KeyboardInputManager.OnBuildInput		-= _PrepareBuildingBlueprint;
        KeyboardInputManager.OnCancelBuildInput	-= _CancelBuildingBlueprint;
		MouseInputManager.OnLeftClickInput		-= _PlaceValidBuilding;
		MouseInputManager.OnBuildInput			-= _PositionBlueprint;

        GameManager.OnResourceUpdate 			-= GameManager_OnResourceUpdate;
	}

	/*
	 *	Function:	GameManager_OnResourceUpdate
	 *	Purpose:	Update player's available resources
	 *	in:			resourceAmount (current amount of player resources)
	 */
	void GameManager_OnResourceUpdate(int resourceAmount)
	{
		// Store most recent resource amount for validations
		_availableResources = resourceAmount;
	}

	/*
	 *	Function:	_PlaceValidBuilding
	 *	Purpose:	Handle user left mouse click and place building given certain conditions
	 *	in:			mousePosition (not used; position of mouse in screen space)
	 */
    void _PlaceValidBuilding(Vector3 mousePosition)
	{
		if (_buildingBlueprint != null
			&& _buildingBlueprint.Cost <= _availableResources
			&& _buildingBlueprint.HasValidPlacement)
		{
			_PlaceBuilding();
		}
	}

	/*
	 *	Function:	_PositionBlueprint
	 *	Purpose:	Handle positioning building blueprint given mouse position
	 *	in:			mousePosition (mouse position in screen space)
	 */
	void _PositionBlueprint(Vector3 mousePosition)
	{
		// Reposition blueprint according to mouse pointer
        if (_buildingBlueprint != null)
		{
			// Do raycast
			_ray = Camera.main.ScreenPointToRay(mousePosition);

			if (Physics.Raycast(_ray, out _raycastHit, 10000.0f, Globals.TERRAIN_LAYER_MASK))
			{
				// Snap Building to position
				Vector3 snappedPosition = new Vector3(Mathf.RoundToInt(_raycastHit.point.x), Mathf.RoundToInt(_raycastHit.point.y), Mathf.RoundToInt(_raycastHit.point.z));
				
				_buildingBlueprint.SetPosition(snappedPosition);

				if(_lastPlacementPosition != snappedPosition)
				{
					_lastPlacementPosition = snappedPosition;
				}
			}

			
		}
	}

	/*
	 *	Function:	_CancelBuildingBlueprint
	 *	Purpose:	Destroy the building blueprint
	 */
	void _CancelBuildingBlueprint()
	{
		if (_buildingBlueprint != null)
		{
			// destroy building blueprint
			Destroy(_buildingBlueprint.Transform.gameObject);
			_buildingBlueprint = null;
			GameManager.Instance.BuildingMode = false;
		}
	}

	/*
	 *	Function:	_PrepareBuildingBlueprint
	 *	Purpose:	Destroy any existing blueprint and and initialize new building blueprint
	 *	in:			buildingDataIndex (index of data for building blueprint)
	 */
	void _PrepareBuildingBlueprint(int buildingDataIndex)
	{
		// destroy previous building blueprint if one exists
		if (_buildingBlueprint != null && !_buildingBlueprint.IsFixed)
		{
			Destroy(_buildingBlueprint.Transform.gameObject);
		}

		Building building = new Building(Globals.BUILDING_DATA[buildingDataIndex]);

		building.Transform.GetComponent<BuildingManager>().Initialize(building);

		_buildingBlueprint = building;
		_buildingBlueprint.SetPosition(_lastPlacementPosition);

		GameManager.Instance.BuildingMode = true;
	}

	/*
	 *	Function:	_PlaceBuilding
	 *	Purpose:	Place the current building blueprint and create a new building blueprint
	 */
	void _PlaceBuilding()
	{
		GameManager.Instance.BakeNavMeshes();
		GameManager.Instance.RegisterCostsIncurred(-_buildingBlueprint.Cost);
		_availableResources -= _buildingBlueprint.Cost;
		_buildingBlueprint.Place();

		// Keep building same building time
		_PrepareBuildingBlueprint(_buildingBlueprint.BuildingDataIndex);
	}
}
