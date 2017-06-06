using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;
using System;


[RequireComponent(typeof(TNObject))]
public class NetShipControllerView : thelab.mvc.View<SpaceApplication>
{
    private float lastSendTime = 0f;
    [SerializeField]
    private float sendInterval = 0.1f;
    [SerializeField]
    private float lerpRate = 4.0f;

    private Vector3 oldPos;

    private Vector3 syncPos;
    private float fraction;
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
        fraction += Time.deltaTime; //sendInterval / Time.deltaTime
        //fraction = Mathf.Clamp01(fraction);
        //Log(fraction + "===" + Time.deltaTime);
        ShipTransform.position = Vector3.Lerp(oldPos, syncPos, fraction);
        ShipTransform.rotation = Quaternion.Lerp(ShipTransform.rotation, syncRot, Time.deltaTime * lerpRate);

        if (!tno.isMine)
            return;

        m_ship.Update_bynet();
        lastSendTime += Time.deltaTime;
        if (lastSendTime >= sendInterval)
        {
            //Debug.Log("send at " + lastSendTime);
            lastSendTime = 0;
            tno.Send("SetRB", Target.AllSaved, m_ship.CachedTransform.position, m_ship.CachedTransform.rotation);
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
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        m_ship.ResetCachedTransform();
        Notify(SpaceNotifications.ShipRespawned);
    }

    /// <summary>
    /// RFC for the rigidbody will be called once per second by default.
    /// </summary>
    [RFC]
    private void SetRB(Vector3 pos, Quaternion rot)
    {
        fraction = 0;
        oldPos = ShipTransform.position;
        syncPos = pos;
        syncRot = rot;
    }

    public void EnableShip(bool enable)
    {
        gameObject.SetActive(enable);
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
