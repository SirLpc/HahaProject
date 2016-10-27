using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AttackShipController : ShipControlBase
{
    #region ===字段===

    [SerializeField]
    private Collider _moveCollider ;
    [SerializeField]
    private Collider _eyeTrigger;
    [SerializeField]
    private Transform _render;

    [SerializeField]
    private GameObject _bulletPref;

    private IEnumerator _attakEnumerator;

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

    public override void InitShip(ShipType shipType, SpacePort spawnPort, bool isEnemy = false)
    {
        base.InitShip(shipType, spawnPort, isEnemy);
        //todo 打开注释，因为这是为测试留的
        //_moveCollider.enabled = false;
        _eyeTrigger.enabled = false;
    }

    public override void TakeOff(Vector3 destination)
    {
        base.TakeOff(destination);
        _moveCollider.enabled = true;
        _eyeTrigger.enabled = true;
    }

    public void ReadyToAttack(Transform target)
    {
        _continueDir = _body.velocity.normalized;
        _body.velocity = Vector3.zero;

        ShipState = ShipState.ATTACKING;

        var dir = (target.position - transform.position).normalized;
        var cross = Vector3.Cross(dir, transform.up);
        var angle = Vector3.Angle(dir, transform.up);
        if (cross.z > 0f)
            angle *= -1;
        transform.Rotate(0, 0, angle);

        _attakEnumerator = RepeatAttack(target);
        StartCoroutine(_attakEnumerator);
    }

    private IEnumerator RepeatAttack(Transform target)
    {
        Attack(target);

        while (target)
        {
            yield return new WaitForSeconds(1f);
            Attack(target);
        }
    }

    private void Attack(Transform target)
    {
        GameObject bullet = (GameObject)Instantiate(_bulletPref, transform.position, transform.rotation);
        var bs = bullet.GetComponent<SpaceBullet>();
        bs.Init(target, OnEnemyDestroyed);
    }

    private void OnEnemyDestroyed()
    {
        StopCoroutine(_attakEnumerator);
        _body.AddForce(_continueDir * _speed);
        ShipState = ShipState.FLYING;
    }

    protected override void InitEnemy()
    {
        base.InitEnemy();

        var sve = _moveCollider.gameObject.AddComponent<ShipDisibleEye>();
        sve.Init(_render);
    }

    protected override void InitFriend()
    {
        base.InitFriend();

        var sde = gameObject.AddComponent<ShipVisibleEye>();
        sde.Init(_render);
    }

    #endregion
}
