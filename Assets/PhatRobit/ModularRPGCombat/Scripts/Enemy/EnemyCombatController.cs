using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(LineRenderer))]
public class EnemyCombatController : MonoBehaviour
{
	public bool aggressive = true;							// If the enemy is aggressive or not.
	public float chaseSpeed = 5f;							// The nav mesh agent's speed when chasing.
	public float chaseWaitTime = 5f;						// The amount of time to wait when the last sighting is reached.

	public Transform weaponMount;							// Where the weapon is mounted for the enemy
	public WeaponType weaponStyle = WeaponType.Melee; 		// What combat style the enemy has
	public float meleeRange = 3;							// The range the enemy has for his melee weapon
	private Transform _playerTransform;						// The player transform
	public float bulletFadeTime = 0.1f;						// Fading time for the bullets
	public float bulletRange = 100;							// The range the bullets can reach
	public Color bulletTrailColor = Color.green;			// Color of the bullets

	public GameObject swordModel;							// What GameObject to use for the melee combat
	public GameObject gunModel;								// What GameObject to use for the ranged combat

	public AudioClip swordSFX;								// What audio file to use for the melee combat
	public AudioClip gunReadySFX;							// What audio to use when the gun is ready to be fired
	public AudioClip gunFireSFX;							// What audio to use when the gun is being fired

	public float fieldOfViewAngle = 160;					// Number of degrees, centred on forward, for the enemy see.
    public float aggroRadius = 10;
	public float aggroChaseRange = 50;

	private GameObject _sword;								// Reference to currently equipped sword object
	private GameObject _gun;								// Reference to currently equipped gun object

	private float _weaponCD = 0;							// Amount of time left before able to attack again

	private bool _attacked = false;							// A check to see if we have attacked after pressing the attack key

	private Transform _t;									// Reference to our transform
	private WeaponData _weaponData;							// Class that holds our weapon information (type, speed, damage)
	private LineRenderer _bulletTrail;						// Reference to the LineRenderer for the bullet trail
	private Animator _animator;								// Reference to the gameobject's animator

	private AnimatorStateInfo _state;						// Animation state information used to check if we are in the attack animation states
	private AnimatorTransitionInfo _transition;				// Animation transition information used to check if we are transitioning to the attack animation states

	private float _range = 0;									// Destination from player where enemy stops running and goes into attack mode instead.

	// Hash IDs for attack animations
	private int _meleeID = 0;
	private int _rangedStartID = 0;
	private int _rangedID = 0;
	private int _rangedEndID = 0;
	private int _rangedTransID = 0;

	private PlayerStatController _playerStatController;		// Reference to the player's stat controller

	private NavMeshAgent _nav;								// Reference to the nav mesh agent.
	private EnemyStatController _enemyStatController;		// The enemy's stat controller
	private float _chaseTimer;								// A timer for the chaseWaitTime.

	private GameObject _player;								// Reference to the player.

    private bool _engaged = false;
	private Vector3 _returnTarget = new Vector3();
	private bool _returning = false;

    public bool Engaged
    {
        get { return _engaged; }
        set { _engaged = value; }
    }

	public bool Returning
	{
		get { return _returning; }
		set { _returning = value; }
	}

	public Vector3 ReturnTarget
	{
		get { return _returnTarget; }
		set { _returnTarget = value; }
	}

	void Awake()
	{
		// Find the player
		_player = GameObject.FindWithTag(Tags.player);

		if(_player)
		{
			// If player is found, set up player references
			_playerTransform = _player.transform;
			_playerStatController = _player.GetComponent<PlayerStatController>();
		}
	}

	void Start()
	{
		_t = transform;
		_bulletTrail = GetComponent<LineRenderer>();
		_animator = GetComponent<Animator>();
		_enemyStatController = GetComponent<EnemyStatController>();
		_nav = GetComponent<NavMeshAgent>();

		_meleeID = Animator.StringToHash("Base Layer.MeleeAttack");
		_rangedStartID = Animator.StringToHash("Base Layer.WeaponRaise");
		_rangedID = Animator.StringToHash("Base Layer.WeaponShoot");
		_rangedEndID = Animator.StringToHash("Base Layer.WeaponLower");
		_rangedTransID = Animator.StringToHash("Base Layer.WeaponRaise -> Base Layer.WeaponShoot");

        // We initialize our weapon data and give it some parameters for our initial weapon
        switch(weaponStyle)
        {
            case WeaponType.Melee:
                EquipWeapon(WeaponType.Melee, 17, 4);
                break;
            case WeaponType.Ranged:
                EquipWeapon(WeaponType.Ranged, 17, 4);
                break;
        }
	}

	void Update()
	{
        if(_enemyStatController.Alive)
        {
            _state = _animator.GetCurrentAnimatorStateInfo(0);
            _transition = _animator.GetAnimatorTransitionInfo(0);

            if(_playerStatController)
            {
                if(_playerStatController.Alive)
                {
                    // If enemy can see the player and is aggressive and player is in range, engage!
                    if(CanSeePlayer() && aggressive && PlayerInAggroRange() && !_engaged && !_returning)
                    {
                        _engaged = true;

						// The last position the enemy was at before engaging the player so we can return to it
						_returnTarget = _t.position;
                    }
                }
                else if(_engaged)
                {
                    // If player is dead enemy doesnt like them anymore
                    _engaged = false;
					_returning = true;
                }
            }

            // While engaged, chase the player!
            if(_engaged)
            {
                Chasing();
            }
			else if(_returning)
			{
				_nav.destination = _returnTarget;
				_nav.stoppingDistance = 0.8f;

				if(Vector3.Distance(_t.position, _returnTarget) <= 1)
				{
					_returning = false;
				}
			}

            // If we are attacking but have not attacked (in code) yet, attack!
            if(Attacking() && !_attacked)
            {
                if(_weaponData.WeaponType == WeaponType.Ranged && Shooting() ||
                _weaponData.WeaponType == WeaponType.Melee)
                {
                    _attacked = true;
                    Attack();
                }
            }

            // Done attacking
            if(!Attacking() && _attacked)
            {
                _attacked = false;
                _animator.SetBool("MeleeAttack", false);
                _animator.SetBool("RangedAttack", false);
            }
        }
        else
        {
            _attacked = false;
            _animator.SetBool("MeleeAttack", false);
            _animator.SetBool("RangedAttack", false);
        }

		// Fade the bullet trail out
		if(_bulletTrail)
		{
			_bulletTrail.material.color = Color.Lerp(_bulletTrail.material.color, Color.clear, Time.deltaTime / bulletFadeTime);
		}
	}

    // A check to see if the player is in the enemies field of view
    public bool CanSeePlayer()
    {
        Vector3 direction = _playerTransform.position - _t.position;

        return Vector3.Angle(direction, _t.forward) <= fieldOfViewAngle / 2f;
    }

    // A check to see if the player is within aggro range
    private bool PlayerInAggroRange()
    {
        return Vector3.Distance(_t.position, _playerTransform.position) <= aggroRadius;
    }

	private void PreAttack()
	{
        Vector3 direction = _playerTransform.position - _t.position;
		float distance = Vector3.Distance(_playerTransform.position, _t.position);

		if(distance <= meleeRange && _weaponCD <= 0)
		{
			if(Vector3.Dot(direction.normalized, _t.forward) > 0)
			{
				_weaponCD = _weaponData.WeaponSpeed;

				if(_weaponData.WeaponType == WeaponType.Melee)
				{
					_animator.SetBool("MeleeAttack", true);
				}
				else if(_weaponData.WeaponType == WeaponType.Ranged)
				{
					_animator.SetBool("RangedAttack", true);

					if(GetComponent<AudioSource>() && gunReadySFX)
					{
						GetComponent<AudioSource>().clip = gunReadySFX;
						GetComponent<AudioSource>().Play();
					}
				}
			}
		}
		else
		{
			_weaponCD -= Time.deltaTime;
		}
	}

	private void Attack()
	{
		// If we're using a melee weapon, do this!
		if(_weaponData.WeaponType == WeaponType.Melee)
		{
			if(GetComponent<AudioSource>() && swordSFX)
			{
				GetComponent<AudioSource>().clip = swordSFX;
				GetComponent<AudioSource>().Play();
			}

			if(_playerStatController)
			{
				_playerStatController.TakeDamage(_weaponData.WeaponDamage);
			}
		}
		// Otherwise if were using a ranged weapon, do this!
		else if(_weaponData.WeaponType == WeaponType.Ranged)
		{
			Ray ray = new Ray(_t.position + new Vector3(0, 1, 0), _t.forward);
			RaycastHit[] hits = Physics.RaycastAll(ray, bulletRange);
			if(_bulletTrail)
			{
				_bulletTrail.material.color = bulletTrailColor;
				_bulletTrail.SetPosition(0, weaponMount.position);
				_bulletTrail.SetPosition(1, _t.position + (ray.direction * bulletRange));
			}

			foreach(RaycastHit hit in hits)
			{
				if(!hit.collider.isTrigger)
				{
					if(_playerStatController)
					{
						_playerStatController.TakeDamage(_weaponData.WeaponDamage);
					}

					if(_bulletTrail)
					{
						_bulletTrail.SetPosition(1, hit.point);
					}
				}
			}

			if(GetComponent<AudioSource>() && gunFireSFX)
			{
				GetComponent<AudioSource>().clip = gunFireSFX;
				GetComponent<AudioSource>().Play();
			}
		}
	}

	private bool Attacking()
	{
		return _state.nameHash == _meleeID ||
			   _state.nameHash == _rangedStartID ||
			   _state.nameHash == _rangedID ||
			   _state.nameHash == _rangedEndID;
	}

	private bool Shooting()
	{
		return _state.nameHash == _rangedID ||
			   _transition.nameHash == _rangedTransID;
	}

	private void EquipWeapon(WeaponType weaponType, int damage, int speed)
	{
		_weaponData = new WeaponData(weaponType, damage, speed);

		if(weaponType == WeaponType.Melee)
		{
            _range = meleeRange;

			if(swordModel)
			{
				if(!_sword)
				{
					_sword = (GameObject)Instantiate(swordModel, Vector3.zero, Quaternion.identity);
					_sword.transform.parent = weaponMount;
					_sword.transform.localPosition = Vector3.zero;
					_sword.transform.localRotation = Quaternion.Euler(180, 0, 0);
				}
			}

			if(_gun)
			{
				Destroy(_gun);
			}
		}
		else if(weaponType == WeaponType.Ranged)
		{
            _range = bulletRange;

			if(gunModel)
			{
				if(!_gun)
				{
					_gun = (GameObject)Instantiate(gunModel, Vector3.zero, Quaternion.identity);
					_gun.transform.parent = weaponMount;
					_gun.transform.localPosition = Vector3.zero;
					_gun.transform.localRotation = Quaternion.Euler(-90, 0, 0);
				}
			}

			if(_sword)
			{
				Destroy(_sword);
			}
		}
	}

	void Chasing()
	{
        // Check to see if enemy is close enough to attack
		if(Vector3.Distance(_t.position, _playerTransform.position) > _range)
		{
            // Set the destination to the player's position
            _nav.destination = _playerTransform.position;

            // Adjust the destination based on the weapon range
            _nav.stoppingDistance = _range;

			// Set the appropriate speed for the NavMeshAgent.
			_nav.speed = chaseSpeed;

			// If near the last personal sighting...
            if(_nav.remainingDistance < _nav.stoppingDistance)
            {
                // If the timer exceeds the wait time...
                if(_chaseTimer >= chaseWaitTime)
                {
                    // ... reset last global sighting, the last personal sighting and the timer.
                    //_personalLastSighting = _resetPosition;
                    _chaseTimer = 0f;
                }
                else
                {
                    // ... increment the timer.
                    _chaseTimer += Time.deltaTime;
                }
            }
            else
            {
                // If not near the last sighting personal sighting of the player, reset the timer.
                _chaseTimer = 0f;
            }
		}
		else
		{
			PreAttack();
		}

		if(Vector3.Distance(_t.position, _returnTarget) > aggroChaseRange)
		{
			_returning = true;
			_engaged = false;
		}
	}
}