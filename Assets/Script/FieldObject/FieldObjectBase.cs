using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldObjectBase : MonoBehaviour {

	[SerializeField] MeshRenderer meshRenderer;
	public bool IsUnvisble;
	// Use this for initialization
	void Start () {
		if(IsUnvisble && meshRenderer != null)
		{
			meshRenderer.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
