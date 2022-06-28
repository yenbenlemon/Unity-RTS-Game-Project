using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // Delegates fopr events
    public delegate void ResourceAwardHandler(int amount);
    public delegate void GoblinAwardHandler();
    public delegate void TankAwardHandler();

    // Events
    public static event GoblinAwardHandler OnGoblinAwarded;
    public static event TankAwardHandler OnTankAwarded;

    // ENUM used to dictate item type
    public enum ItemType
    {
        Goblin,
        Tank,
        Resource
    };

    [Header("Type Selection")]
    public ItemType itemType = ItemType.Goblin;

    // How many resoruces we get
    public int rAwardAmount = 500;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.GetComponent<Team>() != null)
        {
            if(collision.transform.GetComponent<Team>().unitTeam == Team.TeamType.Player)
            {
                // We have collided with the item, and must activate it
                ActivateItem();
            }
        }
    }

	/*
	 *	Function:	ActivateItem
	 *	Purpose:	Spawn unit or award resources to player and destroy item drop game object
	 */
    void ActivateItem()
    {
        switch(itemType)
        {
            case ItemType.Goblin:
                GameManager.Instance.SpawnUnit(transform.position, 0);
                break;
            case ItemType.Tank:
                GameManager.Instance.SpawnUnit(transform.position, 1);
                break;
            case ItemType.Resource:
				GameManager.Instance.UpdateResources(rAwardAmount);
                break;
            default:
                break;
        }

        Destroy(transform.gameObject);
    }
}
