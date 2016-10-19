using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyCombatController))]
public class EnemyStatController : StateBase
{
	public bool giveExperiencePoints;						// Wether or not this gameObject gives experience to the player
	public int experiencePoints = 10;						// Amount of experience to give to the player
	public AudioClip deathSFX;								// Death SFX that plays when gameObject dies
	public Renderer enemyRenderer;							// The color on this renderer's material will change when this gameObject is damaged
	public Color hurtColor = Color.red;						// This is the color the enemyMaterial will change to when hurt
	public float hurtFadeSpeed = 1;							// The speed at which the hurtColor fades away


	private Animator _animator;								// Reference to the animator component
	private HashIDs _hash;									// Reference to the HashIDs script.
	private PlayerStatController _playerStatController;		// Reference to player's stats
	private Color _defaultColor;							// The default color for the gameObject's material

    private EnemyCombatController _combatController;

	void Awake()
	{
		GameObject hash = GameObject.FindWithTag(Tags.gameController);

		if(hash)
		{
			_hash = hash.GetComponent<HashIDs>();
		}

		GameObject player = GameObject.FindWithTag(Tags.player);

		if(player)
		{
			_playerStatController = player.GetComponent<PlayerStatController>();
		}
	}

	void Start()
	{
		_animator = GetComponent<Animator>();
        _combatController = GetComponent<EnemyCombatController>();

		if(enemyRenderer)
		{
			_defaultColor = enemyRenderer.material.color;
		}
	}

	void Update()
	{
		maxHealth = Mathf.Clamp(maxHealth, 1, 999999);

		if(_alive)
		{
			// If not engaged, bring health back up to max
			if(!_combatController.Engaged && health < maxHealth)
			{
				health += 1;
			}

			// When health is at or below 0, gameObject is dead
			if(health <= 0)
			{
				_alive = false;

				// Give Xp to player
				if(_playerStatController)
				{
					_playerStatController.GiveXp(experiencePoints);
				}

				if(_hash)
				{
					_animator.SetBool(_hash.Dead, true);
				}

				// If audio source and deathSFX exist, play deathSFX
				if(GetComponent<AudioSource>() && deathSFX)
				{
					GetComponent<AudioSource>().clip = deathSFX;
					GetComponent<AudioSource>().Play();
				}

				// Destory gameObject in 3 seconds
				Destroy(gameObject, 3);
			}
		}

		if(enemyRenderer)
		{
			// Return enemy material color back to normal
			enemyRenderer.material.color = Color.Lerp(enemyRenderer.material.color, _defaultColor, Time.deltaTime * hurtFadeSpeed);
		}
	}

	public void TakeDamage(int amount)
	{
		// Decrement this GameObject's health by amount inflected by the other GameObject.
		if(!_combatController.Returning)
		{
			health -= amount;
			health = Mathf.Clamp(health, 0, maxHealth);

			if(!_combatController.Engaged)
			{
				_combatController.Engaged = true;
				_combatController.ReturnTarget = transform.position;
			}

			if(enemyRenderer)
			{
				enemyRenderer.material.color = hurtColor;
			}

			Debug.Log(gameObject.tag + " took " + amount + " damage. Current health is: " + health + "/" + maxHealth);
		}
		else
		{
			Debug.Log(gameObject.tag + " is returning to position before engaging and is immortal.");
		}
	}
}