using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : EventManager
{
    // EVENT
	public static event UnitDestroyedHandler OnUnitDestroyed;

    [Header("Health & Armor Stats")]
    public float maxHP = 100.0f;
    public float armour;

    // Used to influence the bar visually
    public MeshRenderer barMesh;

    // Our current HP
    public float currentHP;

    // Checks for animations to allow death state
    public bool isAnimated = false;
    public bool isBase = false;

	public float GetHealth { get => currentHP; }

    // Start is called before the first frame update
    void Start()
    {
        // Set Current HP
        currentHP = maxHP;
        
        // Let listeners know the unit was created
		GameManager.Instance.Health_OnUnitCreated(transform.parent.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // Scale the health bar
        transform.localScale = new Vector3(currentHP / maxHP, transform.localScale.y, transform.localScale.z);

        // Set the color of the health bar if enemy
        if (GetComponentInParent<Team>().unitTeam == Team.TeamType.Enemy) barMesh.material.SetColor("_Color", 
            new Vector4(0.93f, 0.32f, 0.325f, 1));
    }

    void OnDestroy()
    {
        // Prevent some logic from preventing cleanup at game end
        if (!gameObject.scene.isLoaded) return;

        if (GameManager.Instance != null) GameManager.Instance.Health_OnUnitDestroyed(transform.parent.gameObject);
    }

	/*
	 *	Function:	UpdateHP
	 *	Purpose:	Update a units hit points (HP)
	 *	In:			amount (Amount by which to change a units HP)
	 */
    public void UpdateHP(float amount)
    {
        // Set the hp amount
        currentHP += amount;

        // Kill the unit if it is out of HP
        if (currentHP <= 0f)
        {
            // Hides bar, otherwise you get a weird black square
            barMesh.enabled = false;
            DestroyThis();
        }

        // Scale the health bar
        transform.localScale = new Vector3(currentHP / maxHP, transform.localScale.y, transform.localScale.z);
    }

	/*
	 *	Function:	DestroyThis
	 *	Purpose:	Destroy this unit and broadcast its destruction when unit's HP reaches zero or less
	 */
    void DestroyThis()
    {
        if (currentHP <= 0)
		{
			GameObject self = transform.parent.gameObject;
			
			if (self.GetComponent<Team>().unitTeam == Team.TeamType.Enemy)
			{
				OnUnitDestroyed?.Invoke(self);
			}

			Destroy(self);
		}
    }
}
