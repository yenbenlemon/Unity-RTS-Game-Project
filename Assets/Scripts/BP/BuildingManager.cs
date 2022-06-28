using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BuildingManager : EventManager
{
	private BoxCollider _collider;

	private Building _building = null;
	private int _nCollisions = 0;

	public Building Building { get => _building; }

	/*
	 *	Function:	Initialize
	 *	Purpose:	Initialize building blueprint
	 *	in:			building (building to initialize)
	 */
	public void Initialize(Building building)
	{
		_collider = GetComponent<BoxCollider>();
		_building = building;
	}

	/*
	 *	Function:	OnTriggerEnter
	 *	Purpose:	increase collision count when buildings collides with other building
	 *	in:			other (other building with which collision is happening)
	 */
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Terrain") return;
		_nCollisions++;
		CheckPlacement();
	}

	/*
	 *	Function:	OnTriggerExit
	 *	Purpose:	Decrease collision count when building ceases to collide with other building
	 *	in:			other (other building with which collision happened)
	 */
	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Terrain") return;

		_nCollisions--;
		CheckPlacement();	
	}

	/*
	 *	Function:	CheckPlacement
	 *	Purpose:	Check if building blueprint has a valid placement (i.e. is not colliding with another building)
	 *	return:		validPlacement (true if no collision, false otherwise)
	 */
	public bool CheckPlacement()
	{
		if (_building == null) return false;
		if (_building.IsFixed) return false;

		bool validPlacement = HasValidPlacement();

		_building.Placement = BuildingState.VALID;

		if (!validPlacement)
		{
			_building.Placement = BuildingState.INVALID;
		}

		_building.SetMaterialTint(_building.Placement);

		return validPlacement;
	}

	/*
	 *	Function:	_BuildingIsStable
	 *	Purpose:	Check that at least 3/4 corners are on terrain
	 *	return:		true if at least 3/4 corners are on terrain, false otherwise
	 */
	private bool _BuildingIsStable()
	{
		Vector3 position = transform.position;
		Vector3 centre = _collider.center;
		Vector3 edge = _collider.size/2f;
		float bottomHeight = centre.y - edge.y + 0.5f;

		Vector3[] bottomCorners = new Vector3[]
		{
			new Vector3(centre.x - edge.x, bottomHeight, centre.z - edge.z),
			new Vector3(centre.x - edge.x, bottomHeight, centre.z + edge.z),
			new Vector3(centre.x + edge.x, bottomHeight, centre.z - edge.z),
			new Vector3(centre.x + edge.x, bottomHeight, centre.z + edge.z)
		};

		int invalidCornersCount = 0;

		foreach (Vector3 corner in bottomCorners)
		{
			if (!Physics.Raycast(position + corner, -Vector3.up, 2f, Globals.TERRAIN_LAYER_MASK))
			{
				++invalidCornersCount;
			}
		}

		return invalidCornersCount < 3;
	}

	/*
	 *	Function:	HasValidPlacement
	 *	Purpose:	Check if collisions is 0 and at least 3/4 corners of building are on terrain
	 *	return:		true if collisions is 0 and at least 3/4 corners of building are on terrain, false otherwise
	 */
	public bool HasValidPlacement()
	{
		// Debug.Log("#Col: " + _nCollisions);
		if (_nCollisions > 0) return false;

		return _BuildingIsStable();
	}
}
