using UnityEngine;
using System.Collections;
using OneByOne;
using UnityEngine.SceneManagement;

public class MatchHandler : MonoBehaviour,IHandler {

    public void MessageReceive(SocketModel model)
    {
        switch (model.command) { 
            case MatchProtocol.ENTER_SELECT_BRO:
                SceneManager.LoadScene(2);
                break;
        }
    }
}
