using UnityEngine;
using System.Collections;
using OneByOne;
using UnityEngine.UI;

public class SkillTip : MonoBehaviour {
    public Text text;
    public Text info;
    public Image icon;

    public void active(FightSkill skill,Vector3 position)
    {

        icon.sprite = ResourceLoad.getSkillIcon(FightScene.player.heroId + "_" + skill.id);
        text.text = skill.name;
        info.text = skill.info;
        gameObject.transform.localPosition = position;
        gameObject.SetActive(true);
    }
    public void disnable()
    {
        gameObject.SetActive(false);
    }
}
