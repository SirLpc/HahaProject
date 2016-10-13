using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HpProcess : MonoBehaviour {

    public Image hpSilder;
    public Text nameTex;

    public Slider hp;
    public Slider mp;

    public void setColorAndName(Color hp,string name)
    {
        hpSilder.color = hp;
        nameTex.text = name;
    }

    public void hpChange(float value) {
        hp.value = value;
    }
    public void mpChange(float value)
    {
        mp.value = value;
    }
}
