using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : WindowBase {
	public Sprite Howplay;
	public Sprite HowgGame;
	// Use this for initialization

	private int Mode = 0;
	void Update(){
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			if (Mode < 1) {
				Mode += 1;
			}
		}

		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			if (Mode > 0) {
				Mode -= 1;
			}
		}

		switch(Mode){
		case 0:
			gameObject.GetComponent<Image> ().sprite = Howplay;
			break;

		case 1:
			gameObject.GetComponent<Image> ().sprite = HowgGame;
			break;

		}
	}

}
