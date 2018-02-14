using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputUtilityManager : SingleToneBase<InputUtilityManager>
{

    public Action OnClickActionEvent;
    public Action OnClickJumpEvent;
    public Action OnClickPauseMenuEvent;
    public Action<Vector3> OnInputMoveEvent;

    void OnDisable()
    {
        OnClickActionEvent = null;
        OnClickJumpEvent = null;
        OnClickPauseMenuEvent = null;
        OnInputMoveEvent = null;
    }

    public void OnClickAction()
    {
        if (StateConfig.IsPausing) { return; }

        if (OnClickActionEvent != null)
        {
            OnClickActionEvent();
        }
    }
    public void OnClickJump()
    {
        if (StateConfig.IsPausing) { return; }

        if (OnClickJumpEvent != null)
        {
            OnClickJumpEvent();
        }
    }
    public void OnInputMove(Vector3 delta)
    {
        if (StateConfig.IsPausing) { return; }

        if (OnInputMoveEvent != null)
        {
            OnInputMoveEvent(delta);
        }
    }
    void Update()
    {
#if UNITY_STANDALONE
        if (StateConfig.IsPausing) { return; }
        if (Input.GetKey(KeyCode.A))
        {
            if (OnInputMoveEvent != null)
            {
                OnInputMoveEvent(new Vector3(-1f, 0, 0));
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            if (OnInputMoveEvent != null)
            {
                OnInputMoveEvent(new Vector3(1f, 0, 0));
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (OnClickJumpEvent != null)
            {
                OnClickJumpEvent();
            }
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            if (OnClickActionEvent != null)
            {
                OnClickActionEvent();
            }
        }
#endif
    }
}
