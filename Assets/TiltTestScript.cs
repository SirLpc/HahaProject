using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class TiltTestScript : MonoBehaviour
{
    float msg = 0f;

	void Update ()
    {
        msg = CrossPlatformInputManager.GetAxis("Mouse X");
        
	}

    void OnGUI()
    {
        GUILayout.Label("                     InputMousePos : " + msg.ToString());
    }

}
