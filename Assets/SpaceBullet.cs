using UnityEngine;
using System.Collections;

public class SpaceBullet : MonoBehaviour {

    private float _speed = 0.00001f;

    private Rigidbody _body;

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
    }

	public void Init (Transform target)
    {
        var dir = (target.position - transform.position).normalized;
        _body.AddForce(dir * _speed);
	}

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag(Tags.EnemyHero))
        {
            Destroy(gameObject);
        }
    }

}
