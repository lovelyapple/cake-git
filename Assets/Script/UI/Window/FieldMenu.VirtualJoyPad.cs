﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public partial class FieldMenu
{
    [SerializeField] Image targetImage;
    Vector3? startPoint;
    Vector3 calPos;
    Vector3 nowPos;
    float? _maxLimitRange;
    float maxLimitRange
    {
        get
        {
            if (!_maxLimitRange.HasValue)
            {
                _maxLimitRange = GameSettingTable.JoyPadMaxRange;
            }
            return _maxLimitRange.Value;
        }
    }
    float maxDispeareTime = 3.0f;//todo
    Coroutine coroutineRunFadeOut;
    public void OnPointDown()
    {
#if UNITY_STANDALONE
        startPoint = Input.mousePosition; 
#else
        var t = Input.GetTouch(0);
        var Vec = new Vector3(t.position.x, t.position.y, 0);
        startPoint = Vec;
#endif

        if (coroutineRunFadeOut != null)
        {
            StopCoroutine(coroutineRunFadeOut);
            coroutineRunFadeOut = null;
        }

        var col = targetImage.color;
        col.a = 1f;
        targetImage.color = col;
        targetImage.transform.position = startPoint.Value;
    }
    public void OnDrag()
    {
        if (!startPoint.HasValue) { return; }

#if UNITY_STANDALONE
        nowPos = Input.mousePosition;
#else
        var t = Input.GetTouch(0);
        var Vec = new Vector3(t.position.x, t.position.y, 0);
        nowPos = Vec;
#endif
        var diff = nowPos - startPoint.Value;

        if (diff.magnitude < maxLimitRange)
        {
            calPos = nowPos;
        }
        else
        {
            var PosX = diff.x * maxLimitRange / diff.magnitude + startPoint.Value.x;
            var posY = diff.y * maxLimitRange / diff.magnitude + startPoint.Value.y;
            calPos = new Vector3(PosX, posY, nowPos.z);
        }

        targetImage.transform.position = calPos;
        InputUtilityManager.Get().OnInputMove(calPos - startPoint.Value);
    }
    public void OnPointUp()
    {
        if (coroutineRunFadeOut != null)
        {
            StopCoroutine(coroutineRunFadeOut);
            coroutineRunFadeOut = null;
        }

        coroutineRunFadeOut = StartCoroutine(IeRunFadeOut());
        startPoint = null;
    }
    IEnumerator IeRunFadeOut()
    {
        var t = maxDispeareTime;

        while (t > 0)
        {
            t -= Time.deltaTime;
            var col = targetImage.color;
            col.a *= 0.9f;//todo
            targetImage.color = col;
            yield return null;
        }

        var colF = targetImage.color;
        colF.a = 0f;
        targetImage.color = colF;
        coroutineRunFadeOut = null;
    }
    public void OnClickActionButton()
    {
        InputUtilityManager.Get().OnClickAction();
    }
    public void OnClickJumpButton()
    {
        InputUtilityManager.Get().OnClickJump();
    }
    // void OnGUI()
    // {
    //     GUI.Label(new Rect(Screen.width / 7 * 6, Screen.height / 8 * 0, Screen.width / 7, Screen.height / 8), startPoint.ToString());
    //     GUI.Label(new Rect(Screen.width / 7 * 6, Screen.height / 8 * 1, Screen.width / 7, Screen.height / 8), nowPos.ToString());
    //     GUI.Label(new Rect(Screen.width / 7 * 6, Screen.height / 8 * 2, Screen.width / 7, Screen.height / 8), calPos.ToString());
    // }
}
