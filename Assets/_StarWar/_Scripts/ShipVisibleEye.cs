using UnityEngine;
using System.Collections;


class ShipVisibleEye : ShipEyeBase
{
    public void OnTriggerEnter(Collider other)
    {
        if (_renderTr.name.Equals(Tags.FriendHero))
            return;

        //if (other.gameObject.gameObject.layer == _disibleLayer)
        {
            var sde = other.GetComponent<ShipDisibleEye>();
            if (sde)
                sde.ShowShip();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (_renderTr.name.Equals(Tags.FriendHero))
            return;

        //if (other.gameObject.layer == _visibleLayer)
        {
            var sde = other.GetComponent<ShipDisibleEye>();
            if (sde)
                sde.HideShip();
        }
    }
}

