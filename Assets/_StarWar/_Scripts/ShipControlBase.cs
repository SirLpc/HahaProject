using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class ShipControlBase : MonoBehaviour
{

    private void OnMouseDown()
    {
        SignalMgr.OnShipSelected.Invoke(this);


    }

    private void OnEnable()
    {
        SignalMgr.OnShipSelected.AddListener(OnShipSelected);
    }

    private void OnDisable()
    {
        SignalMgr.OnShipSelected.RemoveListener(OnShipSelected);
    }

    private void OnDestroy()
    {
        SignalMgr.OnShipSelected.RemoveListener(OnShipSelected);
    }

    private void OnShipSelected(ShipControlBase ship)
    {
        transform.localScale = ship == this ? Vector3.one*1.3f : Vector3.one;
    }
}