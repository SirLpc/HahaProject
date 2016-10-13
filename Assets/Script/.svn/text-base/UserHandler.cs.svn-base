using UnityEngine;
using System.Collections;
using OneByOne;

public class UserHandler : MonoBehaviour,IHandler {


    public void MessageReceive(SocketModel model)
    {
        switch (model.command)
        {
            case UserProtocol.GET_SRES:
                getResult(model.getMessage<UserDTO>());
                break;
            case UserProtocol.ONELINE_SRES:
                online(model.getMessage<UserDTO>());
                break;
            case UserProtocol.CREATE_SRES:
                create(model.getMessage<bool>());                
                break;
        }
    }

    void getResult(UserDTO value)
    {
        if (value == null)
        {
            SendMessage("openCreate");
        }
        else
        {
            NetWorkScript.Instance.write(Protocol.TYPE_USER, 0, UserProtocol.ONLINE_CREQ, null);
        }
    }

    void online(UserDTO user) {
        GameData.user = user;
        SendMessage("closeMask");
        SendMessage("refreshMain");
        GameData.errors.Add(new ErrorModel("HOHO登录成功，开始初始化"));
       //内容初始化
    }

    void create(bool value)
    {
        if (value)
        {
            NetWorkScript.Instance.write(Protocol.TYPE_USER, 0, UserProtocol.ONLINE_CREQ, null);
            SendMessage("closeCreate");
        }
        else
        {
            SendMessage("openCreate");
            GameData.errors.Add(new ErrorModel("创建失败，可能名称重复了"));
        }
    }
}
