using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class SpaceBullet : MonoBehaviour {

    private float _speed = 0.00001f;

    private Rigidbody _body;
    private UnityAction _onEnemyDestroyed;

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
    }

	public void Init (ShipStateBase target, UnityAction onEnemyDestroyed)
	{
	    _onEnemyDestroyed = onEnemyDestroyed;
        var dir = (target.transform.position - transform.position).normalized;
        _body.AddForce(dir * _speed);
	}

    public void OnTriggerEnter(Collider other)
    {
        var root = other.transform.root;
        if (root.CompareTag(Tags.EnemyHero))
        {
            var state = root.GetComponent<ShipStateBase>();
            if (!state)
                return;

            if (state.IsAlive)
            {
                if(state.TakeDamage(10) && _onEnemyDestroyed != null)
                    _onEnemyDestroyed.Invoke();
            }
            else if(_onEnemyDestroyed != null)
                    _onEnemyDestroyed.Invoke();

            Destroy(gameObject);
        }
    }

}
