using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeEndArea : FieldObjectBase
{
    //新しい仕様では　いらないっぽい
    [SerializeField] float clearTime;
    [SerializeField] float resetTime = 1f;
    [SerializeField] bool isPlayerInside;
    void OnEnable()
    {
        onTriggleStayFix = null;
        onTriggleStayFix = OnPlayerStayInEnd;
        clearTime = 3f;
    }
    void OnPlayerStayInEnd(Collider ohter)
    {
        if (!GameMainObject.Get().IsGamePlaying) { return; }
        if (ohter.gameObject.tag != "Player") { return; }
        clearTime -= Time.deltaTime;

        if (clearTime <= 0)
        {
            GameMainObject.Get().RequestChangeStateWithoutFade(GameState.Result, () =>
             {
                 ResourcesManager.Get().CreateOpenWindow(WindowIndex.ResultWindow, (w) =>
                 {
                     var resWnd = w as ResultWindow;
                     clearTime = 3;
                     resWnd.SetUp(FieldManager.Get().savedFriendCount);
                     SoundManager.Get().PlayOneShotSe_Release();
                 });
             });
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (clearTime < 3f && resetTime > 0 && GameMainObject.Get().IsGamePlaying)
        {
            resetTime -= Time.deltaTime;
            if (resetTime <= 0)
            {
                clearTime = 3f;
                resetTime = 1f;
            }
        }
    }
}
