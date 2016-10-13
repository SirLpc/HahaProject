using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ErrorManager : MonoBehaviour {

    public ErrorPanel errorPanel;

    
	
	// Update is called once per frame
	void Update () {
        if (errorPanel.gameObject.activeSelf || GameData.errors.Count==0) {
            return;
        }
        ErrorModel model = GameData.errors[0];
        GameData.errors.RemoveAt(0);
        errorPanel.error(model.text, model.function);
	}
}
