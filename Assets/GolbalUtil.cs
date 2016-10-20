using UnityEngine;
using System.Collections.Generic;


public class GolbalUtil : MonoBehaviour
{
	#region ===字段===
	#endregion

	#region ===属性===
	#endregion

	#region ===Unity事件=== 快捷键： Ctrl + Shift + M /Ctrl + Shift + Q  实现

    private void Awake()
    {
        Application.targetFrameRate = 45;
        Application.runInBackground = true;

        GameObject.DontDestroyOnLoad(this);
    }

	#endregion

	#region ===方法===

	#endregion
}
