using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TerrainGenerator : MonoBehaviour
{
	Mesh _mesh;
	public Transform water;
	public GameObject enemyBrain;

	Vector3[] _vertices;
	Vector2[] _uvs;
	int[] _triangles;

	[Header("Terrain Info")]
	public int boardHalfWidth = 20;
	public int boardHalfHeight = 20;

	public float waterHeightDivisor = 2.0f;
	public float waterHeight;
	float _perlinXSeed;
	float _perlinYSeed;
	[Range(0, 100)]
	public int perlinRange;
	public float[] amplitudes;
	public float[] frequencies;

	[Header("Base Info")]
	float _baseMinRadius = 10.0f;
	float _baseMaxRadius = 20.0f;
	float _plateauOffset = 0.01f;
	Vector3 _playerPlateauCentre;
	float _playerCentreHeight;
	Vector3 _enemyPlateauCentre;
	float _enemyCentreHeight;

	int _maxTreeIndex = 6;
	int _maxRockIndex = 1;
	int _maxRockSize = 10;
	int _maxBushIndex = 10;

	int _BoardWidth { get => boardHalfWidth * 2; }
	int _BoardHeight { get => boardHalfHeight * 2; }

	void Awake()
	{
		_mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = _mesh;
		Debug.Log(_mesh.isReadable);

		waterHeight = water.localPosition.y;
		water.position = new Vector3(-boardHalfWidth, waterHeight, -boardHalfHeight);
		water.localScale = new Vector3(-_BoardWidth/10.0f, -_BoardHeight/10.0f, water.localScale.z);

		_perlinXSeed = Random.Range(0, perlinRange);
		_perlinYSeed = Random.Range(0, perlinRange);

		_playerPlateauCentre = _InitializePlateauCentre();
		_enemyPlateauCentre = -_playerPlateauCentre;
	}

    // Start is called before the first frame update
    void Start()
    {
        _CreateShape();
		_UpdateMesh();
		_PlaceCommandBases(_playerPlateauCentre);
		_PlaceEnemyBase(_enemyPlateauCentre);
		GameManager.Instance.BakeNavMeshes();
    }

	/*
	 *	Function:	_InitializePlateauCentre
	 *	Purpose:	Determine the player plateau centre
	 *	Return:		Vector3 (Position of the player's plateau centre)
	 */
	Vector3 _InitializePlateauCentre()
	{
		Vector3 direction = Vector3.Normalize(new Vector3(Random.value, 0, Random.value));

		return direction * (Mathf.Min(boardHalfWidth, boardHalfHeight) - _baseMaxRadius);
	}

	/*
	 *	Function:	_CalculatePerlinHeight
	 *	Purpose:	Calculate the height of a vertex based upon its x and z coordinates in the world
	 *	In:			x (x coordinate of vertex)
	 *	In:			z (z coordinate of vertex)
	 *	Return:		float (Y position or height of vertex)
	 */
	float _CalculatePerlinHeight(float x, float z)
	{
		float xPos = x + boardHalfWidth;
		float zPos = z + boardHalfHeight;
		float yPos = 0.0f;

		for (int j = 0; j < amplitudes.Length; ++j)
		{
			yPos += Mathf.PerlinNoise((xPos + _perlinXSeed) * frequencies[j], (zPos + _perlinYSeed) * frequencies[j]) * amplitudes[j];
		}
		return yPos;
	}

	/*
	 *	Function:	_CalculatePlateauHeight
	 *	Purpose:	Calculate the height of terrain in a plateau radius based upon the plateau's height, the given height of a vertex and plateau centre height
	 *	In:			distanceFromCentre (Distance of vertex from centre of plateau)
	 *	In:			yPos (actual height of vertex)
	 *	In:			plateauHeight (Height of plateau centre)
	 *	Return:		float (Desired height of the vertex)
	 */
	float _CalculatePlateauHeight(float distanceFromCentre, float yPos, float plateauHeight)
	{
		if (distanceFromCentre >= _baseMinRadius)
		{
			float baseRadiusRatio = Mathf.Pow(((distanceFromCentre-_baseMinRadius)/(_baseMaxRadius-_baseMinRadius)), 2);
			float portionYPos = yPos * baseRadiusRatio;
			plateauHeight = plateauHeight * (1.0f - baseRadiusRatio);

			return plateauHeight + portionYPos;
		}

		return  Mathf.Max(plateauHeight, waterHeight + _plateauOffset);
	}

	/*
	 *	Function:	_ClosestPlateauCentre
	 *	Purpose:	Calculate if the player or enemy plateau is closer to a given vertex
	 *	In:			vertexPos (Vertex to which to compare distances)
	 *	Out:		plateauHeight (Height of plateau to which vertex is closest)
	 *	Return:		float (Minimum distance to either the player's or enemy's plateaus)
	 */
	float _ClosestPlateauCentre(Vector3 vertexPos, out float plateauHeight)
	{
		float distanceFromPlayerCentre = Vector3.Distance(vertexPos, _playerPlateauCentre);
		float distanceFromEnemyCentre = Vector3.Distance(vertexPos, _enemyPlateauCentre);

		if (distanceFromPlayerCentre < distanceFromEnemyCentre)
		{
			plateauHeight = _playerCentreHeight;
			return distanceFromPlayerCentre;
		}

		plateauHeight = _enemyCentreHeight;
		return distanceFromEnemyCentre;
	}

	/*
	 *	Function:	_PlaceEnvironmentalProps
	 *	Purpose:	Randomly place trees around a given map but not in any area with water
	 *	In:			position (Position at which to place a tree)
	 */
	void _PlaceEnvironmentalProps(Vector3 position)
	{
		float randomOutcome = Random.value;

		if (randomOutcome < 0.01)
		{
			int treeType = Random.Range(0, _maxTreeIndex);
			GameObject go = GameObject.Instantiate(Resources.Load($"Prefabs/Terrain/Trees/Tree" + treeType)) as GameObject;
			go.transform.position = position;
			go.transform.parent = gameObject.transform;
		}
	}

	/*
	 *	Function:	_CreateShape
	 *	Purpose:	Create actual terrain mesh
	 */
	void _CreateShape()
	{
		_playerCentreHeight = Mathf.Ceil(Mathf.Max(_CalculatePerlinHeight(_playerPlateauCentre.x, _playerPlateauCentre.z), waterHeight + _plateauOffset));
		_enemyCentreHeight = Mathf.Ceil(Mathf.Max(_CalculatePerlinHeight(_enemyPlateauCentre.x, _enemyPlateauCentre.z), waterHeight + _plateauOffset));

		_vertices = new Vector3[(_BoardWidth + 1) * (_BoardHeight + 1)];
		_uvs = new Vector2[_vertices.Length];

		for (int z = 0; z <= _BoardHeight; ++z)
		{
			for (int x = 0; x <= _BoardWidth; ++x)
			{
				int i = (x + z) + (z * _BoardWidth);
				int xPos = x - boardHalfWidth;
				int zPos = z - boardHalfHeight;
				float yPos = _CalculatePerlinHeight(xPos, zPos);
				
				Vector3 vertexPos = new Vector3(xPos, 0, zPos);
				
				float plateauHeight;
				float distanceFromCentre = _ClosestPlateauCentre(vertexPos, out plateauHeight);

				if (distanceFromCentre < _baseMaxRadius)
				{
					_vertices[i] = new Vector3(xPos, _CalculatePlateauHeight(distanceFromCentre, yPos, plateauHeight), zPos);
				}
				else
				{
					_vertices[i] =  new Vector3(xPos, yPos, zPos);
					
					if (yPos > waterHeight)
					{
						_PlaceEnvironmentalProps(new Vector3(xPos, yPos, zPos));
					}
				}
			}
		}

		    
		
		for (int i = 0; i < _uvs.Length; ++i)
		{
			_uvs[i] = new Vector2(_vertices[i].x, _vertices[i].z);
		}


		_triangles = new int[_BoardWidth * _BoardHeight * 6];

		int vert = 0;
		int tris = 0;

		for (int z = 0; z < _BoardHeight; ++z)
		{
			for (int x = 0; x < _BoardWidth; ++x)
			{
				_triangles[tris + 0] = vert;
				_triangles[tris + 1] = vert + _BoardWidth + 1;
				_triangles[tris + 2] = vert + 1;
				_triangles[tris + 3] = vert + 1;
				_triangles[tris + 4] = vert + _BoardWidth + 1;
				_triangles[tris + 5] = vert + _BoardWidth + 2;

				++vert;
				tris += 6;
			}

			++vert;
		}
	}

	/*
	 *	Function:	_UpdateMesh
	 *	Purpose:	Update a mesh with the calculated vertex, uv and triangle data and attach a mesh collider
	 */
	void _UpdateMesh()
	{
		_mesh.Clear();

		_mesh.vertices = _vertices;
		_mesh.uv = _uvs;
		_mesh.triangles = _triangles;

		MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = _mesh;

		_mesh.RecalculateNormals();
	}

	/*
	 *	Function:	_ConstructCommandBase
	 *	Purpose:	Instantiate and place a command base
	 *	In:			plateauCentre (Centre of plateau on which to place command base)
	 *	In:			centreHeight (Height at centre of command base)
	 *	Return:		Building (Command base instantiated)
	 */
	Building _ConstructCommandBase(Vector3 plateauCentre, float centreHeight)
	{
		Building building = new Building(Globals.BUILDING_DATA[0]);
		building.Transform.GetComponent<BuildingManager>().Initialize(building);

		Vector3 commandBasePosition = new Vector3(plateauCentre.x
													, centreHeight
													, plateauCentre.z);
		building.SetPosition(commandBasePosition);
		building.Place();

		return building;
	}

	/*
	 *	Function:	_PlaceCommandBases
	 *	Purpose:	Instantiate and place a player command base
	 *	In:			plateauCentre (Centre of plateau on which to place command base)
	 */
	void _PlaceCommandBases(Vector3 plateauCentre)
	{
		Building building = _ConstructCommandBase(_playerPlateauCentre, _playerCentreHeight);

		GameManager.Instance.FocusOnUnit(building.Transform.GetComponent<BuildingManager>().gameObject);
	}

	/*
	 *	Function:	_PlaceEnemyBase
	 *	Purpose:	Instantiate and place a enemy command base, deactive enemy aperture, set enemy command base to enemy team
	 *					and place enemy AI at enemy commmand base position
	 *	In:			plateauCentre (Centre of plateau on which to place command base)
	 */
	void _PlaceEnemyBase(Vector3 pateauCentre)
	{
		Building building = _ConstructCommandBase(_enemyPlateauCentre, _enemyCentreHeight);

		building.Transform.GetChild(0).gameObject.SetActive(false);
		building.Transform.GetComponent<Team>().unitTeam = Team.TeamType.Enemy;

		Object.Instantiate(enemyBrain, building.Transform);
	}
}
