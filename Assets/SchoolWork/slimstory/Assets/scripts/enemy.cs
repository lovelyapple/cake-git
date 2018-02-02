using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour {
	
	public GameObject prefab_Leftmagic;
	public GameObject prefab_Rightmagic;

	public enum EnemyMode{
		Walk = 0,
		Attack,
		Freeze
	};

	int Mode;

	public bool moveLeft;// 左右反転フラグ
	public float move = 3.0f;
	private Vector2 startpos;

	public int count = 60;			// 発動間隔
	public int frame = 0;				// フレーム数


	public float counttime = 0.0f;		// 時間カウント
	public int freezetime = 60;		// フリーズ時間


	// Use this for initialization
	void Start () {
		Mode = 0;
		moveLeft = false;
		startpos = new Vector2(transform.position.x,transform.position.y);
	}
	//
	// Update is called once per frame
	void Update () {
		switch (Mode) {
		case 0:	// 通常状態
			// 右へ移動
			if (moveLeft == false) {				
				transform.Translate (0.01f, 0, 0);
				if (startpos.x + move <= transform.position.x) {
					moveLeft = true;
				}
			} else {// 	左へ移動
				transform.Translate (-0.01f, 0, 0);
				if (startpos.x - move >= transform.position.x) {
					moveLeft = false;
				}
			}

			break;
		case 1:	// 攻撃状態
			frame++;
			if (frame % count == 0) {
				magic ();
			}
			
			break;
		case  2:	// 攻撃後状態
			frame++;

			if (frame % freezetime == 0) {
				Mode = 0;
			}
			break;
		}
	}

	//
	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.tag == "Player") {
			Destroy (gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag == "Player") {
			Mode = 1;
		}
	}

	void OnTriggerExit2D(Collider2D col){
		if (col.gameObject.tag == "Player") {
			Mode = 2;
		}
	}

	//画像反転
	void FixedUpdate()
	{
		if (Mode==0){// 通常状態時

			//右を向いたとき、もしくは左を向いたとき
			if(moveLeft || !moveLeft)
			{
				//右を向いているかどうかで更新する
				transform.localScale = new Vector3((moveLeft ? 0.7f : -0.7f), 0.7f,1);
			}
		}
	}

	public void magic(){
		if (moveLeft) {
			Vector2 pos = transform.position + transform.TransformDirection (Vector2.left);
			GameObject magic = Instantiate (prefab_Leftmagic, pos, Quaternion.identity) as GameObject;
		} else if (!moveLeft) {
			Vector2 pos = transform.position + transform.TransformDirection (Vector2.right);
			GameObject magic = Instantiate (prefab_Rightmagic, pos, Quaternion.identity) as GameObject;
		}

	}
}
