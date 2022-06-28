using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTrainer : EventManager
{
	// Tell Listeners that a cost was incurred
	GameObject _menu;

	bool _selected;

	int _availableResources = 0;

	bool IsPlacingBuilding { get => GameManager.Instance.BuildingMode; }

	void Awake()
	{
		// Get Menu
		_menu = GameObject.Find("UI Manager").transform.Find(gameObject.tag + " Train Menu").gameObject;
		_menu.SetActive(false);
		_selected = false;
	}

	void OnEnable()
	{
		GameManager.OnTrainUnit				+= _TrainUnit;
		GameManager.OnUnitSelected			+= _DisplayUnitsToTrain;
		GameManager.OnDeselectAll			+= _ClearUnitsToTrain;

		// Resource listener
        GameManager.OnResourceUpdate 		+= GameManager_OnResourceUpdate;  
	}

	void OnDisable()
	{
		GameManager.OnTrainUnit 			-= _TrainUnit;
		GameManager.OnUnitSelected			-= _DisplayUnitsToTrain;
		GameManager.OnDeselectAll			-= _ClearUnitsToTrain;


		// Resource listener
        GameManager.OnResourceUpdate 		-= GameManager_OnResourceUpdate;
	}

	/*
	 *	Function:	_TrainUnit
	 *	Purpose:	Spawn a unit of a certain data index at a calculated position
	 *	In:			unitDataIndex (Data index of the unit chosen to spawn)
	 */
	void _TrainUnit(int unitDataIndex)
	{
		_TrainUnit(_CalculateSpawnPosition(transform.position), unitDataIndex);
	}
	
	/*
	 *	Function:	_TrainUnit
	 *	Purpose:	Spawn a unit of a certain data index at a given position and update costs and internal tracking of available player
	 *	In:			spawnPosition (Position at which to spawn unit)
	 *	In:			unitDataIndex (Data index of the unit chosen to spawn)
	 */
	void _TrainUnit(Vector3 spawnPosition, int unitDataIndex)
	{
		if (_selected)
		{
			int unitCost = Globals.UNIT_DATA[unitDataIndex].Cost;
			if (unitCost <= _availableResources)
			{
				GameManager.Instance.RegisterCostsIncurred(-unitCost);
				GameManager.Instance.SpawnUnit(spawnPosition, unitDataIndex);
				// Handle checkout
				_availableResources -= unitCost;
			}
		}
	}

	/*
	 *	Function:	_CalculateSpawnPosition
	 *	Purpose:	Calculate the position at which to spawn
	 *	In:			sourcePosition (Position of item/ building spawning unit)
	 *	Return:		Vector3 (Position at which to spawn unit)
	 */
	Vector3 _CalculateSpawnPosition(Vector3 sourcePosition)
	{
		Vector3 direction = Vector3.Normalize(new Vector3(-sourcePosition.x, sourcePosition.y, -sourcePosition.z));
		return sourcePosition + (3.0f * direction);
	}

	/*
	 *	Function:	_DisplayUnitsToTrain
	 *	Purpose:	Display UI button for units available to train at given selected building
	 *	In:			unit (Unit selected by player)
	 */
	void _DisplayUnitsToTrain(GameObject unit)
	{
		if (!IsPlacingBuilding)
		{
			if (unit == gameObject)
			{
				_selected = true;
				_menu.SetActive(true);
			}
			else
			{
				_selected = false;

				if (unit.tag != gameObject.tag)
				{
					_menu.SetActive(false);
				}
			}
		}
	}

	/*
	 *	Function:	_ClearUnitsToTrain
	 *	Purpose:	Hide UI button for units available to train at given building
	 */
	void _ClearUnitsToTrain()
	{
		_menu.SetActive(false);
		_selected = false;
	}

	/*
	 *	Function:	GameManager_OnResourceUpdate
	 *	Purpose:	Update internal record of player resources
	 *	In:			resourceAmount (Player's current level of resources)
	 */
	void GameManager_OnResourceUpdate(int resourceAmount)
	{
		// Store most recent resource amount for validations
		_availableResources = resourceAmount;
	}
}
