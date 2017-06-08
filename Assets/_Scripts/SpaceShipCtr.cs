using System.Collections;
using System.Collections.Generic;
using TNet;
using UnityEngine;
using thelab.mvc;

public class SpaceShipCtr : Controller<SpaceApplication>
{

    private TNObject tno;

    private void Awake()
    {
        tno = GetComponent<TNObject>();
    }

    [SerializeField]
    private string _shipPrefabPath;


    public override void OnNotification(string p_event, Object p_target, params object[] p_data)
    {
        switch (p_event)
        {
            //Only under attacked ship will call this function!!!!
            case SpaceNotifications.ShipTakeDamage: //under attack called
                var damage = (int)p_data[0];
                var attackPid = (int)p_data[1];
                var attackShip = app.view.Ships.Find(s => s.tno.ownerID == attackPid);
                Damage(damage);
                break;

            //Only under attacked ship will call this function!!!!
            case SpaceNotifications.ShipRespawned:
                SetHp(100);
                break;
       
            default:
                break;
        }
    }

    public void CreatMyShip(Vector3 position, Vector3 rotation)
    {
        TNManager.Instantiate(app.controller.ChannelID, "CreateShipAtPosition",
            _shipPrefabPath, app.controller.Persistent, position, rotation);

        SetHp(100);
    }

    private void Damage(int damage)
    {
        int hp = TNManager.GetPlayerData<int>(SpaceConsts.PlayerHpPath);
        hp -= damage;
        hp = Mathf.Clamp(hp, 0, app.model.ShipMaxHp);
        SetHp(hp);
    }

    private void SetHp(int hp)
    {
        TNManager.SetPlayerData(SpaceConsts.PlayerHpPath, hp);
        Log("set hp" + hp);
    }

    [RCC]
    static GameObject CreateShipAtPosition(GameObject prefab, Vector3 pos, Vector3 rot)
    {
        // Instantiate the prefab
        GameObject go = prefab.Instantiate();

        // Set the position and rotation based on the passed values
        Transform t = go.transform;
        t.position = pos;
        t.eulerAngles = rot;

        return go;
    }

}
