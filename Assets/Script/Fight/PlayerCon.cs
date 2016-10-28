#define USING_RPG 

using UnityEngine;
using System.Collections;
using OneByOne;
using System;

public class PlayerCon : MonoBehaviour
{

    public GameObject EyeRange;
    public GameObject hpObj;
#if !USING_RPG
    public MrpgcKeyboardMovementController moveController;
    public PlayerCombatController combatController;
    public PlayerStatController stateController;
#endif
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
        #if !USING_RPG
        moveController.NetWorkInputGet(dto);
#endif
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

           #if !USING_RPG
        stateController.TakeDamage(value);
#endif
        //hpObj.GetComponent<HpProcess>().hpChange((float)data.hp/data.maxHp);
    }
}
