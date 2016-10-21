using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class ShipControlBase : MonoBehaviour
{
    [SerializeField] private float _speed = 0.00001f;

    public static ShipControlBase SelectedShip { get; private set; }

    public ShipType ShipType { get; private set; }

    private Rigidbody _body;

    public virtual void InitShip(ShipType shipType)
    {
        ShipType = shipType;
    }

    public virtual void SetSelectedShip(ShipControlBase ship = null)
    {
        SelectedShip = ship ? ship : this;
        SignalMgr.OnShipSelected.Invoke(SelectedShip);
    }

    public virtual void TakeOff(Vector3 destination)
    {
        var dir = destination - SelectedShip.transform.position;
        _body.AddForce(dir * _speed);
    }

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
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

    protected virtual void OnShipSelected(ShipControlBase ship)
    {
        if (ship == this)
        {
            ShipControlBase.SelectedShip = this;
            transform.localScale = Vector3.one*1.3f;
        }
        else
            transform.localScale = Vector3.one;
    }
}