using UnityEngine;
using System.Collections;
using OneByOne;
using UnityEngine.UI;
using System.Text;

public class MainScene : MonoBehaviour {

    public CreateUserPanel cup;
    public GameObject mask;
    public Text tex;
    public Text btnTex;
    public GameObject matchWindow;
	IEnumerator Start () {
        //   DisnableUtil.disnable(1024, 768);

	    yield return null;

        if (GameData.user == null)
        {
            NetWorkScript.Instance.write(Protocol.TYPE_USER, 0, UserProtocol.GET_CREQ, null);
        }
        yield break;

        matchWindow.SetActive(false);
        refreshMain();
        if (GameData.user == null)
        {
            mask.SetActive(true);
            NetWorkScript.Instance.write(Protocol.TYPE_USER, 0, UserProtocol.GET_CREQ, null);
        }
        
	}

    void refreshMain() {
        if (GameData.user != null) {
            StringBuilder sb = new StringBuilder();
            sb.Append(GameData.user.name);
            sb.Append("  等级:");
            sb.Append(GameData.user.level);
            tex.text =  sb.ToString();
        }

    }

    void closeMask()
    {
        mask.SetActive(false);
    }

    void openCreate()
    {
        var n = LocalPlayerPrefabs.GetName();
        if (!string.IsNullOrEmpty(n))
            cup.AutoCreate(n);
        else
            cup.open();
    }

    void closeCreate()
    {
        cup.close();
    }
    
    public void AutoMatch()
    {
        NetWorkScript.Instance.write(Protocol.TYPE_MATCH, 0, MatchProtocol.ENTER_CREQ, null);
    }

   public void match() {
        if (btnTex.text == "开始排队")
        {
            //发送排队请求
            btnTex.text = "取消排队";
            matchWindow.SetActive(true);
            NetWorkScript.Instance.write(Protocol.TYPE_MATCH, 0, MatchProtocol.ENTER_CREQ, null);
        }
        else {
            //发送取消排队请求
            btnTex.text = "开始排队";
            matchWindow.SetActive(false);
            NetWorkScript.Instance.write(Protocol.TYPE_MATCH, 0, MatchProtocol.LEAVE_CREQ, null);
        }
    }
}
