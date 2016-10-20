using UnityEngine;
using System.Collections;

public class DebugManager : SA_Singleton<DebugManager>
{

    private string _msg = string.Empty;

    public void Log(string msg)
    {
        _msg += ("\t" + msg);
    }

    private void OnGUI()
    {
        GUILayout.Label(_msg);
    }

}
