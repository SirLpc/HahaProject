using UnityEngine;
using System.Collections;

public class Spell : MonoBehaviour
{
	public float lifeSpan = 5;
	public int manaAmount = 10;								// How much Mana does the spell use?
	
	private float _life = 0;
	private PlayerCombatController _combatController;		// Reference to the player combat controller which is used to get weapon info
	private PlayerStatController _statController;			// Reference to the player's stat information
	private GameObject _player;								// Reference to the Player.
	
	private Transform _t;
	
	public PlayerCombatController CombatController
	{
		get { return _combatController; }
		set { _combatController = value; }
	}

	void Start()
	{
		_t = transform;
		_life = lifeSpan;
		_player = GameObject.FindGameObjectWithTag(Tags.player);
		_statController = _player.GetComponent<PlayerStatController>();

		if(_statController.mana < manaAmount)	// Check if player got enough Mana to cast spell
		{
			Debug.Log ("Not enough Mana!");
			Destroy(gameObject);
		}

		else
		{
			_statController.TakeMana(manaAmount); // Use Mana
		}
	}
	
	void Update()
	{
		if(_life <= 0)
		{
			Destroy(gameObject);
		}
		else
		{
			_life -= Time.deltaTime;
		}
	}
	
	void FixedUpdate()
	{
		if(_combatController)
		{
			GetComponent<Rigidbody>().velocity = _t.forward * _combatController.spellSpeed;
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(_combatController)
		{
			if(other.CompareTag(Tags.enemy))
			{
				EnemyStatController enemyStats = other.GetComponent<EnemyStatController>();
				
				if(enemyStats)
				{
					enemyStats.TakeDamage(_combatController.WeaponDamage);
					Destroy(gameObject);
				}
			}
		}
	}
}