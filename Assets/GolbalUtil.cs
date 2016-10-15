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
        GameObject.DontDestroyOnLoad(this);
        Application.runInBackground = true;
    }

	#endregion

	#region ===方法===

	#endregion
}
