using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class col : MonoBehaviour {


	public Sprite Player_N;
	public Sprite Player_F;
	public static bool ch;


	// Use this for initialization
	void Start () {
		GetComponent<player> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (ch) {
			GetComponent<SpriteRenderer> ().sprite = Player_F;
		} else {
			GetComponent<SpriteRenderer> ().sprite = Player_N;
		}

	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag == "up" && Input.GetKey(KeyCode.Space)) {
			//Destroy (gameObject);
			ch = true;
			transform.localScale = new Vector2 (0.3f, 0.3f);
		}
	}

	//画像反転

}
