using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repair : MonoBehaviour
{
    // Grab our health and team values to confirm we can explode
    public Health health;
    public Team team;

    // Random values. Can be set in inspector
    public float repairRadius = 10;
    public float reapirPower = 15;

    // Our particle effect for the explosion
    public GameObject sparklePrefab;

    // We only attempt explosionss once and they only have a 25% chance
    bool hasAttempted = false;
    int repairChance = 25;

    // If we were destroyed we should try to cause a repair aura
    void OnDestroy()
    {
        // Prevent some logic from preventing cleanup at game end
        if (!gameObject.scene.isLoaded) return;

        // Only a player tank can explode
        if ((health.currentHP / health.maxHP < 1) && team.unitTeam == Team.TeamType.Player)
        {
            // Grab a random int to see if we explode
            int rand = Random.Range(0, 100);

            // We have repaired
            if (rand > repairChance)
            {
                // Create the explosion
                Instantiate(sparklePrefab, transform.position, Quaternion.identity);

                // Repair nearby player units
                AOERepair();
            }
        }
    }

	/*
	 *	Function:	AOERepair
	 *	Purpose:	Deliver Area of Effect (AOE) repair to allied units within AOE
	 */
    void AOERepair()
    {
        // Create a hit radius collider that will simulate our attack range
        Collider[] hitColliders = Physics.OverlapSphere(transform.localPosition, repairRadius);

        foreach (var hitCollider in hitColliders)
        {
            // Attempt to get the team component for a unit
            Team hitType = hitCollider.GetComponent<Team>();

            // Check if we are working with a unit in a team
            if (hitType != null && hitType.unitTeam == Team.TeamType.Player)
            {
                // Repair a player unit (static or dynamic)
                hitType.GetComponentInChildren<Health>().UpdateHP(reapirPower);
            }
        }
    }
}
