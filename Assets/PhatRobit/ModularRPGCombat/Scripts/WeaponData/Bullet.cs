using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
	public float lifeSpan = 5;

	private float _life = 0;
	private PlayerCombatController _combatController;			// Reference to the player combat controller which is used to get weapon info

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
			GetComponent<Rigidbody>().velocity = _t.forward * _combatController.rangedSpeed;
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