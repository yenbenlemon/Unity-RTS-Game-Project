using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
	/*
	 *	Funciton:	_CalculateMusterPosition
	 *	Purpose:	Calculate the postion to which units are to move after spawning
	 *	In:			spawnPosition (Position at which unit is spawning)
	 *	Return:		Vector3 (The muster postiion to which the unit is to move)
	 */
	Vector3 _CalculateMusterPosition(Vector3 spawnPosition)
	{
		Vector3 direction = Vector3.Normalize(new Vector3(-spawnPosition.x, spawnPosition.y, -spawnPosition.z));
		return spawnPosition + (3.0f * direction);
	}

	/*
	 *	Funciton:	SpawnUnit
	 *	Purpose:	Spawn a unit of a given data index, at a given position for a given team
	 *	In:			spawnPosition (Position at which to spawn the unit)
	 *	In:			unitDataIndex (Data index of unit)
	 *	In:			team (Team to whom unit belongs)
	 *	Return:		Unit (The unit spawned)
	 */
	public Unit SpawnUnit(Vector3 spawnPosition, int unitDataIndex, Team.TeamType team)
	{
		Debug.Log("Training unit" + unitDataIndex);
		Unit unit = new Unit(Globals.UNIT_DATA[unitDataIndex]);

		if (team == Team.TeamType.Enemy)
		{
			unit.Transform.GetChild(0).gameObject.SetActive(false);
		}
		
		// Set the team before initializing
		unit.Transform.GetComponent<Team>().unitTeam = team;

		// Initializes the Unit
		unit.Transform.GetComponent<UnitManager>().Initialize(unit);

		// Set position and team
		unit.Transform.gameObject.SetActive(false);
		unit.Position = spawnPosition;
		unit.Transform.gameObject.SetActive(true);

		unit.Transform.GetComponent<Movement>().Seek(_CalculateMusterPosition(spawnPosition));

		return unit;
	}
}
