using UnityEngine;
using System.Collections;

public class HpUp : MonoBehaviour {

    bool move = false;
    [SerializeField]
    private TextMesh mesh;

    public void setValue(int value) {
        if (value > 0)
        {
            mesh.color = Color.red;
        }
        else {
            mesh.color = Color.green;
        }
        mesh.text = Mathf.Abs(value).ToString();
        move = true;

        Invoke("remove", 1);
    }
    void remove() {
        move = false;
        Destroy(gameObject);
    }
	void Update () {
        if (move) {
            transform.Translate(Vector3.up * Time.deltaTime);
        }
	}
}
