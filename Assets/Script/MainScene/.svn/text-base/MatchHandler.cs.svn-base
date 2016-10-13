using UnityEngine;
using System.Collections;
using OneByOne;

public class MatchHandler : MonoBehaviour,IHandler {

    public void MessageReceive(SocketModel model)
    {
        switch (model.command) { 
            case MatchProtocol.ENTER_SELECT_BRO:
                Application.LoadLevel(2);
                break;
        }
    }
}
