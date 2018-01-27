using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerJellyMesh : MonoBehaviour
{
    [SerializeField] CharacterData slimeData;
    [SerializeField] JellyMesh jellyMesh;
	[SerializeField] CharacterColliderController colliderController;
	public Vector3 testPos;
    void Start()
    {
        if (jellyMesh == null)
        {
            jellyMesh = GetComponent<JellyMesh>();

            if (jellyMesh == null)
            {
                Debug.LogError("could not fine JeelyMesh");
            }
        }

        if (slimeData == null)
        {
            Debug.Log("could not find slimeData");
        }

		if(colliderController == null)
		{
			Debug.LogWarning("could not fine CollderController ad new one");
		}
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
		if(jellyMesh != null && jellyMesh.CentralPoint != null)
		{
			testPos = jellyMesh.CentralPoint.transform.position;
		}
    }
}
