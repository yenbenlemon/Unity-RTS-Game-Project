using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitData
{
	string _code;
	int _healthPoints;
	int _cost;
	int _maxSpeed;
	double _maxRotationSpeed;
	int _dynamicAttack;
	int _staticAttack;
	int _range;

	// Constructor
	public UnitData(string code
					, int healthPoints
					, int cost
					, int maxSpeed
					, double maxRotationSpeed
					, int dynamicAttack
					, int staticAttack
					, int range)
	{
		_code = code;
		_healthPoints = healthPoints;
		_cost = cost;
		_maxSpeed = maxSpeed;
		_maxRotationSpeed = maxRotationSpeed;
		_dynamicAttack = dynamicAttack;
		_staticAttack = staticAttack;
		_range = range;
	}

	// Getters
	public string Code { get => _code; }
	public int HP { get => _healthPoints; }
	public int Cost { get => _cost; }
	public int MaxSpeed { get => _maxSpeed; }
	public double MaxRotationSpeed { get => _maxRotationSpeed; }
	public int DynamicAttack { get => _dynamicAttack; }
	public int StaticAttack { get => _staticAttack; }
	public int Range { get => _range; }
}
