using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using thelab.mvc;

public class ShipInputView : View<SpaceApplication>
{

	
    public void OnClickSpeedUp(bool activeSpeedUp)
    {
        Notify(SpaceNotifications.SpeedUp, activeSpeedUp);
    }


}
