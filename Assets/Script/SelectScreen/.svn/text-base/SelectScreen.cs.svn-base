using UnityEngine;
using System.Collections;
using OneByOne;
using System.Collections.Generic;
using UnityEngine.UI;

public class SelectScreen : MonoBehaviour {

    public GameObject initMask;
    public Button startBtn;
    public SelectGrid[] left;
    public SelectGrid[] right;
    public Transform heroList;
    public GameObject heroBtn;
    public SelectRoomDTO room;
    private Dictionary<int, HeroGrid> myGrid = new Dictionary<int, HeroGrid>();

	void Start () {
        SelectEventUtil.selectHero = selectHero;
        initMask.SetActive(true);
        initHeroList();
        NetWorkScript.Instance.write(Protocol.TYPE_SELECT, 0, SelectProtocol.ENTER_CREQ, null);
        
	}

    void activeMask() {
        initMask.SetActive(true);
    }

    void initHeroList() {
        if (GameData.user == null) return;
        for(int i=0;i<GameData.user.heroList.Count ;i++)
        {
            int id= GameData.user.heroList[i];
            GameObject g= Instantiate<GameObject>(heroBtn);
            HeroGrid hg = g.GetComponent<HeroGrid>();
            hg.setData(id);
            RectTransform rtf = g.transform as RectTransform;
            rtf.parent = heroList;
            rtf.localScale = Vector3.one;
            rtf.localPosition = new Vector3(i % 10 * 44 + 25, -i / 10 * 44 + 3);
            myGrid.Add(id, hg);
        }
    
    }

    void refreshHeroList(SelectRoomDTO dto)
    {
        room = dto;
        int team = dto.inTeam(GameData.user.id);
        List<int> selected = new List<int>();
        if (team == 1)
        {
            for (int i = 0; i < dto.teamOne.Length; i++)
            {
                if (dto.teamOne[i].heroId!=-1)
                selected.Add(dto.teamOne[i].heroId);
            }
        }
        else {
            for (int i = 0; i < dto.teamTwo.Length; i++)
            {
                if (dto.teamTwo[i].heroId != -1)
                selected.Add(dto.teamTwo[i].heroId);
            }
        }
        foreach (int item in myGrid.Keys)
        {
            if (selected.Contains(item) ||!startBtn.enabled)
            {
                myGrid[item].deactive();
            }
            else {
                myGrid[item].active();
            }
        }
    }

    void closeMask() {
        initMask.SetActive(false);
    }

    void refreshView(SelectRoomDTO dto) {
        int team = dto.inTeam(GameData.user.id);
        if (team == 1) {
            for (int i = 0; i < dto.teamOne.Length; i++)
            {
                left[i].setData(dto.teamOne[i]);
            }
            for (int i = 0; i < dto.teamTwo.Length; i++)
            {
                right[i].setData(dto.teamTwo[i]);
            }
        }
        else if (team == 2) {
            for (int i = 0; i < dto.teamOne.Length; i++)
            {
                right[i].setData(dto.teamOne[i]);
            }
            for (int i = 0; i < dto.teamTwo.Length; i++)
            {
                left[i].setData(dto.teamTwo[i]);
            }
        }
        refreshHeroList(dto);
    }

    void selectedHero() {
        startBtn.enabled = false;
    }

    public void clickStart() {
        int team = room.inTeam(GameData.user.id);
        SelectModel[] sm;
        if (team == 1)
        {
            sm = room.teamOne;
        }
        else {
            sm = room.teamTwo;
        }
        List<int> last = new List<int>();
        bool mySelected = false;
        foreach (SelectModel item in sm)
        {
            if (item.heroId != -1){ 
                if (item.userId == GameData.user.id) {
                    mySelected = true;
                    break;
                }
                last.Add(item.heroId);
            }            
        }
        if (!mySelected) {
            List<int> temp = new List<int>();
            foreach (int item in GameData.user.heroList)
            {
                if (last.Contains(item)) continue;
                temp.Add(item);
            }
            NetWorkScript.Instance.write(Protocol.TYPE_SELECT, 0, SelectProtocol.SELECT_CREQ, temp[Random.Range(0, temp.Count - 1)]);
        }

        NetWorkScript.Instance.write(Protocol.TYPE_SELECT, 0, SelectProtocol.READY_CREQ, null);
    }

    public void selectHero(int id) {
        if(startBtn.enabled)
        NetWorkScript.Instance.write(Protocol.TYPE_SELECT, 0, SelectProtocol.SELECT_CREQ, id);
    }
}
