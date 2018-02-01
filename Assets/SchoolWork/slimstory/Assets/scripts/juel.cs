using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class juel : MonoBehaviour {
	public Rigidbody2D rb;
	private float paw_x, paw_y;
	// Use this for initialization
	void Start () {
		//StartCoroutine ("juel");	// コルーチン開始
		rb = GetComponent<Rigidbody2D>();
		paw_x = Random.Range (1.0f, 5.0f + 1);
		paw_y = Random.Range (5.0f, 8.0f + 1);

		rb.AddForce (Vector2.right * paw_x,ForceMode2D.Impulse);
		rb.AddForce (Vector2.up * paw_y,ForceMode2D.Impulse);
	}
	
	// Update is called once per frame
	void Update () {
		
		bom ();
	}

	void bom(){
		//yield return new WaitForSeconds(1.0f);		// 2.5秒、処理を待機.
		Destroy(gameObject,1.0f);
	}
}
