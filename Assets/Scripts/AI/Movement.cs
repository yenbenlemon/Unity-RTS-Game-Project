using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{

	public float lastDistance;
	NavMeshAgent _agent;
	int _nextCorner;
	AIManager _movementManager;

	[Header("Movement Info")]
	bool _seeking = false;
	Vector3 _wanderTarget;

	public Vector3 CurrentPosition { get => transform.position; }
	public NavMeshAgent Agent { get => _agent; }
	public float CloseEnough { get => _agent.stoppingDistance; }
	public float MaxSpeed { get => _agent.speed; }
	public float CurrentSpeed { get => _agent.velocity.magnitude; }
	public Vector3 CurrentVelocity { get => _agent.velocity; set => _agent.velocity = value; }
	public float MaxAngularSpeed { get => _agent.angularSpeed; }
	public Vector3 Location { get => transform.position; }

	void Awake()
	{
		_agent = gameObject.GetComponent<NavMeshAgent>();
		_movementManager = GameObject.Find("AI Manager").GetComponent<AIManager>();
	}

	// Start is called before the first frame update
	void Start()
	{
		_agent.speed = (float)gameObject.GetComponent<UnitManager>().Unit.MaxSpeed;
		_agent.angularSpeed = (float)gameObject.GetComponent<UnitManager>().Unit.MaxRotationSpeed;

		_wanderTarget = Vector3.zero;
		_agent.stoppingDistance = 1.0f;
	}

	/*
	 *	Function:	Seek
	 *	Purpose:	Give agent ability to seek a destination position
	 *	In:			targetLocation (Location for agent to seek)
	 */
	public void Seek(Vector3 targetLocation)
	{
		_seeking = true;
		_agent.isStopped = false;
		_agent.SetDestination(targetLocation);
		lastDistance = 100.0f;
	}

	/*
	 *	Function:	Wander
	 *	Purpose:	Give agent ability to wander without any specific location from user
	 */
	public void Wander()
	{
		Debug.Log("Wandering");
		float wanderRadius = 10;
		float wanderDistance = 10;
		float wanderJitter = 1;

		_wanderTarget += new Vector3(Random.Range(-1.0f, 1.0f) * wanderJitter
										, 0
										, Random.Range(-1.0f, 1.0f) * wanderJitter);
		_wanderTarget.Normalize();
		_wanderTarget *= wanderRadius;

		Vector3 localTarget = _wanderTarget + (transform.forward * wanderDistance);
		Vector3 worldTarget = transform.InverseTransformVector(localTarget);

		Seek(localTarget);
	}

	void Update()
	{
		// if (!_seeking)
		// {
		// 	Wander();
		// }
		
		if (!_agent.pathPending)
		{
			if (_agent.remainingDistance <= _agent.stoppingDistance)
			{
				if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
				{
					_agent.velocity = Vector3.zero;
					_agent.isStopped = true;
					// _seeking = false;
				}
			}

			if (Mathf.Abs(lastDistance - _agent.remainingDistance) < 0.0000000001)
			{
				_agent.SetDestination(transform.position);
			}
			else
			{
				lastDistance = _agent.remainingDistance;
			}
		}
	}
}
