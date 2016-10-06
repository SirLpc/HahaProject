using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour
{
	public float forwardForce = 50;							// How much force to be applied to the forward direction of the arrow
	public float upForce = 10;								// How much force to be applied to the up direction of the arrow
	public float lifeSpan = 5;								// How long the arrow lasts (in seconds) before being destroyed

	private float _life = 0;								// Timer for arrow before being destroyed

	private PlayerCombatController _combatController;		// Reference to the player's combat information

	private Transform _t;									// Reference to gameobject's own transform

	public PlayerCombatController CombatController
	{
		get { return _combatController; }
		set { _combatController = value; }
	}

	void Start()
	{
		_t = transform;

		// Set up our life timer
		_life = lifeSpan;

		// Apply initial force
		GetComponent<Rigidbody>().AddForce(forwardForce * _t.forward);
		GetComponent<Rigidbody>().AddForce(upForce * _t.up);
	}

	void Update()
	{
		// Destory object when life runs out
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
		// Rotate the arrow so it is always facing the direction it is moving
		if(GetComponent<Rigidbody>().velocity != Vector3.zero)
		{
			GetComponent<Rigidbody>().rotation = Quaternion.LookRotation(GetComponent<Rigidbody>().velocity);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		// Deal damage to enemies if colliding with them and destroy gameobject
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