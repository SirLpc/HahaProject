using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;
using System;

/// <summary>
/// 可以通过
/// 1. 减少发包率（意思就是 增大 sendInterval  发包间隔）
/// 2. 增大 closeEnough 距离
/// 3. 增大 normalLerpRate、fasterLerpRate 插值速率
/// </summary>
[RequireComponent(typeof(TNObject))]
public class NetShipControllerView : thelab.mvc.View<SpaceApplication>
{
    private float lastSendTime = 0f;
    private float sendInterval = 0.1f;

    [SerializeField]
    private float lerpRate = 9.0f;


    private Vector3 syncPos;
    private float threshold = 0.5f;

    private Quaternion syncRot;

    [NonSerialized]
    public TNObject tno;
    private SpaceshipController m_ship;
    private ETCJoystick m_joystic;
    [SerializeField]
    private ParticleFollow _particleFollow;
    public Transform ShipTransform { get; private set; }

    private void Awake()
    {
        tno = GetComponent<TNObject>();

        m_ship = GetComponentInChildren<SpaceshipController>();
        ShipTransform = m_ship.transform;
        m_joystic = FindObjectOfType<ETCJoystick>();
        if (m_joystic == null)
            Debug.LogError("Joystick NOT found!!");
    }

    private void OnEnable()
    {
        TNManager.onSetPlayerData += OnSetPlayerData;
    }

    private void OnDisable()
    {
        TNManager.onSetPlayerData -= OnSetPlayerData;
    }

    private void Start()
    {
        Notify(SpaceNotifications.ShipCreated, this, tno.isMine);
    }

    /// <summary>
    /// Only the car's owner should be updating the movement axes, and the result should be sync'd with other players.
    /// </summary>

    private void Update()
    {
        if (!tno.isMine)
        {
            ShipTransform.position = Vector3.Lerp(ShipTransform.position, syncPos, Time.deltaTime * lerpRate);
            ShipTransform.rotation = Quaternion.Lerp(ShipTransform.rotation, syncRot, Time.deltaTime * lerpRate);
            return;
        };

        m_ship.Update_bynet();
        if (Time.time - lastSendTime > sendInterval)
        {
            tno.Send("SetRB", Target.OthersSaved, ShipTransform.position, ShipTransform.rotation);
            lastSendTime = Time.time;
        }
    }

    //private void FixedUpdate()
    //{

    //}

    private void LateUpdate()
    {
        if (tno.isMine)
            m_ship.LateUpdate_bynet();
    }

    private void OnSetPlayerData(Player p, string path, DataNode node)
    {
        if (!p.Equals(TNManager.player))
            return;

        if (path != SpaceConsts.PlayerHpPath)
            return;

        var hp = TNManager.GetPlayerData<int>(SpaceConsts.PlayerHpPath);
        Log("in view set player data" + hp);
        if (hp <= 0)
            gameObject.SetActive(false);
    }

    /// <summary>
    /// RFC for the rigidbody will be called once per second by default.
    /// </summary>

    [RFC]
    private void SetRB(Vector3 pos, Quaternion rot)
    {
        syncPos = pos;
        syncRot = rot;
    }

    public void SpeedUp(bool enableSpeedUp)
    {
        m_ship.EnableSpeedUp = enableSpeedUp;
    }

    public void EnableParticaleFollow(bool enabled)
    {
        _particleFollow.gameObject.SetActive(enabled);
    }
}
