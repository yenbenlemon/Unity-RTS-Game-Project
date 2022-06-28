using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnitTrainer
{
	BuildingPurchaser _buildingPurchaser;

	int _nextUnitIndex;

	Hashtable _unitIndexLookup;
	Hashtable _buildingUnitLookup;
	Hashtable _unitBuildingLookup;

	Hashtable _EnemyBuildings { get => _buildingPurchaser.EnemyBuildings; }
	
	List<int> _UnitCodes { get => _unitIndexLookup.Keys.Cast<int>().ToList(); }
	int _NumberOfUnits { get => _unitIndexLookup.Count; }

	List<string> _EnemyBuildingCodes { get => _EnemyBuildings.Keys.Cast<string>().ToList(); }
	string _TrainingBuildingCode { get => (string)_unitBuildingLookup[_UnitCode(_nextUnitIndex)]; }
	Building _TrainingBuilding { get => (Building)_EnemyBuildings[_TrainingBuildingCode]; }

	int _UnitIndex(string unitCode) { return (int)_unitIndexLookup[unitCode]; }
	string _UnitCode(int unitIndex) { return Globals.UNIT_DATA[unitIndex].Code; }
	UnitData _UnitData(string unitCode) { return Globals.UNIT_DATA[_UnitIndex(unitCode)]; }
	int _UnitCost(int unitIndex) { return Globals.UNIT_DATA[unitIndex].Cost; }

	// Constructor
	public EnemyUnitTrainer(BuildingPurchaser buildingPurchaser)
	{
		_nextUnitIndex = 0;

		_unitIndexLookup = new Hashtable();
		_buildingUnitLookup = new Hashtable();
		_unitBuildingLookup = new Hashtable();

		_buildingPurchaser = buildingPurchaser;

		_InitializeUnitLookups();
	}

	/*
	 *	Function:	_InitializeUnitLookups
	 *	Purpose:	Initialize collections for which buildings can train which units and which units are trained by which buildings
	 */
	void _InitializeUnitLookups()
	{
		for (int i = 0; i < Globals.UNIT_DATA.Length; ++i)
		{
			string unitCode = Globals.UNIT_DATA[i].Code;

			_unitIndexLookup.Add(unitCode, i);

			// Not scaleable need better algorithm if time allows
			if (unitCode == "Tank")
			{
				_buildingUnitLookup.Add("Factory", unitCode);
				_unitBuildingLookup.Add(unitCode, "Factory");
			}
			else
			{
				_buildingUnitLookup.Add("Barracks", unitCode);
				_unitBuildingLookup.Add(unitCode, "Barracks");
			}
		}
	}

	/*
	 *	Function:	_SelectUnitToTrain
	 *	Purpose:	Allow enemy AI to chose which unit to train next
	 *	Return:		int (index of the unit to train next)
	 */
	int _SelectUnitToTrain()
	{
		List<int> availableUnitIndices = new List<int>();

		foreach (string buildingCode in _EnemyBuildingCodes)
		{
			availableUnitIndices.Add(_UnitIndex((string)_buildingUnitLookup[buildingCode]));
		}

		if (availableUnitIndices.Count < 1) return -1;

		if (!availableUnitIndices.Contains(_nextUnitIndex))
		{
			_nextUnitIndex = availableUnitIndices[Random.Range(0, availableUnitIndices.Count)];
		}
		
		return _nextUnitIndex;
	}

	/*
	 *	Function:	CanTrainNextUnit
	 *	Purpose:	Check if enemy AI can train the unit it wants to train
	 *	In:			resourcesAvailable (Enemy AI funds available for training)
	 *	Return:		bool (True if unit index is greater than zero and the enemy AI has enough funds to train units)
	 */
	public bool CanTrainNextUnit(int resourcesAvailable)
	{
		int unitChoice = _SelectUnitToTrain();

		return (unitChoice >= 0 && resourcesAvailable >= _UnitCost(unitChoice));
	}

	/*
	 *	Function:	_TrainNextUnit
	 *	Purpose:	Instantiate and spawn next unit chosen by the enemy AI
	 *	Out:		unit (Unit instantiated at time of trianing)
	 *	Return:		int (Cost of the unit trained)
	 */
	int _TrainNextUnit(out Unit unit)
	{
		int lastUnitIndex = _nextUnitIndex;
		Vector3 trainingBuildingPosition = _TrainingBuilding.Transform.position;
		unit = GameManager.Instance.SpawnUnit(trainingBuildingPosition, lastUnitIndex, Team.TeamType.Enemy);

		_nextUnitIndex = (_nextUnitIndex + 1) % _NumberOfUnits;
		
		return _UnitCost(lastUnitIndex);
	}

	/*
	 *	Function:	TrainNextUnit
	 *	Purpose:	Train the next unit chosen by the AI
	 *	In:			resourcesAvailable (Funds held by enemy AI)
	 *	Out:		unit (Unit instantiated at time of trianing)
	 *	Return:		int (Balance of enemy AI funds)
	 */
	public int TrainNextUnit(int resourcesAvailable, out Unit unit)
	{
		return resourcesAvailable - _TrainNextUnit(out unit);
	}
}
