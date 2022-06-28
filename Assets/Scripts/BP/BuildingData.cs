using System.Collections.Generic;

public class BuildingData
{
	private string _code;
	private int _healthPoints;
	private int _cost;
	private int _coverage;

	// Constructor
	public BuildingData(string code
						, int healthPoints
						, int cost
						, int coverage)
	{
		_code = code;
		_healthPoints = healthPoints;
		_cost = cost;
		_coverage = coverage;
	}

	public string Code { get => _code; }
	public int HP { get => _healthPoints; }
	public int Cost { get => _cost; }
	public int Coverage { get => _coverage; }
}
