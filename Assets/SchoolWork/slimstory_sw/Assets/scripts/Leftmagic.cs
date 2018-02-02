using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leftmagic : MonoBehaviour {
	private bool lifeFrug = true;
	public float speed = 5;
	// Use this for initialization
	void Start () {
		


	}
	
	// Update is called once per frame
	void Update () {
			GetComponent<Rigidbody2D> ().velocity = -transform.right.normalized * speed;
		
		bom ();
	}

	void bom(){
		//yield return new WaitForSeconds(1.0f);		// 2.5秒、処理を待機.
		if(lifeFrug)
			Destroy(gameObject,1.0f);
	}


	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag == "Player") {
			Destroy (gameObject);
		}
	}
}
