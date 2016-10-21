using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

class DefenseShipController : ShipControlBase
{
    private bool _stopStartPosSetted = false;
    private Vector3 _stopStartPos;
    private Vector3 _stopEndPos;

    public override void TakeOff(Vector3 destination)
    {
        if (!_stopStartPosSetted)
        {
            _stopStartPos = destination;
            _stopStartPosSetted = true;
        }
        else
        {
            _stopEndPos = destination;
            base.TakeOff(_stopStartPos);
        }
    }

    protected override void OnShipSelected(ShipControlBase ship)
    {
        base.OnShipSelected(ship);
        if (ship != this)
            _stopStartPosSetted = false;
    }

    private void FixedUpdate()
    {
    }
}

