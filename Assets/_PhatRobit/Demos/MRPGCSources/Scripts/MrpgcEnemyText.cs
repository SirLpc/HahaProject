using UnityEngine;
using System.Collections;

public class MrpgcEnemyText : MonoBehaviour
{
	private Transform _t;

	void Start()
	{
		_t = transform;
		_t.localScale = new Vector3(-1, 1, 1);
	}

	void Update()
	{
		_t.LookAt(Camera.main.transform);
	}
}