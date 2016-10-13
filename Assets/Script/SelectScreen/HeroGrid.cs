using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeroGrid : MonoBehaviour {

    public Button btn;
    private int id = -1;
    public void setData(int hero) {
        id = hero;
        Image sp = btn.gameObject.GetComponent<Image>();
        sp.sprite = ResourceLoad.getHead(hero + "");
    }

   public void active() {
        btn.enabled = true;
    }
   public void deactive() {
        btn.enabled = false;
    }

   public void click() {
       if(id!=-1)
       SelectEventUtil.selectHero(id);
   }
}
