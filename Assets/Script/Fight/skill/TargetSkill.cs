using UnityEngine;
using System.Collections;
using OneByOne;

public class TargetSkill : MonoBehaviour {

    GameObject target;
    int skill;
    int atkId;
    public void setData(GameObject target,int skill,int id)
    {
        this.skill = skill;
        this.target = target;
        this.atkId = id;
    }

	// Update is called once per frame
	void Update () {
        if (target)
        {
            transform.position = Vector3.Lerp(transform.position, target.transform.position + Vector3.up, 0.5f);
            if (Vector3.Distance(transform.position,target.transform.position+Vector3.up)<0.1f)
            {
                //发送攻击 切销毁自身
                DamageDTO dto=new DamageDTO();
                dto.id = atkId;
                dto.skill = skill;
                dto.targetDamage = new int[][] {new int[]{target.GetComponent<PlayerCon>().Data.id} };
                NetWorkScript.Instance.write(Protocol.TYPE_FIGHT, -1, FightProtocol.DAMAGE_CREQ, dto);
                Destroy(gameObject);
            }
        }
	}
}
