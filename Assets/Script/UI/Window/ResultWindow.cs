using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultWindow : WindowBase
{
    public void OnClickRestart()
    {
        FieldManager.Get().ReSetMap(() =>
        {
            Close();
            GameMainObject.Get().RequestChangeStateWithoutFade(GameState.Game, null);
        }, null);
    }
}
