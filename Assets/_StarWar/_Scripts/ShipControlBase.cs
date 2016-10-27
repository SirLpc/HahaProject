using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class ShipControlBase : MonoBehaviour, ITeam
{
    //todo kill test field
    public bool _isEnemy;
    public bool IsEnemy { get; set; }

    public static ShipControlBase SelectedShip { get; private set; }

    public ShipType ShipType { get; protected set; }
    public ShipState ShipState { get; protected set; }

    [SerializeField]
    protected Collider _moveCollider;
    [SerializeField]
    protected Transform _render;
    [SerializeField]
    protected Collider _eyeTrigger;

    protected float _speed = 0.00001f;

    protected Rigidbody _body;
    protected Transform _transform;
    protected Collider _touchCollider;
    protected Vector3 _continueDir;

    private SpacePort _spawnPort = null;

    //todo kill test scripts
    private void Start()
    {
        if (_isEnemy)
        {
            InitShip(ShipType.ATTACK, null, true);
        }
    }

    public virtual void InitShip(ShipType shipType, SpacePort spawnPort, bool isEnemy)
    {
        IsEnemy = isEnemy;
        ShipType = shipType;
        ShipState = ShipState.INBASE;
        _spawnPort = spawnPort;

        //todo 打开注释，因为这是为测试留的
        //_moveCollider.enabled = false;
        _eyeTrigger.enabled = false;

        if (isEnemy)
            InitEnemy();
        else
            InitFriend();
    }

    public virtual void InitEnemy()
    {
        gameObject.tag = Tags.EnemyHero;
    }

    public virtual void InitFriend()
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

        _moveCollider.enabled = true;
        _eyeTrigger.enabled = true;

        ShipState = ShipState.FLYING;

        _spawnPort.OnShipTakeOff();
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