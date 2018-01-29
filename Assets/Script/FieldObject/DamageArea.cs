using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageArea : MonoBehaviour
{

    [SerializeField] MeshRenderer meshRenderer;
    // Use this for initialization
    void Start()
    {
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
	void OnCollisionStay(Collision other)
	{
		//if(other.gameObject.layer == )
	}
}
