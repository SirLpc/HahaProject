using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PlayerStatController : MonoBehaviour
{
	public int health = 100;			// GameObject's current health
	public int maxHealth = 100;			// GameObject's max health
	public int mana = 100;				// GameObject's current mana
	public int maxMana = 100;			// GameObject's max mana
	public int experience = 0;			// Player's experience points
	public int maxExperience = 10;		// Maximum experience
	public int level = 1;				// Player's level
	public float healthTimer = 10;		// How much in between health additions (when it will give "healthAmount" to the player).
	public int healthAmount = 5;		// How much health that will given every "healthTimer" seconds.
	public float manaTimer = 10;		// How much in between mana additions (when it will give "healthAmount" to the player).
	public int manaAmount = 10;		// How much mana that will given every "manaTimer" seconds.

	public AudioClip levelUpSFX;		// Audio for leveling up
	public AudioClip deathSFX;			// Audio for dying

	public Renderer playerRenderer;		// The color on this renderer's material will change when this gameObject is damaged
	public Color hurtColor = Color.red;	// This is the color the playerRenderer will change to when hurt
	public float hurtFadeSpeed = 1;		// The speed at which the hurtColor fades away

	private bool _alive = true;			// Is the player alive or dead?
	private Vector3 _spawnPoint;		// Initial spawn point
	private Color _defaultColor;		// The default color for the gameObject's material

	private Transform _t;				// Reference to this gameobject's transform

	private Animator _animator;			// Reference to the animator component
	private HashIDs _hash;				// Reference to the HashIDs script

	private float _healthTimer = 0;		// How much in between health additions (when it will give "healthAmount" to the player).
	private float _manaTimer = 10;		// How much in between mana additions (when it will give "healthAmount" to the player).

    public bool Alive
    {
        get { return _alive; }
    }

	void Awake()
	{
		GameObject hash = GameObject.FindWithTag(Tags.gameController);

		if(hash)
		{
			_hash = hash.GetComponent<HashIDs>();
		}
	}

	void Start()
	{
		_t = transform;
		_animator = GetComponent<Animator>();

		_spawnPoint = _t.position;

		if(playerRenderer)
		{
			_defaultColor = playerRenderer.material.color;
		}
	}

	void Update()
	{
		if(_alive)
		{
			if(health < maxHealth)
			{
				if(_healthTimer >= healthTimer)
				{
					GiveHealth(healthAmount);
					_healthTimer = 0;
				}
				else
				{
					_healthTimer += Time.deltaTime;
				}
			}

			if(mana < maxMana)
			{
				if(_manaTimer >= manaTimer)
				{
					GiveMana(manaAmount);
					_manaTimer = 0;
				}
				else
				{
					_manaTimer += Time.deltaTime;
				}
			}


			// If health <= 0, player is dead
			if(health <= 0)
			{
				_alive = false;

				if(_hash)
				{
					_animator.SetBool(_hash.Dead, true);
				}

				// Play death SFX if audio source and audioclip exist
				if(GetComponent<AudioSource>() && deathSFX)
				{
					GetComponent<AudioSource>().clip = deathSFX;
					GetComponent<AudioSource>().Play();
				}
			}
		}

		if(playerRenderer)
		{
			// Return player material color back to normal
			playerRenderer.material.color = Color.Lerp(playerRenderer.material.color, _defaultColor, Time.deltaTime * hurtFadeSpeed);
		}
	}

	void OnGUI()
	{
		if(!_alive)
		{
			GUILayout.Window(10, new Rect(Screen.width / 2f - 75, Screen.height / 2f, 150, 32), RespawnWindow, "Died");
		}
	}

	private void RespawnWindow(int id)
	{
		if(GUILayout.Button("Respawn"))
		{
			Respawn();
		}
	}

	private void Respawn()
	{
		_t.position = _spawnPoint;

		if(_hash)
		{
			_alive = true;
			health = maxHealth;
			_animator.SetBool(_hash.Dead, false);
		}
	}

	public void TakeDamage(int amount)
	{
		// Decrement this GameObject's health by amount inflected by the other GameObject.
		health -= amount;
		health = Mathf.Clamp(health, 0, maxHealth);

		if(playerRenderer)
		{
			playerRenderer.material.color = hurtColor;
		}

		Debug.Log(gameObject.tag + " took " + amount + " damage. Current health is: " + health + "/" + maxHealth);
	}

	public void GiveHealth(int amount)
	{
		// Decrement this GameObject's health by amount inflected by the other GameObject.
		health += amount;
		health = Mathf.Clamp(health, 0, maxHealth);
	}

	public void TakeMana(int amount)
	{
		// Decrement this GameObject's mana by amount used by the other GameObject.
		mana -= amount;
		mana = Mathf.Clamp(mana, 0, maxMana);
	}

	public void GiveMana(int amount)
	{
		// Decrement this GameObject's mana by amount used by the other GameObject.
		mana += amount;
		mana = Mathf.Clamp(mana, 0, maxMana);
	}

	public void GiveXp(int amount)
	{
		// Add to experience, when >= maxExperience then level up!
		experience += amount;

		Debug.Log(gameObject.tag + " was awarded " + amount + " experience. Current experience is: " + experience);

		if(experience >= maxExperience)
		{
			experience -= maxExperience;
			LevelUp();
		}
	}

	private void LevelUp()
	{
		// When experience is >= maxExperience, increment level and adjust stats
		level++;
		maxExperience = level * 10;
		maxHealth = level * 10 + 100;
		maxHealth = Mathf.Clamp(maxHealth, 1, 999999);
		health = maxHealth;
		maxMana = level * 10 + 100;
		maxMana = Mathf.Clamp(maxMana, 1, 999999);
		mana = maxMana;

		// If audio source and levelUpSFX exist, play levelUpSFX
		if(GetComponent<AudioSource>() && levelUpSFX)
		{
			GetComponent<AudioSource>().clip = levelUpSFX;
			GetComponent<AudioSource>().Play();
		}
	}
}