using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleWIndow : WindowBase
{
    public void OnClickStartGame()
    {
        GameMainObject.Get().ChangeStateToGame();
        GameMainObject.Get().OnFadeInOver = () =>
        {
            Close();
            FieldManager.Get().CreateField(() =>
            {
                //onFinished
            },
            () =>
            {
                //onError
            });
        };
    }
}
