using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSlimeController : JellyMeshController
{
    void Update()
    {
        if (jellyMesh != null && !StateConfig.IsPausing)
        {
            UpdateCharacterInput();
        }
    }
}
