using UnityEngine;
using System.Collections;

public enum EnemyLogicStyle
{
	Idle,
	FreeRoam,
	Patrol
}

[RequireComponent(typeof(NavMesh))]
public class EnemyMovementController : MonoBehaviour
{
	public EnemyLogicStyle logicStyle = EnemyLogicStyle.Idle;
	public bool patrolBacktrack = false;		// Should the enemy backtrack his patrol path once completed or not?
	public float speed = 4f;					// What's the speed of the enemy?
    public float deadZone = 5f;					// The number of degrees for which the rotation isn't controlled by Mecanim.

	public float waitTime = 1f;					// How long does the enemy wait once reaching his target?
	public Transform[] patrolWayPoints;			// What are the enemy's patrol points?

	public float roamRadius = 5f;				// If Free Roaming, how far away from his origin position can he move?

    public float speedDampTime = 0.1f;			// Damping time for the Speed parameter.
    public float angularSpeedDampTime = 0.7f;	// Damping time for the AngularSpeed parameter
    public float angleResponseTime = 0.6f;		// Response time for turning an angle into angularSpeed.

	private NavMeshAgent _nav;					// Reference to the NavMeshAgent
	private float _waitTimer;					// A wait timer used to see when it's time to move again
	private bool _freeRoamDone = true;			// A bool used to set if enemy has reached his free roam target.
	private Vector3 _startPosition;				// This is the start position of the enemy
	private int _currentWaypoint = 0;			// Which waypoint is the enemy at right now?
	private bool _backingUp = false;			// Is the enemy currently backtracking?

	private Transform _t;						// Reference to the transform

    private Transform _player;					// Reference to the player's transform.
    private EnemyCombatController _enemyCombatController;				// Reference to the Enemy Combat Controller script.
    private Animator _anim;						// Reference to the Animator.
    private HashIDs _hash;						// Reference to the HashIDs script.

	void Awake()
	{
		GameObject player = GameObject.FindWithTag(Tags.player);

		if(player)
		{
			_player = player.transform;
		}

		// Get HashIDs used for Mecanim.
		GameObject hash = GameObject.FindWithTag(Tags.gameController);

		if(hash)
		{
			_hash = hash.GetComponent<HashIDs>();
		}
	}

	void Start()
	{
		// Setting up the references.
		_t = transform;
		_nav = GetComponent<NavMeshAgent>();
		_anim = GetComponent<Animator>();
		_enemyCombatController = GetComponent<EnemyCombatController>();

		_startPosition = _t.position;

        // Making sure the rotation is controlled by Mecanim.
        _nav.updateRotation = false;

        // We need to convert the angle for the deadzone from degrees to radians.
        deadZone *= Mathf.Deg2Rad;
	}


	void Update()
	{
        if(!_enemyCombatController.Engaged && !_enemyCombatController.Returning)
        {
            switch(logicStyle)
            {
                case EnemyLogicStyle.Idle:
                    break;
                case EnemyLogicStyle.FreeRoam:
                    FreeRoam();
                    break;
                case EnemyLogicStyle.Patrol:
                    Patrol();
                    break;
            }
        }

        // Calculate the parameters that need to be passed to the animator component.
        NavAnimSetup();
	}

	private void Patrol()
	{
		if(patrolWayPoints.Length > 0)
		{
			// Set an appropriate speed for the NavMeshAgent.
			_nav.speed = speed;

			// If near the next waypoint or there is no destination...
			if(_nav.remainingDistance <= _nav.stoppingDistance)
			{
				// ... increment the timer.
				_waitTimer += Time.deltaTime;

				// If the timer exceeds the wait time...
				if(_waitTimer >= waitTime)
				{
					// Reset the timer.
					_waitTimer = 0;

					// Increment / Decrement current waypoint depending on patrol direction
					if(_backingUp)
					{
						_currentWaypoint--;
					}
					else
					{
						_currentWaypoint++;
					}

					// Done backtracking
					if(_currentWaypoint < 0)
					{
						_backingUp = false;
						_currentWaypoint = 1;
					}

					// Restart at first waypoint, or start backtracking
					if(_currentWaypoint >= patrolWayPoints.Length)
					{
						if(patrolBacktrack)
						{
							_backingUp = true;
							_currentWaypoint -= 2;
						}
						else
						{
							_currentWaypoint = 0;
						}
					}
				}
			}
			else
			{
				// If not near a destination, reset the timer.
				_waitTimer = 0;
			}

			// Set the destination to the patrolWayPoint.
			_nav.destination = patrolWayPoints[_currentWaypoint].position;
		}
	}

	private void FreeRoam()
	{
		_nav.speed = speed;
		if(_freeRoamDone == true && (_waitTimer >= waitTime))
		{	// Move the enemy towards the next random position within the radius range you have specified.
			Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
			randomDirection += _startPosition;
			NavMeshHit hit;
			NavMesh.SamplePosition(randomDirection, out hit, roamRadius, 1);
			Vector3 finalPosition = hit.position;
			_nav.destination = finalPosition;
			_freeRoamDone = false;
			_waitTimer = 0;
		}
		else
		{	// This is what's used to calculate if the enemy has reached his destination point. If he has - move again once the waiting time has been reached.
			float dist = _nav.remainingDistance;
			if(dist != Mathf.Infinity && _nav.pathStatus == NavMeshPathStatus.PathComplete && _nav.remainingDistance < 1f)
			{
				_waitTimer += Time.deltaTime;
				_freeRoamDone = true;
			}
		}
	}

    private void OnAnimatorMove()
    {
        // Set the NavMeshAgent's velocity to the change in position since the last frame, by the time it took for the last frame.
        _nav.velocity = _anim.deltaPosition / Time.deltaTime;

        // The gameobject's rotation is driven by the animation's rotation.
        _t.rotation = _anim.rootRotation;
    }

    private void NavAnimSetup()
    {
        // Create the parameters to pass to the helper function.
        float speed;
        float angle;

        // If the player is not in sight...
        if(_enemyCombatController.Engaged && !_enemyCombatController.CanSeePlayer())
        {
            // ... the enemy should stop...
            speed = 0f;

            // ... and the angle to turn through is towards the player.
			angle = FindAngle(_t.forward, _player.position - _t.position, _t.up);
        }
        else
        {
            // Otherwise the speed is a projection of desired velocity on to the forward vector...
			speed = Vector3.Project(_nav.desiredVelocity, _t.forward).magnitude;

            // ... and the angle is the angle between forward and the desired velocity.
			angle = FindAngle(_t.forward, _nav.desiredVelocity, _t.up);

            // If the angle is within the deadZone...
            if(Mathf.Abs(angle) < deadZone)
            {
                // ... set the direction to be along the desired direction and set the angle to be zero.
				_t.LookAt(_t.position + _nav.desiredVelocity);
                angle = 0f;
            }
        }

        // Call the Setup function of the helper class with the given parameters.
        Setup(speed, angle);
    }

    private float FindAngle(Vector3 fromVector, Vector3 toVector, Vector3 upVector)
    {
        // If the vector the angle is being calculated to is 0...
        if(toVector == Vector3.zero)
            // ... the angle between them is 0.
            return 0f;

        // Create a float to store the angle between the facing of the enemy and the direction it's travelling.
        float angle = Vector3.Angle(fromVector, toVector);

        // Find the cross product of the two vectors (this will point up if the velocity is to the right of forward).
        Vector3 normal = Vector3.Cross(fromVector, toVector);

        // The dot product of the normal with the upVector will be positive if they point in the same direction.
        angle *= Mathf.Sign(Vector3.Dot(normal, upVector));

        // We need to convert the angle we've found from degrees to radians.
        angle *= Mathf.Deg2Rad;

        return angle;
    }

    private void Setup(float speed, float angle)
    {
        // Angular speed is the number of degrees per second.
        float angularSpeed = angle / angleResponseTime;

        // Set the mecanim parameters and apply the appropriate damping to them.
        _anim.SetFloat(_hash.Speed, speed, speedDampTime, Time.deltaTime);
        _anim.SetFloat(_hash.AngleSpeed, angularSpeed, angularSpeedDampTime, Time.deltaTime);
    }
}