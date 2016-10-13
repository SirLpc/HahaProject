using UnityEngine;
using System.Collections;
using OneByOne;

public class SelectHandler : MonoBehaviour, IHandler
{
    private SelectRoomDTO myRoom;

    public void MessageReceive(SocketModel model)
    {
        switch (model.command)
        {
            case SelectProtocol.ENTER_SRES:
                myEnter(model.getMessage<SelectRoomDTO>());
                break;
            case SelectProtocol.ENTER_BRO:
                otherEnter(model.getMessage<int>());
                break;
            case SelectProtocol.READY_BRO:
                ready(model.getMessage<SelectModel>());
                break;
            case SelectProtocol.ROOM_DESTORY_BRO:
                Application.LoadLevel(1);
                break;
            case SelectProtocol.SELECT_SRES:
                GameData.errors.Add(new ErrorModel("选择角色失败，请重新选择"));
                break;
            case SelectProtocol.SELECT_BRO:
                select(model.getMessage<SelectModel>());
                break;
            case SelectProtocol.START_FIGHT_BRO:
                SendMessage("activeMask");
                Application.LoadLevelAsync(3);
                break;
        }
    }

    private void ready(SelectModel sm)
    {
        if (sm.userId == GameData.user.id)
        {
            SendMessage("selectedHero");
        }
        if (myRoom.inTeam(sm.userId) == 1)
        {
            foreach (SelectModel item in myRoom.teamOne)
            {
                if (item.userId == sm.userId)
                {
                    item.heroId = sm.heroId;
                    item.ready = sm.ready;
                }
            }
        }
        else
        {
            foreach (SelectModel item in myRoom.teamTwo)
            {
                if (item.userId == sm.userId)
                {
                    item.heroId = sm.heroId;
                    item.ready = sm.ready;
                }
            }
        }
        
        SendMessage("refreshView", myRoom);

    }

    private void select(SelectModel sm) {

        if (myRoom.inTeam(sm.userId)==1) {
            foreach (SelectModel item in myRoom.teamOne)
            {
                if (item.userId == sm.userId)
                {
                    item.heroId=sm.heroId;
                }
            }
        }
        else
        {
            foreach (SelectModel item in myRoom.teamTwo)
            {
                if (item.userId == sm.userId)
                {
                    item.heroId = sm.heroId;
                }
            }
        }

        SendMessage("refreshView", myRoom);
    }

    private void myEnter(SelectRoomDTO room)
    {
        Debug.Log("接收到自己进入");
        SendMessage("closeMask"); 
        myRoom = room;
        SendMessage("refreshView", room);
        
    }
    private void otherEnter(int id)
    {
        if (myRoom == null) return;
        foreach (SelectModel item in myRoom.teamOne)
        {
            if (item.userId == id)
            {
                item.entered = true;
                SendMessage("refreshView", myRoom);
                return;
            }
        }
        foreach (SelectModel item in myRoom.teamTwo)
        {
            if (item.userId == id)
            {
                item.entered = true;
                SendMessage("refreshView", myRoom);
                return;
            }
        }
        
    }
}
