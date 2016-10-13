using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 此脚本用于非友方单位的可视处理
/// 默认挂载，初始化数据的时候　友方单位移除
/// </summary>
public class EnemyEye : MonoBehaviour {
    public GameObject[] head;
    public GameObject[] me;
    private List<GameObject> list = new List<GameObject>();
    IHero con;

    void Start() {
        con=GetComponent<PlayerComponent>().con;
    }
	void Update () {
        if (list.Count > 0)
        {
            
            if (gameObject.layer != LayerMask.NameToLayer("visible")) {
                gameObject.layer=LayerMask.NameToLayer("visible");
               
            }
            if (me[0].layer != LayerMask.NameToLayer("visible")) {
                me[0].layer = LayerMask.NameToLayer("visible");
            }
            if (!me[1].activeSelf)
            {
                me[1].SetActive(true);
            }
            if (head[0].layer != LayerMask.NameToLayer("vHead"))
            {
                head[0].layer = LayerMask.NameToLayer("vHead");
            }
            if (head[1].layer != LayerMask.NameToLayer("vHead"))
            {
                head[1].layer = LayerMask.NameToLayer("vHead");
            }
            if (!con.getHpObj().activeSelf)
            {
                con.getHpObj().SetActive(true);
            }
            
        }
        else {
            if (gameObject.layer != LayerMask.NameToLayer("disible")) {
                gameObject.layer=LayerMask.NameToLayer("disible");
            }
            if (me[0].layer != LayerMask.NameToLayer("disible")) {
                me[0].layer = LayerMask.NameToLayer("disible");
            }
            if (me[1].activeSelf)
            {
                me[1].SetActive(false);
            }
            if (head[0].layer != LayerMask.NameToLayer("dHead"))
            {
                head[0].layer = LayerMask.NameToLayer("dHead");
            }
            if (head[1].layer != LayerMask.NameToLayer("dHead"))
            {
                head[1].layer = LayerMask.NameToLayer("dHead");
            }
            if (con.getHpObj().activeSelf)
            {
                con.getHpObj().SetActive(false);
            }
        }
	}

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "friendHero" || other.gameObject.tag == "friendsoldier" || other.gameObject.tag == "friendbuild") {
            list.Add(other.gameObject);
        }
    }
    void OnTriggerExit(Collider other) {
        if (list.Contains(other.gameObject)) {
            list.Remove(other.gameObject);
        }
    }
}
