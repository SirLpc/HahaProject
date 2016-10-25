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

    private void ShowAllRender(bool show)
    {
        _renderTr.gameObject.layer = show ? _visibleLayer : _disibleLayer;
        ShowRender(_renderTr, show);
    }

    private void ShowRender(Transform tr, bool show)
    {
        for (int i = 0; i < tr.childCount; i++)
        {
            var child = tr.GetChild(i);
           child.gameObject.layer = show ? _visibleLayer : _disibleLayer;
            ShowRender(child, show);
        }
    }

}

