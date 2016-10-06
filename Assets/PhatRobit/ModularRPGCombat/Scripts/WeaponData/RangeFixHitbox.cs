using UnityEngine;
using System.Collections.Generic;

public class RangeFixHitbox : MonoBehaviour
{
    private List<Transform> _inSightEnemys = new List<Transform>();

    public Transform TryGetTargetEnemy()
    {
        if (_inSightEnemys.Count <= 0)
            return null;

        var minTr = _inSightEnemys[0];
        if(minTr == null)
        {
            _inSightEnemys.Remove(minTr);
            return TryGetTargetEnemy();
        }

        if (_inSightEnemys.Count == 1)
            return minTr;

        var minAngle = Vector3.Angle((minTr.position - transform.position), transform.forward);
        for (int i = 1; i < _inSightEnemys.Count; i++)
        {
            var curTr = _inSightEnemys[i];
            if (curTr == null)
                continue;
            var curAngle = Vector3.Angle(curTr.position - transform.position, transform.forward);
            if (curAngle >= minAngle)
                continue;
            minAngle = curAngle;
            minTr = _inSightEnemys[i];
        }

        // Show mercy to death
        EnemyStatController esc = minTr.GetComponent<EnemyStatController>();
        if(esc == null || !esc.Alive)
        {
            _inSightEnemys.Remove(minTr);
            return TryGetTargetEnemy();
        }

        return minTr;
    }

    private void SortInSightEnemys()
    {
        _inSightEnemys.Sort((x, y) =>
        {
            Vector3 d1 = x.position - transform.position;
            float a1 = Vector3.Angle(d1, transform.forward);
            Vector3 d2 = y.position - transform.position;
            float a2 = Vector3.Angle(d2, transform.forward);

            return (int)(a1 - a2);
        });
    }

    //public Transform target;
    //public float angle = 60f;
    //void Update()
    //{
    //    Vector3 direction = target.position - transform.position;
    //    if (Vector3.Angle(direction, transform.forward) < angle)
    //            print("I got you!");
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(Tags.enemy))
            return;

        var ot = other.transform;
        if (_inSightEnemys.Contains(ot))
            return;

        _inSightEnemys.Add(ot);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(Tags.enemy))
            return;

        var ot = other.transform;
        if (!_inSightEnemys.Contains(ot))
            return;

        _inSightEnemys.Remove(ot);
    }
}