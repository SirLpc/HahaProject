using UnityEngine;
using System.Collections;

class ShipDisibleEye : ShipEyeBase
{
    private int _inEyeShip = 0;

    public void ShowShip()
    {
        _inEyeShip++;
        ShowAllRender(true);
    }

    public void HideShip()
    {
        _inEyeShip--;
        if (_inEyeShip < 0)
            _inEyeShip = 0;
        if (_inEyeShip == 0)
            ShowAllRender(false);
    }



}

