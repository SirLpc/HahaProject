using UnityEngine;
using System.Collections;
using OneByOne;

public class TargetSkill : SkillBase {

	// Update is called once per frame
	void Update () {
        if (target)
        {
            transform.position = Vector3.Lerp(transform.position, target.transform.position + Vector3.up, 0.5f);
            if (Vector3.Distance(transform.position,target.transform.position+Vector3.up)<0.1f)
            {
                //发送攻击 切销毁自身
                NetWorkSetDamage();

                Destroy(gameObject);
            }
        }
	}
}
