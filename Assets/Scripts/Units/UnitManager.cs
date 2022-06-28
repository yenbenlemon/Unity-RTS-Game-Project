using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : EventManager
{
    private Team _ourType;
    private Attack _ourAttack;

	private Unit _unit = null;

	public Unit Unit { get => _unit; }

	/*
	 *	Function:	Initialize
	 *	Purpose:	Initialize building blueprint
	 *	in:			building (building to initialize)
	 */
	public void Initialize(Unit unit)
	{
		_ourType = this.GetComponent<Team>();
		_ourAttack = this.GetComponent<Attack>();
		_unit = unit;
    }

    // Update is called once per frame
    void Update()
    {
        // No point to check if we still have a target
        if (!_ourAttack.hasTarget)
        {
            // Create a hit radius collider that will simulate our attack range
            Collider[] hitColliders = Physics.OverlapSphere(transform.localPosition, _ourAttack.atkRadius);

            foreach (var hitCollider in hitColliders)
            {
                // Attempt to get the team component for a unit
                Team hitType = hitCollider.GetComponent<Team>();

                // Check if we are working with a unit in a team
                if (hitType != null)
                {
                    // If we are of opposite teams, we must set target
                    if ((_ourType.unitTeam == Team.TeamType.Enemy && hitType.unitTeam == Team.TeamType.Player) ||
                        (_ourType.unitTeam == Team.TeamType.Player && hitType.unitTeam == Team.TeamType.Enemy))
                    {
                        this.GetComponent<Attack>().SetTarget(hitCollider.gameObject);
                    }
                }
            }
        }
    }
}
