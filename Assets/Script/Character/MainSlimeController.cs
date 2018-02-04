using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSlimeController : MonoBehaviour
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
    void Update()
    {
        if (charaMeshController != null)
        {
            charaMeshController.UpdateCharacterInput();
        }
    }
}
