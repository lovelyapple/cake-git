using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBase : MonoBehaviour
{
    [SerializeField] protected CharacterControllerJellyMesh charaMeshController;
    [SerializeField] protected CharacterColliderController colliderController;
    protected virtual void OnTriggerEnterCheckFix(Collider col) { }
    public uint friendId;
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

        if (colliderController == null)
        {
            Debug.LogWarning("could not fine CollderController ad new one");
            colliderController = ResourcesManager.Get().CreateInstance(FieldObjectIndex.SlimeCharacterCollderController).GetComponent<CharacterColliderController>();
        }
        else
        {
            colliderController.onTriggerEnterFix = OnTriggerEnterCheckFix;
        }
    }
    public void CreateJellyMesh(Action<JellyMesh> onFinished)
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
