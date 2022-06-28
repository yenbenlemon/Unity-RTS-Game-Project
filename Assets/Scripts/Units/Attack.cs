using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : EventManager
{
    public static event UnitAttackHandler OnUnitAttack;

    [Header("Attack Stats")]
    public float attackPower = 5.0f;
    public float fireRate = 5.0f;
    public float atkRadius = 5.0f; // How far the goblin needs to be to attack

    public bool hasTarget = false;

    private float timer = 0.0f;

    private GameObject targetObj;

    // Update is called once per frame
    void Update()
    {
        // Update the timer
        timer = Mathf.Max(0.0f, timer - Time.deltaTime);

        // If we have a target, but they are now out of range, remove them
        if (hasTarget && targetObj != null 
            && Vector3.Distance(transform.position, targetObj.transform.position) > atkRadius)
        {
            RemoveTarget();
        }

        // If we have a target and we can attack
        if (hasTarget && targetObj != null && timer == 0.0f)
        {
            // Reset timer 
            timer = fireRate;

            // Animate attack
            OnUnitAttack?.Invoke(gameObject, true);

            // Unit attacks target using health component
            float newHP = targetObj.GetComponentInChildren<Health>().GetHealth - attackPower;

            targetObj.GetComponentInChildren<Health>().UpdateHP(-attackPower);

            // We have killed our target
            if (newHP <= 0) RemoveTarget();
        }
    }

	/*
	 *	Function:	SetTarget
	 *	Purpose:	Set a target for attacking
	 *	In:			gObj (GameObject ot be target by unit)
	 */
    public void SetTarget(GameObject gObj)
    {
        targetObj = gObj;
        hasTarget = true;
    }

	/*
	 *	Function:	RemoveTarget
	 *	Purpose:	Remove current target
	 */
    public void RemoveTarget()
    {
        targetObj = null;
        hasTarget = false;
    }
}
