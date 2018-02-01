using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goal : MonoBehaviour {

	private int count;
	public int MaxSlime;
	// Use this for initialization
	void Start () {
		count = 0;
	}
	
	// Update is called once per frame
	void Update () {
		// 子オブジェクトをカウント
		//MaxSlime = this.transform.childCount;


	}

	// アタリ判定 ゲームクリア判定
	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag == "fslime") {
			count++;
			if (MaxSlime == count) {
				Destroy (gameObject);
			}
		}

	}
}
