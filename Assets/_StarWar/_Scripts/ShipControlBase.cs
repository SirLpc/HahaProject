using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class ShipControlBase : MonoBehaviour
{
    public bool IsEnemy = false;

    public static ShipControlBase SelectedShip { get; private set; }

    public ShipType ShipType { get; protected set; }
    public ShipState ShipState { get; protected set; }

    protected float _speed = 0.00001f;

    protected Rigidbody _body;
    protected Transform _transform;
    protected Collider _touchCollider;
    protected Vector3 _continueDir;

    private SpacePort _spawnPort = null;

    //todo kill test scripts
    private void Start()
    {
        if (IsEnemy)
        {
            InitShip(ShipType.ATTACK, null, true);
        }
    }

    public virtual void InitShip(ShipType shipType, SpacePort spawnPort, bool isEnemy = false)
    {
        IsEnemy = isEnemy;
        ShipType = shipType;
        ShipState = ShipState.INBASE;
        _spawnPort = spawnPort;
        if (isEnemy)
            InitEnemy();
        else
            InitFriend();
    }

    protected virtual void InitEnemy()
    {
        gameObject.tag = Tags.EnemyHero;
    }

    protected virtual void InitFriend()
    {
        gameObject.tag = Tags.FriendHero;
    }

    public virtual void SetSelectedShip(ShipControlBase ship = null)
    {
        SelectedShip = ship ? ship : this;
        SignalMgr.OnShipSelected.Invoke(SelectedShip);
    }

    public virtual void TakeOff(Vector3 destination)
    {
        var dir = (destination - SelectedShip.transform.position).normalized;
        _body.AddForce(dir * _speed);

        _touchCollider.enabled = false;
        _transform.localScale = Vector3.one;
        _transform.parent = null;

        ShipState = ShipState.FLYING;
    }

    protected virtual void Awake()
    {
        _body = GetComponent<Rigidbody>();
        _transform = transform;
        _touchCollider = GetComponent<Collider>();
    }


    private void OnEnable()
    {
        SignalMgr.OnShipSelected.AddListener(OnShipSelected);
    }

    private void OnDisable()
    {
        SignalMgr.OnShipSelected.RemoveListener(OnShipSelected);
    }

    private void OnDestroy()
    {
        SignalMgr.OnShipSelected.RemoveListener(OnShipSelected);
    }

    protected virtual void OnShipSelected(ShipControlBase ship)
    {
        if (ship == this)
        {
            ShipControlBase.SelectedShip = this;
            _transform.localScale = Vector3.one*1.3f;
        }
        else
            _transform.localScale = Vector3.one;
    }

}