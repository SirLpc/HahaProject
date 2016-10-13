using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using OneByOne;

public class FightSkillGrid : MonoBehaviour ,IPointerEnterHandler,IPointerExitHandler{

    public Image mask;
    public Image background;
    public Button upBtn;

    private FightSkill skill;

    private int maxTime;
    private float nowTime;
    private bool sclied = false;

    void Update()
    {
        if (sclied)
        {
            nowTime -= Time.deltaTime;
            if (nowTime < 0) { nowTime = 0; sclied = false; mask.gameObject.SetActive(false); }
            mask.fillAmount = nowTime / maxTime;
        }

    }
    public void setIcon(int hero, FightSkill skill)
    {
        this.skill = skill;
       Sprite sp= ResourceLoad.getSkillIcon(hero+"_"+skill.id);
       mask.sprite = sp;
       background.sprite = sp;
       mask.gameObject.SetActive(true);
    }

    public void skillChange(FightSkill skill)
    {
        this.skill = skill;
    }

    public void setMask(int time)
    {
        if (time == 0)
        {
            
            if (!sclied && skill.level>0)
            {
                mask.gameObject.SetActive(false);
                return;
            }
            else
            {
                mask.gameObject.SetActive(true);
                return;
            }
        }
        maxTime = time;
        nowTime = maxTime;
        mask.gameObject.SetActive(true);
        sclied = true;
        
    }

    public void btnState(bool value)
    {
        upBtn.gameObject.SetActive(value);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        FightScene.skillTip.active(skill,transform.localPosition+new Vector3(0,133));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        FightScene.skillTip.disnable();
    }

    public void upClick() {
        NetWorkScript.Instance.write(Protocol.TYPE_FIGHT, 0, FightProtocol.SKILL_UP_CREQ, skill.id);
    }
}
