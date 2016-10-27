using UnityEngine;
using System.Collections;


class ShipVisibleEye : ShipEyeBase
{
    public void OnTriggerEnter(Collider other)
    {
        var root = other.transform.root;
        if (root.CompareTag(Tags.EnemyHero))
        {
            var sde = other.GetComponent<ShipDisibleEye>();
            if (sde)
            {
                sde.ShowShip();
                var atkShip = _ship as AttackShipController;
                if (atkShip)
                {
                    var ssb = root.GetComponent<ShipStateBase>();
                    if(ssb)
                        atkShip.ReadyToAttack(ssb);
                }
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.transform.root.CompareTag(Tags.EnemyHero))
        {
            var sde = other.GetComponent<ShipDisibleEye>();
            if (sde)
                sde.HideShip();
        }
    }

}

