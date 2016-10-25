using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

class DefenseShipController : ShipControlBase
{
    [SerializeField]
    private Collider _defenseRangeCollider;
    [SerializeField]
    private Transform _baseShip;
    [SerializeField]
    private Transform _additionShip;

    private const float StopValue = 0.05f;

    private Transform _defenseRangeTr;

    private bool _stopStartPosSetted;
    private Vector3 _stopStartPos;
    private Vector3 _stopEndPos;
    private Direction _direction;

    protected override void Awake()
    {
        base.Awake();
        _defenseRangeTr = _defenseRangeCollider.transform;
    }

    public override void InitShip(ShipType shipType, SpacePort spawnPort, bool isEnemy = false)
    {
        base.InitShip(shipType, spawnPort, isEnemy);
        _defenseRangeCollider.enabled = false;
        _stopStartPosSetted = false;
        _defenseRangeTr.localPosition = Vector3.zero;
    }

    public override void TakeOff(Vector3 destination)
    {
        if (!_stopStartPosSetted)
        {
            _stopStartPos = destination;
            _stopStartPosSetted = true;
        }
        else
        {
            _stopEndPos = destination;
            base.TakeOff(_stopStartPos);
            _stopStartPosSetted = false;
        }
    }

    protected override void OnShipSelected(ShipControlBase ship)
    {
        base.OnShipSelected(ship);
        if (ship != this)
            _stopStartPosSetted = false;
    }

    protected override void InitEnemy()
    {
        base.InitEnemy();
    }

    protected override void InitFriend()
    {
        base.InitFriend();
    }

    private void StopShip()
    {
        _body.velocity = Vector3.zero;
        _body.isKinematic = true;
    }

    private void Defense()
    {

        Debug.Log(_direction.ToString());

    }

    private void SetDirection()
    {
        var deltaX = Mathf.Abs(_stopEndPos.x - _stopStartPos.x);
        var deltaY = Mathf.Abs(_stopEndPos.y - _stopStartPos.y);
        if (deltaX > deltaY)
            _direction = _stopEndPos.x > _stopStartPos.x ? Direction.RIGHT : Direction.LEFT;
        else
            _direction = _stopEndPos.y > _stopStartPos.y ? Direction.UP : Direction.DOWN;
    }

    private void SetNormalStopEndPos()
    {
        var dir = new Vector3();
        switch (_direction)
        {
            case Direction.LEFT:
                dir = Vector3.left;
                break;
            case Direction.RIGHT:
                dir = Vector3.right;
                break;
            case Direction.UP:
                dir = Vector3.up;
                break;
            case Direction.DOWN:
                dir = Vector3.down;
                break;
        }
        _stopEndPos = _baseShip.localPosition + dir;
    }

    private void SetDeffensePos_And_EnableCollider()
    {
        switch (_direction)
        {
            case Direction.LEFT:
                _defenseRangeTr.localPosition += Vector3.left * 0.5f;
                break;
            case Direction.RIGHT:
                _defenseRangeTr.localPosition += Vector3.right * 0.5f;
                break;
            case Direction.UP:
                _defenseRangeTr.Rotate(0, 0, 90);
                _defenseRangeTr.localPosition += Vector3.up * 0.5f;
                break;
            case Direction.DOWN:
                _defenseRangeTr.Rotate(0, 0, 90);
                _defenseRangeTr.localPosition += Vector3.down * 0.5f;
                break;
        }

        _defenseRangeCollider.enabled = true;
    }

    private void FixedUpdate()
    {
        if (ShipState != ShipState.FLYING)
            return;

        if (_body.velocity == Vector3.zero)
        {
            if (_additionShip.localPosition == _stopEndPos)
                return;

            _additionShip.localPosition
                = Vector3.Lerp(_additionShip.localPosition, _stopEndPos, Time.deltaTime);

            if(Vector3.Distance(_additionShip.localPosition, _stopEndPos) < StopValue)
            {
                _additionShip.localPosition = _stopEndPos;
                SetDeffensePos_And_EnableCollider();
                ShipType = ShipType.DEFFENSE;
            }
        }
        else
        {
            if (Vector3.Distance(_transform.position, _stopStartPos) < StopValue)
            {
                StopShip();
                SetDirection();
                SetNormalStopEndPos();
                Defense();
            }
        }

    }

}

