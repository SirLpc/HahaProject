using UnityEngine;
using System.Collections;
using OneByOne;
using System.Collections.Generic;
using UnityEngine.UI;

public class FightHandler : MonoBehaviour,IHandler {


    private Dictionary<int, GameObject> heros = new Dictionary<int, GameObject>();
    private FightScene scene;

    void Start()
    {
        scene = GetComponent<FightScene>();        
    }

    public void MessageReceive(SocketModel model)
    {
        switch (model.command) { 
            case FightProtocol.FIGHT_BRO:
                fightStart(model.getMessage<FightRoomModel>());
                break;
            case FightProtocol.MOVE_BRO:
                move(model.getMessage<MoveDTO>());
                break;
            case FightProtocol.SKILL_UP_SRES:
                skillLevelUp(model.getMessage<FightSkill>());
                break;
            case FightProtocol.ATTACK_BRO:
                attack(model.getMessage<AttackDTO>());
                break;
            case FightProtocol.DAMAGE_BRO:
                damage(model.getMessage<DamageDTO>());
                break;
        }
    }

    private void damage(DamageDTO dto) {
        foreach (int[] item in dto.targetDamage)
        {
            GameObject target=heros[item[0]];
            //todo damage text effect
            //GameObject obj= Instantiate(ResourceLoad.getHpUp(), target.transform.position+Vector3.up*2, Camera.main.transform.rotation) as GameObject;
            //obj.GetComponent<HpUp>().setValue(item[1]);
            target.GetComponent<PlayerComponent>().con.damage(item[1]);
        }
    }

    private void attack(AttackDTO atk)
    {
        GameObject hero= heros[atk.id];
        List<GameObject> list=new List<GameObject>();
        foreach (int item in atk.target)
	    {
		     list.Add(heros[item]);
             Debug.Log("xxxx"+item);
	    }
        hero.GetComponent<PlayerComponent>().con.attack(list.ToArray());
    }

    private void skillLevelUp(FightSkill skill)
    {
        for (int i = 0; i < FightScene.player.skills.Length; i++)
        {
            if (FightScene.player.skills[i].id == skill.id)
            {
                FightScene.player.free -= 1;
                FightScene.player.skills[i] = skill;
                scene.refreshUI();
                return;
            }
        }
    }

    private void move(MoveDTO dto)
    {
        heros[dto.userId].SendMessage("StartMove", dto);
    }

    private void fightStart(FightRoomModel model) {
        foreach (FightPlayerModel item in model.teamOne)
        {
            if (item.id == GameData.user.id) {
                scene.myTeam = 1;
                break;
            }
        }

        if (scene.myTeam == 0) scene.myTeam = 2;

        foreach (FightPlayerModel item in model.teamOne)
        {
            addHero(1, item);
        }


        foreach (FightPlayerModel item in model.teamTwo)
        {
            addHero(2, item);
        }

    }

    private void addHero(int team,FightPlayerModel model) {
        GameObject o;
        if (team == 1)
        {
            o = (GameObject)Instantiate(ResourceLoad.getHeroModel(model.heroId), GameData.teamOneStart,Quaternion.identity);
        }
        else
        {
            o = (GameObject)Instantiate(ResourceLoad.getHeroModel(model.heroId), GameData.teamTwoStart, Quaternion.identity);
        }
        IHero pc = o.GetComponent<PlayerComponent>().con;
        pc.setData(model);
        //todo set hpbar, case now, we use gui statebar for quick start
        //GameObject hp = Instantiate<GameObject>(ResourceLoad.getBar());
        //pc.setHp(hp, scene.par);
        if (scene.myTeam == team)
        {
            o.layer = LayerMask.NameToLayer("visible");
            //hp.GetComponent<HpProcess>().setColorAndName(Color.green, model.name);
            Destroy(o.GetComponent<EnemyEye>());
            pc.setTag(Tags.FriendHero);
            if (model.id == GameData.user.id)
            {
                FightScene.player = model;
                scene.myHero = o;
                //todo set head portrait
                //scene.myHead.sprite = ResourceLoad.getHead(model.heroId.ToString());

                //scene.initUI();       //Mainly skill uis
                //scene.cameraReset();
                scene.InitFollowCamera(o);
            }
        }
        else
        {
            o.layer = LayerMask.NameToLayer("disible");
           
            pc.setTag(Tags.EnemyHero);
            //hp.GetComponent<HpProcess>().setColorAndName(Color.red, model.name);
        }
        heros.Add(model.id, o);        
       
    }
}
