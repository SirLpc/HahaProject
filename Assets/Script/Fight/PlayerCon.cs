﻿using UnityEngine;
using System.Collections;
using OneByOne;

public class PlayerCon : MonoBehaviour
{

    public GameObject EyeRange;
    public GameObject hpObj;
    public MrpgcKeyboardMovementController moveController;
    public PlayerCombatController combatController;
    public PlayerStatController stateController;

    protected NavMeshAgent agent=null;

    protected Animator anim;

    public bool IsSelf { protected set; get; }

    private FightPlayerModel data;

    public enum PlayerState
    {
        IDLE, RUN, ATK, NOCON
    }

    protected PlayerState myState = PlayerState.IDLE;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
    }

   public FightPlayerModel Data
    {
        get {return data;}
        set { data = value; }
    }

    public void StartMove(MoveDTO dto)
    {
        moveController.NetWorkInputGet(dto);
        return;

        myState = PlayerState.RUN;
        if (agent==null) agent = GetComponent<NavMeshAgent>();
        agent.ResetPath();
        agent.SetDestination(new Vector3(dto.x, dto.y, dto.z));
        anim.SetInteger("state", 1);
    }

    public void setTag(string tag)
    {
        gameObject.tag = tag;

        if(EyeRange)
            EyeRange.tag = tag;
    }
    // Update is called once per frame
    void Update()
    {
        hpChange();
        switch (myState)
        {
            case PlayerState.IDLE:
                break;
            case PlayerState.RUN:
                if (agent.pathStatus==NavMeshPathStatus.PathComplete&& agent.remainingDistance <= 0 && !agent.pathPending)
                {
                    myState = PlayerState.IDLE;
                    anim.SetInteger("state", 0);
                }
                else
                {
                    if (agent.isOnOffMeshLink)
                    {
                        agent.CompleteOffMeshLink();
                    }
                }
                break;
            case PlayerState.ATK:
                break;
            case PlayerState.NOCON:
                break;
            default:
                break;
        }
    }

    void hpChange()
    {
        if (hpObj)
        {
            Vector3 tar = GameData.worldToTarget(transform.position);
            
            if (hpObj.transform.localPosition != tar)
            {
              
                hpObj.transform.localPosition = tar;
            }
        }
    }

    public void setHp(GameObject hp, Transform par)
    {
        hpObj = hp;
        hp.transform.parent = par;
        hp.transform.localScale = Vector3.one;
        hp.transform.localPosition = GameData.worldToTarget(transform.position);
    }

    protected void damage(int value) {
        data.hp -= value;
        if (data.hp < 0) data.hp = 0;
        if (data.hp > data.maxHp) data.hp = data.maxHp;

        stateController.TakeDamage(value);
        //hpObj.GetComponent<HpProcess>().hpChange((float)data.hp/data.maxHp);
    }
}
