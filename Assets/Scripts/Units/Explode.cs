using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour
{
    // Grab our health and team values to confirm we can explode
    public Health health;
    public Team team;

    // Random values. Can be set in inspector
    public float explosionRadius = 10f;
    public float explosionPower = 75f;

    // Our particle effect for the explosion
    public GameObject explosionPrefab;

    // We only attempt explosionss once and they only have a 25% chance
    private bool hasAttempted = false;
    private int explodeChance = 25;

    // Update is called once per frame
    void Update()
    {
        // Only a player tank can explode
        if ((health.currentHP / health.maxHP < 1) && team.unitTeam == Team.TeamType.Player && hasAttempted == false)
        {
            // We only attempt an explosion once per the life of the Tank
            hasAttempted = true;

            // Grab a random int to see if we explode
            int rand = Random.Range(0, 100);

            // We have exploded
            if(rand > explodeChance)
            {
                // Create the explosion
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                
                // Damage nearby enemy units
                AOEDamage();

                // Destroy our tank
                Destroy(gameObject);
            }
        }
    }

	/*
	 *	Function:	AOEDamage
	 *	Purpose:	Deliver Area of Effect (AOE) damage to units within AOE
	 */
    void AOEDamage()
    {
        // Create a hit radius collider that will simulate our attack range
        Collider[] hitColliders = Physics.OverlapSphere(transform.localPosition, explosionRadius);

        foreach (var hitCollider in hitColliders)
        {
            // Attempt to get the team component for a unit
            Team hitType = hitCollider.GetComponent<Team>();

            // Check if we are working with a unit in a team
            if (hitType != null && hitType.unitTeam == Team.TeamType.Enemy)
            {
                // Deal damage to an enemy unit (static or dynamic)
                hitType.GetComponentInChildren<Health>().UpdateHP(-explosionPower);
            }
        }
    }
}
