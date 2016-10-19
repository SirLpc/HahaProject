using UnityEngine;
using System.Collections;

public class AOE : MonoBehaviour
{

	public float lifeSpan = 15;								// How long the arrow lasts (in seconds) before being destroyed
	public float damageInterval = 3;						// How often the damage is inflicted within the AOE;
	
	private float _life = 0;								// Timer for AOE before being destroyed
	
	private PlayerCombatController _combatController;		// Reference to the player's combat information

	private float _timer = 0;
	
	public PlayerCombatController CombatController
	{
		get { return _combatController; }
		set { _combatController = value; }
	}
	
	void Start()
	{
		
		// Set up our life timer
		_life = lifeSpan;

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
	
	void OnTriggerStay(Collider other)
	{
		// Deal damage to enemies if colliding with them and destroy gameobject
		if(_combatController)
		{
			if(other.CompareTag(Tags.enemy))
			{
				EnemyStatController enemyStats = other.GetComponent<EnemyStatController>();
				if(enemyStats)
				{
					if(_timer > damageInterval)
					{
						enemyStats.TakeDamage(_combatController.WeaponDamage);
						_timer = 0;
					}
					else
					{
						_timer += Time.deltaTime;
					}
				}
			}
		}
	}
}