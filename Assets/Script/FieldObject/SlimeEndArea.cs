using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeEndArea : FieldObjectBase
{
    //新しい仕様では　いらないっぽい
    [SerializeField] float clearTime;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        clearTime = 3f;
    }
    // Update is called once per frame
    void Update()
    {
        if (IsPlayerInside())
        {
            clearTime -= Time.deltaTime;

            if (clearTime <= 0)
            {
                GameMainObject.Get().RequestChangeStateWithoutFade(GameState.Result, () =>
                 {
                     ResourcesManager.Get().CreateOpenWindow(WindowIndex.ResultWindow, (w) =>
                     {
                        //donoth
                     });
                 });
            }
        }
    }
}
