using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(LineRenderer))]
public class PlayerCombatController : MonoBehaviour
{
	public Transform weaponMount;				// This is where the character's weapon will appear when equipped
	public float meleeRange = 3;				// Max range of melee attacks
	public float bulletFadeTime = 0.1f;			// Amount of time it takes until the bullet trail is no longer visible
	public float bulletRange = 100;				// Range of your bullets
	public Color bulletTrailColor = Color.cyan;	// The color of the bullet trail
	public int meleeDamage = 17;				// Melee weapon damage
	public float meleeWaitTime = 1;				// Melee wait time (time between attacks)
	public int rangedDamage = 35;				// Ranged weapon damage
	public float rangedWaitTime = 1;				// Ranged wait time (time between attacks)
	public float rangedSpeed = 30;				// How fast the bullet flies
	public float spellSpeed = 5;
	public bool autoAimMelee = false;			// Automatically aim at the closest enemy with melee weapons
	public bool autoAimRanged = false;			// Automatically aim at the closest enemy with ranged weapons
	public bool targetSwitch = false;			// Be able to target switch
	public float autoAimDistance = 50;			// How far away an enemy can be for auto aim to function

	public KeyCode attackKey = KeyCode.Space;	// Attack key
	public KeyCode specialAttackKey = KeyCode.LeftShift;	// Special Attack key
	public KeyCode swordKey = KeyCode.Alpha1;	// Equip sword key
	public KeyCode gunKey = KeyCode.Alpha2;		// Equip gun key
	public KeyCode bowKey = KeyCode.Alpha3;		// Equip bow key
	public KeyCode spellKey = KeyCode.Alpha4;	// Equip spell key
	public KeyCode aoeSpellKey = KeyCode.Alpha5;		// Equip Area Of Effect spell key
	public bool laserSight;						// Laser Sight for gun.
	public Color laserSightColor = Color.red;	// The color of the laser sight
	public bool laserGun;						// If the gun should be a lasergun or not
	public KeyCode targetSwitchKey = KeyCode.Tab;	// Target Switch Key
	public Color enemyInfoBarColor = Color.red;	// Standard Enemy Info Bar color for non-selected enemies
	public Color targetedEnemyColor = Color.blue;	// The color of the info bar of the selected enemy

	public GameObject swordModel;				// Model to be used for the sword
	public GameObject gunModel;					// Model to be used for the gun
	public GameObject bowModel;					// Model to be used for the bow
	public Material swordMaterial;				// Material for the sword
	public Material gunMaterial;				// Material for the gun
	public Material bowMaterial;				// Material for the bow

	public GameObject meleeHitbox;				// The hitbox for melee attacks
    public RangeFixHitbox rangeFixHitbox;           // The hitbox for fix range attacks` aim forward
	public GameObject bullet;					// The bullet prefab used by gun
	public GameObject spell;					// The spell prefab used
	public GameObject aoeSpell;					// The area of effect spell prefab used
	public GameObject arrow;					// The arrow prefab used by bow

	public AudioClip swordSFX;					// The sound that plays when using the sword
	public AudioClip gunReadySFX;				// The sound that plays when using the gun
	public AudioClip gunFireSFX;				// The sound that plays when the gun is fired

	private GameObject _sword;					// Reference to currently equipped sword object
	private GameObject _gun;					// Reference to currently equipped gun object
	private GameObject _bow;					// Reference to currently equipped bow object

	private float _weaponCD = 0;				// Amount of time left before able to attack again

	private bool _attacked = false;				// A check to see if we have attacked after pressing the attack key

	private Transform _t;						// Reference to our transform
	private WeaponData _weaponData;				// Class that holds our weapon information (type, speed, damage)
	private LineRenderer _bulletTrail;			// Reference to the LineRenderer for the bullet trail
	private Animator _animator;					// Reference to the gameobject's animator

	private AnimatorStateInfo _state;			// Animation state information used to check if we are in the attack animation states

	// Hash IDs for attack animations
	private int _meleeID = 0;
	private int _specialID = 0;
	private int _rangedID = 0;

    // Weap switch
    private int _curWeaponIndex = 0;
    private float _lastSwitchWeaponTime = float.MinValue;   //Last switch weapon time, prevent multy switch one swipe
    private float _SwitchWeaponInterval = 1.5f;             //Switch interval between two action


    public Animator Animator
	{
		get { return _animator; }
	}

	public int WeaponDamage
	{
		get { return _weaponData.WeaponDamage; }
		set { _weaponData.WeaponDamage = value; }
	}

	public float WeaponSpeed
	{
		get { return _weaponData.WeaponSpeed; }
		set { _weaponData.WeaponSpeed = value; }
	}

	void Start()
	{
		_t = transform;
		_bulletTrail = GetComponent<LineRenderer>();
		_animator = GetComponent<Animator>();

		_meleeID = Animator.StringToHash("Base Layer.MeleeAttack");
		_specialID = Animator.StringToHash("Base Layer.SpecialAttack");
		_rangedID = Animator.StringToHash("Base Layer.RangedAttack");

		// We initialize our weapon data and give it some parameters for our initial weapon
		EquipWeapon(WeaponType.Melee, meleeDamage, meleeWaitTime);
	}

	void Update()
	{
		_state = _animator.GetCurrentAnimatorStateInfo(0);

		if(laserSight)
		{
			if(_weaponData.WeaponType == WeaponType.Ranged && _weaponData.RangedType == RangedType.Gun)
			{
				Ray ray = new Ray(_t.position + new Vector3(0, 1, 0), _t.forward);
				RaycastHit[] hits = Physics.RaycastAll(ray, bulletRange);

				if(_bulletTrail)
				{
					_bulletTrail.SetPosition(0, weaponMount.position);
					_bulletTrail.SetPosition(1, _t.position + (ray.direction * bulletRange));
				}

				foreach(RaycastHit hit in hits)
				{
					if(!hit.collider.isTrigger)
					{
						if(_bulletTrail)
						{
							_bulletTrail.SetPosition(1, hit.point);
						}
					}
				}
			}
			else
			{
				_bulletTrail.SetPosition(0, Vector3.zero);
				_bulletTrail.SetPosition(1, Vector3.zero);
			}
		}
		else
		{
			_bulletTrail.SetPosition(0, Vector3.zero);
			_bulletTrail.SetPosition(1, Vector3.zero);
		}


		if(_weaponCD <= 0 && !Attacking())
        {
            // When keys are pressed, equip a weapon
            InputEquipWeapon();

            // Attack!
#if UNITY_EDITOR
            if (Input.GetKeyDown(attackKey))
#else
            if (CrossPlatformInputManager.GetButtonDown("Fire1"))
#endif
            {
                _attacked = false;
                _weaponCD = _weaponData.WeaponSpeed;

                AutoAim();

                if (_weaponData.WeaponType == WeaponType.Melee)
                {
                    _animator.SetBool("MeleeAttack", true);
                }
                else if (_weaponData.WeaponType == WeaponType.Ranged)
                {
                    _animator.SetBool("RangedAttack", true);

                    if (GetComponent<AudioSource>() && gunReadySFX)
                    {
                        GetComponent<AudioSource>().clip = gunReadySFX;
                        GetComponent<AudioSource>().Play();
                    }
                }
            }

            // Special Attack!
            if (_weaponData.WeaponType == WeaponType.Melee) // Only allow special attack with melee weapon
            {
                if (Input.GetKeyDown(specialAttackKey))
                {
                    _attacked = false;
                    _weaponCD = _weaponData.WeaponSpeed;
                    _animator.SetBool("SpecialAttack", true);

                    AutoAim();
                }
            }
        }
        else
		{
			_weaponCD -= Time.deltaTime;
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
			_animator.SetBool("SpecialAttack", false);
			_animator.SetBool("RangedAttack", false);
		}

		// Fade the bullet trail out
		if(_bulletTrail)
		{
			_bulletTrail.material.color = Color.Lerp(_bulletTrail.material.color, laserSightColor, Time.deltaTime / bulletFadeTime);
		}

		if(targetSwitch)
		{
			if(Input.GetKeyDown(targetSwitchKey))
			{
				GameObject[] enemies = GameObject.FindGameObjectsWithTag(Tags.enemy);
				Transform target = null;
				
				float closest = Mathf.Infinity;
				
				foreach(GameObject enemy in enemies)
				{
					EnemyStatController enemyStats = enemy.GetComponent<EnemyStatController>();

					EnemyInfoBar _enemyInfoBar;
					_enemyInfoBar = enemy.GetComponent<EnemyInfoBar>();
					_enemyInfoBar.BarColor = enemyInfoBarColor;
					
					if(enemyStats && enemyStats.Alive)
					{
						float distance = Vector3.Distance(_t.position, enemy.transform.position);

						if(distance <= autoAimDistance &&
						   distance < closest)
						{
							closest = distance;
							target = enemy.transform;
						}
					}
				}
				
				if(target)
				{
					Vector3 point = target.position;
					point.y = _t.position.y;
					_t.LookAt(point);
					EnemyInfoBar _enemyInfoBar;
					_enemyInfoBar = target.GetComponent<EnemyInfoBar>();
					_enemyInfoBar.BarColor = targetedEnemyColor;
				}
			}
		}
	}

    private void InputEquipWeapon()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(swordKey))
        {
            EquipWeapon(WeaponType.Melee, meleeDamage, meleeWaitTime);
        }

        if (Input.GetKeyDown(gunKey))
        {
            EquipWeapon(WeaponType.Ranged, rangedDamage, rangedWaitTime, RangedType.Gun);
        }

        if (Input.GetKeyDown(spellKey))
        {
            EquipWeapon(WeaponType.Ranged, rangedDamage, rangedWaitTime, RangedType.Spell);
        }

        if (Input.GetKeyDown(aoeSpellKey))
        {
            EquipWeapon(WeaponType.Ranged, rangedDamage, rangedWaitTime, RangedType.AOESpell);
        }

        if (Input.GetKeyDown(bowKey))
        {
            EquipWeapon(WeaponType.Ranged, rangedDamage, rangedWaitTime, RangedType.Bow);
        }
#else
        var axis = CrossPlatformInputManager.GetAxis("Mouse X");
        if (Time.time - _lastSwitchWeaponTime < _SwitchWeaponInterval)
            return;
        if (axis < -0.2f)
        {
            _curWeaponIndex = _curWeaponIndex == 0 ? 0 : _curWeaponIndex - 1;
            EquipWeapon((RangedType)_curWeaponIndex);
            _lastSwitchWeaponTime = Time.time;
        }
        else if (axis > 0.2f)
        {
            //todo We should use truely weapon count
            _curWeaponIndex = _curWeaponIndex == 4 ? 4 : _curWeaponIndex + 1;
            EquipWeapon((RangedType)_curWeaponIndex);
            _lastSwitchWeaponTime = Time.time;
        }
#endif
    }

    private void AutoAim()
	{
        // If autoaim is enabled, find the nearest or sector enemy within range and rotate to face it
        if (_weaponData.WeaponType == WeaponType.Melee && autoAimMelee ||
		_weaponData.WeaponType == WeaponType.Ranged && autoAimRanged)
        {
            Transform target = null;
            //target = AutoAimByNearest();
            target = rangeFixHitbox.TryGetTargetEnemy();

            if (target)
            {
                Vector3 point = target.position;
                point.y = _t.position.y;
                _t.LookAt(point);
                EnemyInfoBar _enemyInfoBar;
                _enemyInfoBar = target.GetComponent<EnemyInfoBar>();
                _enemyInfoBar.BarColor = targetedEnemyColor;
            }
        }
    }

    private Transform AutoAimByNearest()
    {
        Transform target = null;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(Tags.enemy);

        float closest = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            EnemyStatController enemyStats = enemy.GetComponent<EnemyStatController>();

            EnemyInfoBar _enemyInfoBar;
            _enemyInfoBar = enemy.GetComponent<EnemyInfoBar>();
            _enemyInfoBar.BarColor = enemyInfoBarColor;

            if (enemyStats && enemyStats.Alive)
            {
                float distance = Vector3.Distance(_t.position, enemy.transform.position);

                if (distance <= autoAimDistance &&
                distance < closest)
                {
                    closest = distance;
                    target = enemy.transform;
                }
            }
        }

        return target;
    }

    private void Attack()
	{
		// If we're using a melee weapon, do this!
		if(_weaponData.WeaponType == WeaponType.Melee)
		{
			if(meleeHitbox)
			{
				meleeHitbox.SetActive(true);
			}

			if(GetComponent<AudioSource>() && swordSFX)
			{
				GetComponent<AudioSource>().clip = swordSFX;
				GetComponent<AudioSource>().Play();
			}
		}
		// Otherwise if were using a ranged weapon, do this!
		else if(_weaponData.WeaponType == WeaponType.Ranged)
		{
			if(_weaponData.RangedType == RangedType.Gun)
			{
				if(laserGun)
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
							EnemyStatController health = hit.collider.GetComponent<EnemyStatController>();

							if(health)
							{
								health.TakeDamage(_weaponData.WeaponDamage);
							}

							if(_bulletTrail)
							{
								_bulletTrail.SetPosition(1, hit.point);
							}
						}
					}
				}
				else
				{
					Bullet();
				}
			}

			else if(_weaponData.RangedType == RangedType.Spell)
			{
				Spell();
			}

			else if(_weaponData.RangedType == RangedType.AOESpell)
			{
				AOESpell();
			}

			else if(_weaponData.RangedType == RangedType.Bow)
			{
				Arrow();
			}
			
			if(GetComponent<AudioSource>() && gunFireSFX)
			{
				GetComponent<AudioSource>().clip = gunFireSFX;
				GetComponent<AudioSource>().Play();
			}
		}
	}

    // Here is where the weapons are equipped / unequipped
    // Use default data
    private void EquipWeapon(RangedType rangedType)
    {
        switch (rangedType)
        {
            case RangedType.Gun:
                EquipWeapon(WeaponType.Ranged, rangedDamage, rangedWaitTime, RangedType.Gun);
                break;
            case RangedType.Bow:
                EquipWeapon(WeaponType.Ranged, rangedDamage, rangedWaitTime, RangedType.Bow);
                break;
            case RangedType.Spell:
                EquipWeapon(WeaponType.Ranged, rangedDamage, rangedWaitTime, RangedType.Spell);
                break;
            case RangedType.AOESpell:
                EquipWeapon(WeaponType.Ranged, rangedDamage, rangedWaitTime, RangedType.AOESpell);
                break;
            default:
                EquipWeapon(WeaponType.Melee, meleeDamage, meleeWaitTime);
                break;
        }
    }
    // Use custom data
    public void EquipWeapon(WeaponType weaponType, int weaponDamage, float weaponSpeed, RangedType rangedType = RangedType.None)
	{
		_weaponData = new WeaponData(weaponType, weaponDamage, weaponSpeed, rangedType);

		// Remove the gun
		if(_gun)
		{
			Destroy(_gun);
			_gun = null;
		}

		// Remove the bow
		if(_bow)
		{
			Destroy(_bow);
			_bow = null;
		}

		// Remove the sword
		if(_sword)
		{
			Destroy(_sword);
			_sword = null;
		}

		if(weaponType == WeaponType.Melee)
		{
			if(swordModel)
			{
				if(!_sword)
				{
					// Instantiate the sword model and place it on our weapon mount
					_sword = (GameObject)Instantiate(swordModel, Vector3.zero, Quaternion.identity);
					_sword.transform.parent = weaponMount;
					_sword.transform.localPosition = Vector3.zero;
					_sword.transform.localRotation = Quaternion.Euler(180, 0, 0);

					if(swordMaterial)
					{
						_sword.GetComponent<Renderer>().material = swordMaterial;
					}
				}
			}
		}
		else if(weaponType == WeaponType.Ranged)
		{
			if(rangedType == RangedType.Gun)
			{
				if(gunModel)
				{
					if(!_gun)
					{
						// Instantiate the gun model and place it on our weapon mount
						_gun = (GameObject)Instantiate(gunModel, Vector3.zero, Quaternion.identity);
						_gun.transform.parent = weaponMount;
						_gun.transform.localPosition = Vector3.zero;
						_gun.transform.localRotation = Quaternion.Euler(-90, 0, 0);

						if(gunMaterial)
						{
							_gun.GetComponent<Renderer>().material = gunMaterial;
						}
					}
				}
			}
			else if(rangedType == RangedType.Bow)
			{
				if(bowModel)
				{
					if(!_bow)
					{
						// Instantiate the bow model and place it on our weapon mount
						_bow = (GameObject)Instantiate(bowModel, Vector3.zero, Quaternion.identity);
						_bow.transform.parent = weaponMount;
						_bow.transform.localPosition = Vector3.zero;
						_bow.transform.localRotation = Quaternion.identity;

						if(bowMaterial)
						{
							_bow.GetComponent<Renderer>().material = bowMaterial;
						}
					}
				}
			}
		}
	}

	// A check to see if we are in the attack animation states / transitions
	private bool Attacking()
	{
		return _state.nameHash == _meleeID ||
			   _state.nameHash == _specialID ||
			   _state.nameHash == _rangedID;
	}

	// A check to see if we are in the shooting animation state / transition
	private bool Shooting()
	{
		return _state.nameHash == _rangedID;
	}

	public void Bullet()
	{
		if(bullet)
		{
			GameObject go = (GameObject)Instantiate(bullet, weaponMount.position, _t.rotation);

			Bullet goBullet = go.GetComponent<Bullet>();

			if(goBullet)
			{
				goBullet.CombatController = this;
			}
		}
	}

	public void Spell()
	{
		if(spell)
		{
			GameObject go = (GameObject)Instantiate(spell, weaponMount.position, _t.rotation);
			
			Spell goSpell = go.GetComponent<Spell>();
			
			if(goSpell)
			{
				goSpell.CombatController = this;
			}
		}
	}

	public void AOESpell()
	{
		if(aoeSpell)
		{
			GameObject go = (GameObject)Instantiate(aoeSpell, weaponMount.position, _t.rotation);
			
			AOESpell goAOESpell = go.GetComponent<AOESpell>();
			
			if(goAOESpell)
			{
				goAOESpell.CombatController = this;
			}
		}
	}

	public void Arrow()
	{
		if(arrow)
		{
			GameObject go = (GameObject)Instantiate(arrow, weaponMount.position, _t.rotation);

			Arrow goArrow = go.GetComponent<Arrow>();

			if(goArrow)
			{
				goArrow.CombatController = this;
			}
		}
	}
}