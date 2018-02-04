using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSlimeController : MonoBehaviour
{
    [SerializeField] CharacterControllerJellyMesh charaMeshController;
    [SerializeField] CharacterColliderController colliderController;
    void OnEnable()
    {
        if (charaMeshController == null)
        {
            charaMeshController = GetComponent<CharacterControllerJellyMesh>();
            if (charaMeshController == null)
            {
                Debug.LogError("could not find meshCtrl");
                return;
            }
        }

        if (colliderController == null)
        {
            Debug.LogWarning("could not fine CollderController ad new one");
            colliderController = ResourcesManager.Get().CreateInstance(FieldObjectIndex.SlimeCharacterCollderController).GetComponent<CharacterColliderController>();
        }
        else
        {
            //colliderController.onTriggerEnterFix = OnTriggerEnterFix;
        }
    }
    void Update()
    {
        if (charaMeshController != null)
        {
            charaMeshController.UpdateCharacterInput();
        }
    }
    public CharacterControllerJellyMesh GetCharaMeshController()
    {
        return charaMeshController;
    }
}
