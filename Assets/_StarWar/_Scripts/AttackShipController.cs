using UnityEngine;
using System.Collections.Generic;


public class AttackShipController : ShipControlBase
{
    #region ===字段===

    [SerializeField]
    private Collider _moveCollider ;
    [SerializeField]
    private Collider _eyeTrigger;

    #endregion

    #region ===属性===

    #endregion

    #region ===Unity事件=== 快捷键： Ctrl + Shift + M /Ctrl + Shift + Q  实现

    protected override void Awake()
    {
        base.Awake();
    }

    #endregion

    #region ===方法===

    public override void InitShip(ShipType shipType, SpacePort spawnPort)
    {
        base.InitShip(shipType, spawnPort);
        _moveCollider.enabled = false;
        _eyeTrigger.enabled = false;
    }

    public override void TakeOff(Vector3 destination)
    {
        base.TakeOff(destination);
        _moveCollider.enabled = true;
        _eyeTrigger.enabled = true;
    }

    #endregion
}
