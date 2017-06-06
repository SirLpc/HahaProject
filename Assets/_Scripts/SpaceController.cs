using System.Collections;
using UnityEngine;
using thelab.mvc;
using TNet;

public class SpaceController : Controller<SpaceApplication>
{

    private TNObject tno;

    private SpaceShipCtr _ship;
    public SpaceShipCtr Ship { get { return _ship = Assert(_ship); } }

    private int _channelID = 0;
    public int ChannelID { get { return _channelID; } }
    private bool _persistent = false;
    public bool Persistent { get { return _persistent; } }

    private float _waitPlayerJoinTimer = 0f;

    private void Awake()
    {
        tno = GetComponent<TNObject>();
    }

    private IEnumerator Start()
    {

        while (TNManager.isJoiningChannel)
            yield return null;

        if (_channelID < 1) _channelID = TNManager.lastChannelID;

        var randPos = Vector3.zero;// Random.insideUnitSphere * 100f;
        var randRotAng = Vector3.zero;// new Vector3(0f, Random.Range(0f, 360f), 0f);
        Ship.CreatMyShip(randPos, randRotAng);

        yield return StartCoroutine(CoWaitAllPlayerJoin());

        app.view.EnableAllShips(true);
    }

    private IEnumerator CoWaitAllPlayerJoin()
    {
        _waitPlayerJoinTimer = 0f;
        while (app.view.Ships.Count < app.model.TotalPlayerNum)
        {
            _waitPlayerJoinTimer += Time.deltaTime;
            if (_waitPlayerJoinTimer < app.model.WailPlayerJoinTime)
                yield return null;
            else
                yield break;
        }
    }



    public override void OnNotification(string p_event, Object p_target, params object[] p_data)
    {
        switch (p_event)
        {
            case SpaceNotifications.ShipCreated:
                var createdShip = p_data[0] as NetShipControllerView;
                app.view.AddNewShip(createdShip, (bool)p_data[1]);
                //游戏开始的时候不能动，要等所有玩家OK才开始
                createdShip.EnableShip(false);
                break;

            case SpaceNotifications.SpeedUp:
                app.view.MyShip.SpeedUp((bool)p_data[0]);
                break;

            case SpaceNotifications.CreatBullet:
                CreatBullet();
                break;

            case SpaceNotifications.BulletAttakOn:  //attacker called
                var tagetShip = p_data[0] as NetShipControllerView;
                Log("my id " + TNManager.playerID);
                Log("other ship ton id" + tagetShip.tno.ownerID + "===" + tagetShip.tno.owner.id);
                tno.Send("TakeDamage", tagetShip.tno.ownerID, (int)p_data[1], app.view.MyShip.tno.ownerID);
                break;

            //case SpaceNotifications.ShipTakeDamage: //under attack called
            //    var damage = (int)p_data[0];
            //    var attackShip = p_data[1] as NetShipControllerView;

            //    Ship.Damage(damage); 
            //    break;

            default:
                break;
        }
    }


 

    #region CreatBullet
    [SerializeField]
    private string _bulletPrefabPath;

    private float _lastFireTime = float.MinValue;
    private float _fireInterval = .25f;

    private void CreatBullet()
    {
        if (Time.time - _lastFireTime < _fireInterval)
            return;
        _lastFireTime = Time.time;

        if (_channelID < 1) _channelID = TNManager.lastChannelID;
        //TODO 根据速度，改变子弹生成的初始位置 +30往前生成点
        var pos = app.view.MyShip.ShipTransform.position;
        pos += app.view.MyShip.ShipTransform.forward * 20f;
        var rot = app.view.MyShip.ShipTransform.rotation;
        TNManager.Instantiate(_channelID, "CreateBulletAtPosition", _bulletPrefabPath, _persistent, pos, rot);
    }

    [RCC]
    static GameObject CreateBulletAtPosition(GameObject prefab, Vector3 pos, Quaternion rot)
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


    [TNet.RFC]
    private void TakeDamage(int damage, int attackPlayerId)
    {
        Log("damange rfc");
        Notify(SpaceNotifications.ShipTakeDamage, damage, attackPlayerId);
    }

}
