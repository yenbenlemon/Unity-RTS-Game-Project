using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
	/*
	 *	Function:	MoveUnits
	 *	Purpose:	Make all selected player units seek to a destiantion
	 *	In:			selecteedUnits (Dynamic units selected by the player)
	 *	In:			destination (The location to which the units are to travel)
	 */
	public void MoveUnits(List<GameObject> selectedUnits, Vector3 destination)
	{
		foreach (GameObject unit in selectedUnits)
		{
			Movement unitMovement = unit.GetComponent<Movement>();
			unitMovement.Agent.isStopped = false;
			unitMovement.Seek(destination);
		}
	}
}