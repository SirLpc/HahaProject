using UnityEngine;
using System.Collections.Generic;

public class InputController : MonoBehaviour
{
	#region ===字段===

    private LayerMask _mask = 12;
    private Camera _mainCamera;
    private Transform _transform;

	#endregion

	#region ===属性===
	#endregion

	#region ===Unity事件=== 快捷键： Ctrl + Shift + M /Ctrl + Shift + Q  实现

    private void Awake()
    {
        _transform = transform;
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DoClick();
        }
    }

	#endregion

	#region ===方法===

    private void DoClick()
    {
        RaycastHit hit;
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag(Tags.EnemyHero))
                return;

            if (hit.transform == _transform)
            {
                var tPos = new Vector3(hit.point.x, hit.point.y, 0);
                ShipControlBase.SelectedShip.TakeOff(tPos);
            }
            else
            {
                var scb = hit.transform.GetComponent<ShipControlBase>();
                if(scb != null)
                    scb.SetSelectedShip();
            }
        }
    }

	#endregion
}
