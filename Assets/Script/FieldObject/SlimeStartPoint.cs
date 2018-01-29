using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeStartPoint : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }
    }
	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update()
	{
		//todo 何かをする？
	}
}
