using UnityEngine;
using System.Collections.Generic;
using OneByOne;

public class SkillBase : MonoBehaviour
{
    #region ===字段===

    protected GameObject target;
    protected int skill;
    protected int atkId;

    #endregion

    #region ===属性===
    #endregion

    #region ===Unity事件=== 快捷键： Ctrl + Shift + M /Ctrl + Shift + Q  实现

    #endregion

    #region ===方法===

    public void setData(GameObject target, int skill, int id)
    {
        this.skill = skill;
        this.target = target;
        this.atkId = id;
    }

    protected void NetWorkSetDamage()
    {
        DamageDTO dto = new DamageDTO();
        dto.id = atkId;
        dto.skill = skill;
        dto.targetDamage = new int[][] { new int[] { target.GetComponent<PlayerCon>().Data.id } };
        NetWorkScript.Instance.write(Protocol.TYPE_FIGHT, -1, FightProtocol.DAMAGE_CREQ, dto);
    }

    #endregion
}
