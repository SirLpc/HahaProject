using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;

[RequireComponent(typeof(TNObject))]
public class NetShipControllerView : thelab.mvc.View<SpaceApplication>
{
    /// <summary>
    /// Maximum number of updates per second when synchronizing input axes.
    /// The actual number of updates may be less if nothing is changing.
    /// </summary>

    [Range(1f, 20f)]
    public float inputUpdates = 10f;

    /// <summary>
    /// Maximum number of updates per second when synchronizing the rigidbody.
    /// </summary>

    [Range(0.25f, 5f)]
    public float rigidbodyUpdates = 1f;

    [Range(1f, 20f)]
    public float lerpRate = 15f;

    /// <summary>
    /// We want to cache the network object (TNObject) we'll use for network communication.
    /// If the script was derived from TNBehaviour, this wouldn't have been necessary.
    /// </summary>

    [System.NonSerialized]
    public TNObject tno;

    private Vector4 mLastInput;
    private float mLastInputSend = 0f;
    private float mNextRB = 0f;

    private SpaceshipController m_ship;
    private ETCJoystick m_joystic;

    [SerializeField]
    private ParticleFollow _particleFollow;

    private void Awake()
    {
        tno = GetComponent<TNObject>();

        m_ship = GetComponentInChildren<SpaceshipController>();
        m_joystic = FindObjectOfType<ETCJoystick>();
        if (m_joystic == null)
            Debug.LogError("Joystick NOT found!!");
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
        // Only the player that actually owns this car should be controlling it
        if (!tno.isMine) return;

        m_ship.Update_bynet();

        //float time = Time.time;
        //float delta = time - mLastInputSend;
        //float delay = 1f / inputUpdates;

        //// Don't send updates more than 20 times per second
        //if (delta > 0.05f)
        //{
        //    // The closer we are to the desired send time, the smaller is the deviation required to send an update.
        //    float threshold = Mathf.Clamp01(delta - delay) * 0.5f;

        //    // If the deviation is significant enough, send the update to other players
        //    if (Tools.IsNotEqual(mLastInput.x, m_ship.SmoothedInput.x, threshold) ||
        //        Tools.IsNotEqual(mLastInput.y, m_ship.SmoothedInput.y, threshold) ||
        //        Tools.IsNotEqual(mLastInput.z, m_ship.SmoothedInput.z, threshold) ||
        //        Tools.IsNotEqual(mLastInput.w, m_ship.SmoothedInput.w, threshold))
        //    {
        //        mLastInputSend = time;
        //        mLastInput = m_ship.SmoothedInput;
        //        tno.Send("SetAxis", Target.OthersSaved, m_ship.SmoothedInput);
        //    }
        //}

        //// Since the input is sent frequently, rigidbody only needs to be corrected every couple of seconds.
        //// Faster-paced games will require more frequent updates.
        //if (mNextRB < time)
        //{
        //    mNextRB = time + 1f / rigidbodyUpdates;
        //    tno.Send("SetRB", Target.OthersSaved, m_ship.Rigidbody.position, m_ship.Rigidbody.rotation,
        //        m_ship.Rigidbody.velocity, m_ship.Rigidbody.angularVelocity);
        //}
    }

    private Vector3 targetPos;
    private Quaternion targetRot;
    private void FixedUpdate()
    {
        if(!tno.isMine)
        {
            m_ship.Rigidbody.position = Vector3.Lerp(m_ship.Rigidbody.position, targetPos, Time.deltaTime * lerpRate);
            m_ship.Rigidbody.rotation = Quaternion.Lerp(m_ship.Rigidbody.rotation, targetRot, Time.deltaTime * lerpRate);
            return;
        }

        tno.Send("SetRB", Target.OthersSaved, m_ship.Rigidbody.position, m_ship.Rigidbody.rotation);
    }

    private void LateUpdate()
    {
        if (!tno.isMine) return;

        m_ship.LateUpdate_bynet();
    }

    /// <summary>
    /// RFC for the input will be called several times per second.
    /// </summary>

    [RFC]
    private void SetAxis(Vector4 v) { m_ship.SmoothedInput = v; }

    /// <summary>
    /// RFC for the rigidbody will be called once per second by default.
    /// </summary>

    [RFC]
    private void SetRB(Vector3 pos, Quaternion rot)
    {
        targetPos = pos;
        targetRot = rot;
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
