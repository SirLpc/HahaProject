using UnityEngine;
using System.Collections;

public class Hero1:PlayerCon,IHero {

    public GameObject effect;

    private GameObject[] targets;
    private Transform target;
    private int skillCode;

    public void attack(GameObject[] targets)
    {
        combatController.NetWorkAttackGet();
        return;

        if (PlayerState.RUN == myState)
        {
            agent.CompleteOffMeshLink(); ;
        }
        this.targets = targets;
        gameObject.transform.LookAt(targets[0].transform);
        myState = PlayerState.ATK;
        anim.SetInteger("state", 2);
    }

    public void attacked()
    {
        myState = PlayerState.ATK;
        foreach (GameObject item in targets)
        {
            GameObject go= (GameObject)Instantiate(effect, transform.position + new Vector3(0, 1),transform.rotation);
            go.GetComponent<TargetSkill>().setData(item,-1,Data.id);
        }
        myState = PlayerState.IDLE;
        anim.SetInteger("state", 0);
    }

    public void skill(int code, GameObject[] targets)
    {
        
    }

    public void skill(int code, Transform target)
    {
        
    }

    public void skilled()
    {
        
    }


    public OneByOne.FightPlayerModel getData()
    {
        return base.Data;
    }

    public void setData(OneByOne.FightPlayerModel model)
    {
        base.Data = model;
        base.stateController.NetSpawn(model);
        IsSelf = model.id == GameData.user.id;
    }

    public GameObject getHpObj()
    {
        return hpObj;
    }


    public PlayerCon.PlayerState getState()
    {
        return myState;
    }

    public void damage(int value)
    {
        base.damage(value);
    }
}
