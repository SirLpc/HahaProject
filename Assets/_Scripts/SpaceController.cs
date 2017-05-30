using System.Collections;
using UnityEngine;
using thelab.mvc;
using TNet;

public class SpaceController : Controller<SpaceApplication>
{



    private IEnumerator Start()
    {

        while (TNManager.isJoiningChannel)
            yield return null;

        CreatMyShip();

    }

    public override void OnNotification(string p_event, Object p_target, params object[] p_data)
    {
        switch (p_event)
        {
            case SpaceNotifications.ShipCreated:
                app.view.AddNewShip(p_data[0] as NetShipControllerView, (bool)p_data[1]);
                break;

            case SpaceNotifications.SpeedUp:
                app.view.MyShip.SpeedUp((bool)p_data[0]);
                break;

            default:
                break;
        }
    }


    #region CreatMyShip
    private int _channelID = 0;
    [SerializeField]
    private string _shipPrefabPath;
    private bool _persistent = false;

    private void CreatMyShip()
    {
        if (_channelID < 1) _channelID = TNManager.lastChannelID;
        TNManager.Instantiate(_channelID, "CreateAtPosition", _shipPrefabPath, _persistent, transform.position, transform.rotation);
    }

    [RCC]
    static GameObject CreateAtPosition(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        // Instantiate the prefab
        GameObject go = prefab.Instantiate();

        // Set the position and rotation based on the passed values
        Transform t = go.transform;
        t.position = pos;
        t.rotation = rot;
        return go;
    }
    #endregion

}
