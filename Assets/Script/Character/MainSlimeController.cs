using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSlimeController : JellyMeshController
{
    void OnEnable()
    {
        InputUtilityManager.Get().OnClickActionEvent = OnClickAction;
        InputUtilityManager.Get().OnClickJumpEvent = OnClickJump;
        InputUtilityManager.Get().OnInputMoveEvent = OnMove;
    }
    void OnDisable()
    {
        InputUtilityManager.Get().OnClickActionEvent = null;
        InputUtilityManager.Get().OnClickJumpEvent = null;
        InputUtilityManager.Get().OnInputMoveEvent = null;
    }
}
