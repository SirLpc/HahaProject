using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public delegate void ErrorResult();
public class ErrorPanel : MonoBehaviour {

    ErrorResult er = null;

    public Text text;

    public void error(string value, ErrorResult e = null) {
        text.text = value;
        er = e;
        gameObject.SetActive(true);
    }

    public void RightClick() {
        if (gameObject.activeSelf) {
            gameObject.SetActive(false);
        }
        if (er != null) {
            er();
        }
    }
}
