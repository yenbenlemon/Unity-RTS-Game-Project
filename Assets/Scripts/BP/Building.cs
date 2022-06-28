using System.Collections.Generic;
using UnityEngine;

public enum BuildingState
{
	VALID,
	INVALID,
	FIXED
};

public class Building
{
	public BuildingManager _buildingManager;

	// Private variables
	BuildingData _data;
	Transform _transform;
	int _currentHealth;
	BuildingState _placement;
	Material _material;
	Color _originalMaterialColor;

	GameObject _apertureMask;

	// Constructor
	public Building(BuildingData data)
	{
		_data = data;
		_currentHealth = data.HP;
		
		GameObject go = GameObject.Instantiate(Resources.Load($"Prefabs/Buildings/{_data.Code}")) as GameObject;
		_transform = go.transform;

		_material = _transform.Find("Mesh").GetComponent<Renderer>().material;
		_originalMaterialColor = _material.color;

		_apertureMask = _transform.Find("Aperture Mask").gameObject;
		_apertureMask.SetActive(false);
		
		// Set material to match valid state
		_buildingManager = go.GetComponent<BuildingManager>();
		_placement = BuildingState.VALID;
		SetMaterialTint();
	}

	// Building attributes
	public string Code { get => _data.Code; }
	public Transform Transform { get => _transform; }
	public int HP { get => _currentHealth; set => _currentHealth = value; }
	public int MaxHP { get => _data.HP; }

	public int Cost { get => _data.Cost; }
	public int Coverage { get => _data.Coverage; }

	public bool HasValidPlacement { get => _placement == BuildingState.VALID; }
	public bool IsFixed { get => _placement == BuildingState.FIXED; }

	public BuildingState Placement { get => _placement; set => _placement = value; }

	public int BuildingDataIndex
	{
		get
		{
			for (int i = 0; i < Globals.BUILDING_DATA.Length; ++i)
			{
				if (Globals.BUILDING_DATA[i].Code == _data.Code)
				{
					return i;
				}
			}

			return -1;
		}
	}

	/*
	 *	Function:	SetMaterialTint
	 *	Purpose:	Main SetMaterialTint function helper
	 */
	public void SetMaterialTint() { SetMaterialTint(_placement); }

	/*
	 *	Functions:	SetMaterialTint
	 *	Purpose:	Set tint of material according to building state
	 *	in:			placement (Building state with respect to placement)
	 */
	public void SetMaterialTint(BuildingState placement)
	{
		Color materialColor = Globals.MATERIAL_TINTS[0];

		if (placement == BuildingState.VALID)
		{
			materialColor = (_originalMaterialColor + Globals.MATERIAL_TINTS[1]) * 0.5f;
		}
		else if (placement == BuildingState.INVALID)
		{
			materialColor = (_originalMaterialColor + Globals.MATERIAL_TINTS[2]) * 0.5f;
		}
		else if (placement == BuildingState.FIXED)
		{
			materialColor = _originalMaterialColor;
		}
		else
		{
			return;
		}

		_material.color = materialColor;
	}

	/*
	 *	Function:	SetPosition
	 *	Purpose:	Set building position
	 *	in:			position (position to which to set building)
	 */
	public void SetPosition(Vector3 position)
	{
		_transform.position = position;
	}

	/*
	 *	Function:	Place
	 *	Purpose:	Place the building (i.e. change building state to fixed, set material tint to original colour and remove trigger collider)
	 */
	public void Place()
	{
		// set placement state
		_placement = BuildingState.FIXED;
		_apertureMask.SetActive(true);

		// Change to Building materials
		SetMaterialTint();

		// remove "is trigger" flag to allow collisions
		_transform.GetComponent<BoxCollider>().isTrigger = false;
	}

	/*
	 *	Function:	CheckValidPlacement
	 *	Purpose:	Check if the building blueprint has a valid placement
	 */
	public void CheckValidPlacement()
	{
		if (_placement == BuildingState.FIXED) return;

		_placement = _buildingManager.CheckPlacement() ? BuildingState.VALID : BuildingState.INVALID;
		Debug.Log(_placement);
	}
}
