using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    // Private variables
	private UnitData _data;
	private Transform _transform;
	private int _currentHealth;
	public UnitManager _unitManager;

	// Constructor
	public Unit(UnitData data)
	{
		_data = data;
		_currentHealth = data.HP;
		
		GameObject go = GameObject.Instantiate(Resources.Load($"Prefabs/Units/{_data.Code}")) as GameObject;
		_transform = go.transform;
		
		// Set material to match valid state
		_unitManager = go.GetComponent<UnitManager>();
	}

	// Unit attributes
	public string Code { get => _data.Code; }
	public Transform Transform { get => _transform; }
	public Vector3 Position { get => _transform.position; set => _transform.position = value; }
	public int HP { get => _currentHealth; set => _currentHealth = value; }
	public int MaxHP { get => _data.HP; }
	public int Cost { get => _data.Cost; }
	public int MaxSpeed { get => _data.MaxSpeed; }
	public double MaxRotationSpeed { get => _data.MaxRotationSpeed; }
	public int DynamicAttack { get => _data.DynamicAttack; }
	public int StaticAttack { get => _data.StaticAttack; }
	public int Range { get => _data.Range; }

	public int UnitDataIndex
	{
		get
		{
			for (int i = 0; i < Globals.UNIT_DATA.Length; ++i)
			{
				if (Globals.UNIT_DATA[i].Code == _data.Code)
				{
					return i;
				}
			}

			return -1;
		}
	}
}
