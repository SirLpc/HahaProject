using UnityEngine;
using System.Collections;
using OneByOne;

public class MeleeHitbox : SkillBase
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
            //if(other.CompareTag(Tags.enemy))
            if (other.CompareTag(Tags.EnemyHero))
            {
				int damage = playerCombat.WeaponDamage;

				if(playerCombat.Animator.GetBool("SpecialAttack"))
				{
					damage *= 2;
				}

				StateBase enemyStats = other.GetComponent<StateBase>();

				if(enemyStats && enemyStats.Alive)
				{
					//enemyStats.TakeDamage(damage);
				    base.target = other.gameObject;
                    base.NetWorkSetDamage();
				}
			}
		}
	}
}