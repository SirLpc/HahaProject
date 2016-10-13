using UnityEngine;
using System.Collections;
using OneByOne;

public class MessageManager : MonoBehaviour {

    IHandler login;
    IHandler user;
    IHandler match;
    IHandler select;
    IHandler fight;
	// Use this for initialization
    void Awake() {
        login = GetComponent<LoginHandler>();
        user = GetComponent<UserHandler>();
        match = GetComponent<MatchHandler>();
        select = GetComponent<SelectHandler>();
        fight = GetComponent<FightHandler>();
    }
	// Update is called once per frame
	void Update () {
        while (NetWorkScript.Instance.messageList.Count > 0)
        {
            SocketModel model = NetWorkScript.Instance.messageList[0];
            NetWorkScript.Instance.messageList.RemoveAt(0);
            StartCoroutine("MessageReceive", model);
        }
	}

    void MessageReceive(SocketModel model) {
        switch (model.type) { 
            case Protocol.TYPE_LOGIN:
                login.MessageReceive(model);
                break;
            case Protocol.TYPE_USER:
                user.MessageReceive(model);
                break;
            case Protocol.TYPE_MATCH:
                match.MessageReceive(model);
                break;
            case Protocol.TYPE_SELECT:
                select.MessageReceive(model);
                break;
            case Protocol.TYPE_FIGHT:
                fight.MessageReceive(model);
                break;
                
        }
    }

}
