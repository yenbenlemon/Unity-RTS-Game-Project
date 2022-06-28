using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCommander
{
	Vector3 _garrisonPosition;

	int _minTroopPosture = 12;
	int _troopSize = 4;
	Queue _garrisonedUnits;
	List<Unit> _deployedUnits;

	public Queue GarrisonedUnits { get => _garrisonedUnits; }
	public bool ThresholdForAttackMet { get => _garrisonedUnits.Count >= _troopSize; }
	public bool PostureUnsatisfactory { get => _TroopCount < _minTroopPosture; }
	int _TroopCount { get => _garrisonedUnits.Count + _deployedUnits.Count; }
	Unit nextGarrisonedUnit
	{
		get
		{
			if (_garrisonedUnits.Peek() == null) return null;

			return (Unit)_garrisonedUnits.Dequeue();
		}
	}

	// Constructor
	public UnitCommander(Vector3 garrisonPosition)
	{
		_garrisonedUnits = new Queue();
		_deployedUnits = new List<Unit>();

		_garrisonPosition = garrisonPosition;
	}

	/*
	 *	Function:	CheckForUnitsToDestroy
	 *	Purpose:	Check and remove the destroyed unit if it exists in one of the enemy AI's collections
	 *	In:			unitDestroyed (The destroyed unit for which to look)
	 */
	public void CheckForUnitsToDestroy(GameObject unitDestroyed)
	{
		_CheckForGarrisonedUnitsToDestroy(unitDestroyed);
		_CheckForDeployedUnitsToDestroy(unitDestroyed);
	}

	/*
	 *	Function:	_CheckForDeployedUnitsToDestroy
	 *	Purpose:	Check and remove the destroyed unit if it exists in the deployed units collections
	 *	In:			unitDestroyed (The destroyed unit for which to look)
	 */
	void _CheckForDeployedUnitsToDestroy(GameObject unitDestroyed)
	{
		List<Unit> existingDeployedUnits = new List<Unit>();

		foreach (Unit unit in _deployedUnits)
		{
			GameObject unitGameObject = unit.Transform.gameObject;

			if (unitGameObject != unitDestroyed)
			{
				existingDeployedUnits.Add(unit);
			}
		}

		_deployedUnits = existingDeployedUnits;
	}

	/*
	 *	Function:	_CheckForGarrisonedUnitsToDestroy
	 *	Purpose:	Check and remove the destroyed unit if it exists in the garrisoned units collections
	 *	In:			unitDestroyed (The destroyed unit for which to look)
	 */
	void _CheckForGarrisonedUnitsToDestroy(GameObject unitDestroyed)
	{
		Queue existingGarrisonedUnits = new Queue();

		while (_garrisonedUnits.Count > 0)
		{
			Unit unit = nextGarrisonedUnit;
			GameObject unitGameObject = unit.Transform.gameObject;

			if (unitGameObject != unitDestroyed)
			{
				existingGarrisonedUnits.Enqueue(unit);
			}
		}

		_garrisonedUnits = existingGarrisonedUnits;
	}

	/*
	 *	Function:	GarrisonUnit
	 *	Purpose:	Place unit in the garrisoned units collections
	 *	In:			unit (The unit to be placed in the garrisoned unit collection)
	 */
	public void GarrisonUnit(Unit unit)
	{
		_garrisonedUnits.Enqueue(unit);
	}

	/*
	 *	Function:	_ReceiveMarchingOrders
	 *	Purpose:	Send unit to attack player
	 *	In:			unit (The unit to be sent to attack)
	 */
	void _ReceiveMarchingOrders(Unit unit)
	{
		Vector3 attackPosition = new Vector3(-_garrisonPosition.x, _garrisonPosition.y, -_garrisonPosition.z);
		unit.Transform.GetComponent<Movement>().Seek(attackPosition);
	}

	/*
	 *	Function:	DeployUnits
	 *	Purpose:	Remove unit from the garrisoned units collectiona, place it in the deployed units collection and send unit to attack player
	 */
	public void DeployUnits()
	{
		Debug.Log("Deploy Units");
		for (int i = 0; i < _troopSize; ++i)
		{
			Unit unitToDeploy = (Unit)_garrisonedUnits.Dequeue();
			_deployedUnits.Add(unitToDeploy);
			_ReceiveMarchingOrders(unitToDeploy);
		}
	}
}
