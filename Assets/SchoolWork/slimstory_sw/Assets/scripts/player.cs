using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class player : MonoBehaviour {
	private bool facingRight = true;// 左右反転フラグ
	public GameObject prefab_fslime;// 救助スライムオブジェクト
	public GameObject prefab_juel;//宝石オブジェクト
	public float y;// 伸びる高さ
	public float x;// 伸びる幅
	private bool ok;// 通常操作フラグ

	public Text HPtext;//テキスト用変数
	public int HP = 1;// HP

	public float defalutScale_x = 0.3f;
	public float defalutScale_y = 0.3f;



	// Use this for initialization
	void Start () {
		ok = true;
		HPtext.text = "HP : ";
	}
	
	// Update is called once per frame
	void Update () {
		// HP更新
		HPtext.text = "HP : " + HP.ToString();

		if (HP == 0) {
			Destroy (gameObject);
		}

		// 通常状態
		if (col.ch == false) {
			// 左右移動
			if (Input.GetKey (KeyCode.LeftArrow)) {
				if (transform.position.x >= -6.5f) {
					transform.Translate (-0.1f, 0, 0);
				}
			}
			if (Input.GetKey (KeyCode.RightArrow)) {
				if (transform.position.x <= 6.5f) {
					transform.Translate (0.1f, 0, 0);
				}
			}
		}

		if (col.ch == false && ok == true) {
			// 排出
			if(HP >= 2){
				if (Input.GetKeyDown (KeyCode.N)) {
					fslime ();
					HP -= 1;
					x -= 0.05f;
					y -= 0.05f;
					transform.localScale = new Vector2 (x , y);
				}
			}

			// 伸びる
			if (Input.GetKey (KeyCode.Space)) {
				if (transform.localScale.y <= 1.4f) {
					y += 0.05f;
					transform.localScale = new Vector2 (x, y);
				}
			}
			//縮む
			if (Input.GetKeyUp (KeyCode.Space)) {
				for (; transform.localScale.y >= 0.3f;) {
					y -= 0.05f;
					transform.localScale = new Vector2 (x, y);
				}
			}

			// @HP調整
			if (Input.GetKeyDown (KeyCode.S)) {
				HP += 1;
				x += 0.05f;
				y += 0.05f;
				transform.localScale = new Vector2 (x , y);
			}
		} 

		// 引っ付き状態
		else if(col.ch == true){
			// 操作不可
			ok = false;
			//transform.localScale = new Vector3 (x, (col.ch ? -y : y), 0.5f);

			if (transform.localScale.y >= 0.3f) {
				y -= 0.02f;
				transform.localScale = new Vector2 (x, y);
			}
			//transform.localScale =  new Vector2(0.5f,0.5f);
			// 座標調整
			Vector2 pos = transform.position;
			pos.y = 0.0f;
			transform.position = pos;

			// 回転(離す)
			if (Input.GetKey (KeyCode.H)) {
				//transform.Rotate (new Vector3 (0, 0, 5));
				col.ch = false;
				transform.localScale = new Vector2 (0.3f, 0.3f);
			}


		}
	}


	// アタリ判定
	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.tag == "down") {		// 操作制限
			ok = true;
		}

		if (col.gameObject.tag == "slime") {	// 仲間スライム
			HP += 1;
			/*x = x +(HP * 0.025f);
			y = y + (HP * 0.025f);*/
			x += 0.05f;
			y += 0.05f;
			transform.localScale = new Vector2 (x , y);
		}		
	}

	void OnTriggerEnter2D(Collider2D col){
		// 球が当たれば
		if (col.gameObject.tag == "magic") {

			if (HP > 1) {
				juel ();
			}
			//Destroy (gameObject);
			HP -= 1;
			/*x = x + -(HP * 0.05f);
			y = y + -(HP * 0.05f);*/
			x -= 0.05f;
			y = 0.05f;
			transform.localScale = new Vector2 (x, y);
		}
	}



	//画像反転
	void FixedUpdate()
	{
		if (col.ch == false && ok == true){
		float h = Input.GetAxis ("Horizontal");

		//右を向いていて、左の入力があったとき、もしくは左を向いていて、右の入力があったとき
			if((h > 0 && !facingRight) || (h < 0 && facingRight))
			{
				//右を向いているかどうかを、入力方向をみて決める
				facingRight = (h > 0);
				//localScale.xを、右を向いているかどうかで更新する
				transform.localScale = new Vector3((facingRight ? x : -x), y,0.5f);
			}
		}
	}

	// 球発射
	private void juel(){
		Vector2 pos = transform.position + transform.TransformDirection (Vector2.up);
		GameObject juel = Instantiate (prefab_juel, pos, Quaternion.identity) as GameObject;
	}

	private void fslime(){
		Vector2 pos = transform.position + transform.TransformDirection (Vector2.up);
		GameObject fslime = Instantiate(prefab_fslime, pos, Quaternion.identity) as GameObject;
	}
}
