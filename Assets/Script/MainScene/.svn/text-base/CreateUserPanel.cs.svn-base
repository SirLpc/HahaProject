using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using OneByOne;

public class CreateUserPanel : MonoBehaviour {

    public InputField nameField;

    public Button cbtn;

    public void open()
    {
        cbtn.enabled = true;
        gameObject.SetActive(true);        
    }

    public void close()
    {
        gameObject.SetActive(false);
    }

   public void createClick()
    {
        if (nameField.text.Length > 6 || nameField.text == string.Empty) { GameData.errors.Add(new ErrorModel("名称不合法")); return; }
        cbtn.enabled = false;
        NetWorkScript.Instance.write(Protocol.TYPE_USER, 0, UserProtocol.CREATE_CREQ, nameField.text);
    }
}
