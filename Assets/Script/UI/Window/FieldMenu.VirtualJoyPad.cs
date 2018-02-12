using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
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
#elif UNITY_IOS
        var t = Input.GetTouch(0);
        startPoint.x = t.position.x;
        startPoint.y = t.position.y;
        startPoint.z = 0;
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
#elif UNITY_IOS
		var t = Input.GetTouch(0);
		nowPos.x = t.position.x;
        nowPos.y = t.position.y;
        nowPos.z = 0;
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
    // void OnGUI()
    // {
    //     GUI.Label(new Rect(Screen.width / 7 * 6, Screen.height / 8 * 0, Screen.width / 7, Screen.height / 8), startPoint.ToString());
    //     GUI.Label(new Rect(Screen.width / 7 * 6, Screen.height / 8 * 1, Screen.width / 7, Screen.height / 8), nowPos.ToString());
    //     GUI.Label(new Rect(Screen.width / 7 * 6, Screen.height / 8 * 2, Screen.width / 7, Screen.height / 8), calPos.ToString());
    // }
}
