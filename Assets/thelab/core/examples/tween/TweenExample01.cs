using UnityEngine;
using System.Collections;
using thelab.core;

public class TweenExample01 : MonoBehaviour 
{

    public Transform target;
    public Transform position;

	// Use this for initialization
	void Start () 
    {
	
        
	}
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            DoTween();
        }
        float r = 2.5f;
        float w0 = Mathf.Sin(Time.time * 360f * Mathf.Deg2Rad * 0.5f) * r * 0.25f;
        float w1 = Mathf.Cos(Time.time * 360f * Mathf.Deg2Rad * 0.3f) * r * 1f;
        float w2 = Mathf.Sin(Time.time * 360f * Mathf.Deg2Rad * 0.2f) * r * 2f;
        Vector3 pos = new Vector3(w0 + w1, w1 + w2, w0 + w2);
        position.position = pos;
    }

    public void DoTween()
    {
        Tween.Add<Vector3>(target,"position",position.position,0.5f,Elastic.OutBig);
    }
	

}
