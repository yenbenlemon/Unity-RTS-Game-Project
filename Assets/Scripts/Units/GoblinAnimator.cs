using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoblinAnimator : MonoBehaviour
{
    public Animator anim;
    public NavMeshAgent navAgent;
    public Attack attack;

    // Start is called before the first frame update
    void Start()
    {
        // Start with our idle animation that loops
        IdleAnim();
    }

    void OnEnable()
    {
        // Subscribe for attack animation
        Attack.OnUnitAttack += AttackAnim;
    }

    void OnDisable()
    {
        // Unsubscribe
        Attack.OnUnitAttack -= AttackAnim;
    }

    // Update is called once per frame
    void Update()
    {
        // Walk / Idle based on movement speed
        if(navAgent.velocity.magnitude > 0)
        {
            WalkAnim();
        }
        else
        {
            IdleAnim();
        }
    }

	/*
	 *	Function:	IdleAnim
	 *	Purpose:	Show goblin idle animation
	 */
    void IdleAnim()
    {
        // Transition idle animation
        anim.SetFloat("Speed", 0, 0.1f, Time.deltaTime);
    }

	/*
	 *	Function:	WalkAnim
	 *	Purpose:	Show goblin walk animation
	 */
    void WalkAnim()
    {
        // Transition to walk animation
        anim.SetFloat("Speed", 1, 0.1f, Time.deltaTime);
    }

	/*
	 *	Function:	AttackAnim
	 *	Purpose:	Show goblin attack animation
	 *	In:			unit (?)
	 *	In:			isAttacking (True if goblin is attacking false otherwise)
	 */
    void AttackAnim(GameObject unit, bool isAttacking = true)
    {
        // Make sure this only animates for goblins that are actcually attacking
        if(attack.hasTarget) anim.Play("Attack"); 
    }
}
