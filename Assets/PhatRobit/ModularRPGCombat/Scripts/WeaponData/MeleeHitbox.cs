using UnityEngine;
using System.Collections;

public class MeleeHitbox : MonoBehaviour
{
	public PlayerCombatController playerCombat;
	public float activeTime = 1;

	private float _timer = 0;

	void Update()
	{
		if(_timer >= activeTime)
		{
			_timer = 0;
			gameObject.SetActive(false);
		}
		else
		{
			_timer += Time.deltaTime;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(playerCombat)
		{
			if(other.CompareTag(Tags.enemy))
			{
				int damage = playerCombat.WeaponDamage;

				if(playerCombat.Animator.GetBool("SpecialAttack"))
				{
					damage *= 2;
				}

				EnemyStatController enemyStats = other.GetComponent<EnemyStatController>();

				if(enemyStats && enemyStats.Alive)
				{
					enemyStats.TakeDamage(damage);
				}
			}
		}
	}
}