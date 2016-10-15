using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
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
        //GUILayout.Label("                     InputMousePos : " + msg.ToString());
        if (GUILayout.Button("Load"))
        {
            SceneManager.LoadScene(3);
            //Application.LoadLevel(3);
        }
    }

}
