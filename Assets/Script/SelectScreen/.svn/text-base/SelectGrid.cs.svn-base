using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using OneByOne;

public class SelectGrid : MonoBehaviour {

    public Image head;
    public Text nameText;
    public Sprite nullSP;

    private Image img;

    void Start() {
        img = GetComponent<Image>();
    }

    public void setData(SelectModel model) {
        nameText.text = model.name;
        if (model.entered)
        {
            if (model.heroId == -1)
            {
                head.sprite = nullSP;
            }
            else {
                head.sprite = ResourceLoad.getHead(model.heroId+"");
            }
        }
        else {
            head.sprite = ResourceLoad.getHead("Image 19");
        }
        if (model.ready)
        {
            selected();
        }
        else {
            img.color = Color.white;
        }
    }

    public void selected() {
        img.color = new Color(242, 0, 0, 255);
    }
}
