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

    public int MaxHp = 100;

    public override void OnNotification(string p_event, Object p_target, params object[] p_data)
    {
        switch (p_event)
        {
            case SpaceNotifications.ShipTakeDamage: //under attack called
                var damage = (int)p_data[0];
                var attackShip = p_data[1] as NetShipControllerView;
                Damage(damage);
                break;

            default:
                break;
        }
    }

    public void CreatMyShip()
    {
        TNManager.Instantiate(app.controller.ChannelID, "CreateShipAtPosition",
            _shipPrefabPath, app.controller.Persistent, transform.position, transform.rotation);

        SetHp(100);
    }

    private void Damage(int damage)
    {
        int hp = TNManager.GetPlayerData<int>(SpaceConsts.PlayerHpPath);
        hp -= damage;
        hp = Mathf.Clamp(hp, 0, MaxHp);
        SetHp(hp);
    }

    private void SetHp(int hp)
    {
        TNManager.SetPlayerData(SpaceConsts.PlayerHpPath, hp);
        Log("set hp" + hp);
    }

    [RCC]
    static GameObject CreateShipAtPosition(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        // Instantiate the prefab
        GameObject go = prefab.Instantiate();

        // Set the position and rotation based on the passed values
        Transform t = go.transform;
        t.position = pos;
        t.rotation = rot;
        return go;
    }

}
