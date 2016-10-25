using UnityEngine;
using System.Collections;


class ShipVisibleEye : ShipEyeBase
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag(Tags.EnemyHero))
        {
            var sde = other.GetComponent<ShipDisibleEye>();
            if (sde)
            {
                sde.ShowShip();
                var atkShip = _ship as AttackShipController;
                if (atkShip)
                    atkShip.ReadyToAttack(other.transform);
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

