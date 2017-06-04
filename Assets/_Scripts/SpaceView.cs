using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceView : thelab.mvc.View<SpaceApplication>
{

    public NetShipControllerView MyShip { get { return _myShip; } }
    private NetShipControllerView _myShip;

    private List<NetShipControllerView> _ships = new List<NetShipControllerView>();

    public void AddNewShip(NetShipControllerView ship, bool isMyShip)
    {
        _ships.Add(ship);
        if (isMyShip)
            _myShip = ship;

        ship.EnableParticaleFollow(isMyShip);
    }

    public void AttakOn(NetShipControllerView ship, int damage)
    {

    }


}
