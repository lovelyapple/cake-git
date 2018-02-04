using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBase : MonoBehaviour
{
    [SerializeField] CharacterControllerJellyMesh charaMeshController;
    void OnEnable()
    {
        if (charaMeshController == null)
        {
            charaMeshController = GetComponent<CharacterControllerJellyMesh>();
            if (charaMeshController == null)
            {
                Debug.LogError("could not find meshCtrl");
            }
        }
    }
    public void CreateJellyMesh(Action<GameObject> onFinished)
    {
        if (charaMeshController == null) { return; }
        charaMeshController.CreateCharacter(onFinished);
    }

    public void AddForce(Vector3 force, bool centralPoinOnly)
    {
        if (charaMeshController == null) { return; }
        charaMeshController.JellyMeshAddForce(force, centralPoinOnly);
    }
}
