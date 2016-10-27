using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class SpacePort : MonoBehaviour, ITeam
{
    #region ===字段===
    //todo kill test field
    public bool _isEnemy = false;

    [SerializeField]
    private float _spawnInterval = 5;
    [SerializeField]
    private GameObject[] _shipPref;

    private Transform _renderTr;

    private ShipControlBase _ship = null;

    #endregion

    #region ===属性===

    public bool IsEnemy { get; set; }

    #endregion

    #region ===Unity事件=== 快捷键： Ctrl + Shift + M /Ctrl + Shift + Q  实现

    private void Awake()
    {
        _renderTr = transform;
    }

    private void Start()
    {
        SpawnRandomShip();
        InitPort(_isEnemy);
    }

    #endregion

    #region ===方法===

    public void InitPort(bool isEnemy)
    {
        //这里有个BUG，因为SpacePort是root，因此Init时，如果走了Eye的Init相关方法，会把Ship的层也给改了
        //if (IsEnemy)
        //    InitEnemy();
        //else
        //    InitFriend();
    }

    public void OnShipTakeOff()
    {
        _ship = null;
        Invoke("SpawnRandomShip", _spawnInterval);
    }

    public void InitEnemy()
    {
        var sde = gameObject.AddComponent<ShipDisibleEye>();
        sde.Init(_renderTr);
    }

    public void InitFriend()
    {
        var sve = gameObject.AddComponent<ShipVisibleEye>();
        sve.Init(_renderTr);
    }

    private void SpawnRandomShip()
    {
        if (_ship != null)
            return;

        var ran = Random.Range(0, _shipPref.Length);
        //var ran = 0;
        SpawnShip((ShipType)ran);
    }

    private void SpawnShip(ShipType type)
    {
        if (_ship != null)
            return;

        var s = GameObject.Instantiate(_shipPref[(int)type]).transform;
        s.SetParent(transform, false);
        s.position = new Vector3(s.position.x, s.position.y, 0);
        _ship = s.GetComponent<ShipControlBase>();
        _ship.InitShip(type, this, false);
    }

    #endregion
}