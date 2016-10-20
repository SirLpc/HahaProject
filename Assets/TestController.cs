using UnityEngine;
using System.Collections;

public class TestController : MonoBehaviour {
    private float speed = 0.00001f;

	void Update () {

        if (Input.GetKeyDown(KeyCode.Q))
            GetComponent<Rigidbody>().AddForce(Vector3.up * speed);
	
	}
}
