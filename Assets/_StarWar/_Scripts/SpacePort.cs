using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class SpacePort : MonoBehaviour
{
    #region ===字段===

    [SerializeField] private float _spawnInterval = 5;
    [SerializeField] private GameObject[] _shipPref;

    private ShipControlBase _ship = null;

    #endregion

    #region ===属性===

    #endregion

    #region ===Unity事件=== 快捷键： Ctrl + Shift + M /Ctrl + Shift + Q  实现

    private void Start()
    {
        InvokeRepeating("SpawnRandomShip", 0, _spawnInterval);
    }

    #endregion

    #region ===方法===

    private void SpawnRandomShip()
    {
        if (_ship != null)
            return;

        var ran = Random.Range(0, _shipPref.Length);
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
        _ship.InitShip(type);
    }

    #endregion
}