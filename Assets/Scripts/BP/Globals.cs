using UnityEngine;

public class Globals
{
	// Building tints
	public static Color[] MATERIAL_TINTS = new Color[]
	{
		new Color(0.0f, 0.0f, 0.0f, 0.0f),	// Placed (no tint)
		new Color(0.0f, 1.0f, 0.0f, 0.0f),	// Valid (green)
		new Color(1.0f, 0.0f, 0.0f, 0.0f)	// Invalid (red)
	};

	// Terrain layer index
	public static int TERRAIN_LAYER_MASK = 1 << 3;
	
	// Data for buildings available to build
	public static BuildingData[] BUILDING_DATA = new BuildingData[]
	{
		// Code, Health, Cost, Coverage
		new BuildingData("Command Base", 100, 10000, 10),
		new BuildingData("Barracks", 100, 1000, 3),
		new BuildingData("Factory", 100, 1500, 3),
		new BuildingData("Wall", 100, 10, 3),
	};

	// Data for units available to train
	public static UnitData[] UNIT_DATA = new UnitData[]
	{
		/**** BARRACKS UNITS ****/
		// Code, Health, Cost, Max Speed, Max Rotation Speed, Dynamic Attack, Static Attack, Range
		new UnitData("Goblin"	, 100	, 30	, 3	, 120	, 10	, 3		, 1),

		/**** FACTORY UNITS ****/
		// Code, Health, Cost, Max Speed, Max Rotation Speed, Dynamic Attack, Static Attack, Range
		new UnitData("Tank"		, 100	, 100	, 8	, 150	, 5		, 10	, 10)
	};
}
