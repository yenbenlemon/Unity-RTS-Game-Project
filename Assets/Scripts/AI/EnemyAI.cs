using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
	int _resourceAmount = 0;

	float _timer;
	int _moveTimeDelay = 1;

	BuildingPurchaser _buildingPurchaser;
	EnemyUnitTrainer _enemyUnitTrainer;
	UnitCommander _unitCommander;


	void Awake()
	{
		_buildingPurchaser = new BuildingPurchaser(transform.position);
		_enemyUnitTrainer = new EnemyUnitTrainer(_buildingPurchaser);
		_unitCommander = new UnitCommander(transform.position);
	}

	void OnEnable()
	{
		GameManager.OnPayResources += GameManager_OnPayResources; 
		Health.OnUnitDestroyed += _CheckForUnitsToDestroy;
	}

	void OnDisable()
	{
		GameManager.OnPayResources	-= GameManager_OnPayResources; 
		Health.OnUnitDestroyed -= _CheckForUnitsToDestroy;
	}

	/*
	 *	Function:	_CheckForUnitsToDestroy
	 *	Purpose:	Check which of teh enemy AI's units was destroyed
	 *	In:			destroyedUnit (Unit destroyed)
	 */
	void _CheckForUnitsToDestroy(GameObject destroyedUnit)
	{
		_unitCommander.CheckForUnitsToDestroy(destroyedUnit);
		_buildingPurchaser.CheckForStructuresToDestroy(destroyedUnit);
	}

	/*
	 *	Function:	GameManager_OnPayResources
	 *	Purpose:	Pay the enemyAI whenever the resources are doled out by the game manager
	 *	In:			amount (Amount to pay to enemy AI)
	 */
	void GameManager_OnPayResources(int amount)
	{
		_resourceAmount += amount;
	}

    // Update is called once per frame
    void Update()
    {
		_timer += Time.deltaTime;
		
		if (_timer > _moveTimeDelay)
		{
			_timer = 0.0f;

			// Sense
			// Check if military posture is unstaisfactory
			if(_unitCommander.PostureUnsatisfactory || _buildingPurchaser.NoKeyBuildings)
			{
				// Check if no key building has been built
				if (_buildingPurchaser.NoKeyBuildings)
				{
					// Check if funds are available to build the key building
					if (_buildingPurchaser.FundsAvailableForBuildingPurchase(_resourceAmount))
					{
						// Build the key building
						_resourceAmount = _buildingPurchaser.PurchaseBuilding(_resourceAmount);
					}
				}
				else
				{
					// Check if funds are available to train troops
					if (_enemyUnitTrainer.CanTrainNextUnit(_resourceAmount))
					{
						// Train and garrison troops
						Unit newUnit;
						_resourceAmount = _enemyUnitTrainer.TrainNextUnit(_resourceAmount, out newUnit);
						_unitCommander.GarrisonUnit(newUnit);
					}
					// Check if can build wall
					else if (_buildingPurchaser.CanBuildWall(_resourceAmount))
					{
						// Build wall
						_buildingPurchaser.BuildWall(_resourceAmount);
					}
				}
			}
			else
			{
				// Check if a key building is missing
				if (_buildingPurchaser.BuildingAvailableForPurchase())
				{
					// Check if funds are available to build the key building
					if (_buildingPurchaser.FundsAvailableForBuildingPurchase(_resourceAmount))
					{
						// Build the key building
						_resourceAmount = _buildingPurchaser.PurchaseBuilding(_resourceAmount);
					}
					// Check if enough troops are available for attack
					else if (_unitCommander.ThresholdForAttackMet)
					{
						// Send troops to attack
						_unitCommander.DeployUnits();
					}
				}
				else
				{
					// Check if enough troops are available for attack
					if (_unitCommander.ThresholdForAttackMet)
					{
						// Send troops to attack
						_unitCommander.DeployUnits();
					}
					// Check if funds are available to train troops
					else if (_enemyUnitTrainer.CanTrainNextUnit(_resourceAmount))
					{
						// Train and garrison troops
						Unit newUnit;
						_resourceAmount = _enemyUnitTrainer.TrainNextUnit(_resourceAmount, out newUnit);
						_unitCommander.GarrisonUnit(newUnit);
					}
					// Check if can build wall
					else if (_buildingPurchaser.CanBuildWall(_resourceAmount))
					{
						// Build wall
						_buildingPurchaser.BuildWall(_resourceAmount);
					}
				}
			}
		}
    }
}
