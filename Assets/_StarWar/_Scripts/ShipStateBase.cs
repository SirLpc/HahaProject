using System;
using UnityEngine;

public class ShipStateBase : MonoBehaviour
{
    private int _health = 100;
    private int _maxHealth = 100;

    public bool IsAlive { private set; get; }

    //todo kill test script
    private void Start()
    {
        Init();
    }

    public void Init()
    {
        _health = _maxHealth;
        IsAlive = true;
    }

    /// <summary>
    /// 执行伤害，返回是否死亡
    /// </summary>
    /// <param name="damage">true == dead, fale == still alive</param>
    /// <returns></returns>
    public bool TakeDamage(int damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            _health = 0;
            IsAlive = false;
            DestroyShip();
        }

        return !IsAlive;
    }

    private void DestroyShip()
    {
        GameObject.Destroy(this.gameObject);
    }

}

