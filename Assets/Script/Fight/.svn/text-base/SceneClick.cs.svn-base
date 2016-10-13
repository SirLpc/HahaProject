using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class SceneClick : MonoBehaviour, IPointerDownHandler
{

    public FightScene scene;

    public void OnPointerDown(PointerEventData eventData)
    {
        switch(eventData.pointerId){
            case PointerInputModule.kMouseRightId:
                scene.sceneRightClick(eventData.position);
                break;
            case PointerInputModule.kMouseLeftId:
                scene.sceneLeftClick(eventData.position);
                break;
        
        }
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            scene.cameraReset();
            return;
        }
        if (Input.mousePosition.x <= 5)
        {
            scene.cameraMove(1);
        }
        else if (Input.mousePosition.x >=Screen.width-5)
        {

            scene.cameraMove(2);
        } else
        if (Input.mousePosition.y <= 5)
        {

            scene.cameraMove(3);
        }
        else if (Input.mousePosition.y >= Screen.height-5)
        {

            scene.cameraMove(4);
        }
        else {
            scene.cameraMove(0);
        }
    }
}
