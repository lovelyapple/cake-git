using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultWindow : WindowBase
{
    public void OnClickRestart()
    {
        FieldManager.Get().ReSetMap(() =>
        {
            GameMainObject.Get().RequestChangeStateWithoutFade(GameState.Game, null);
            ResourcesManager.Get().CreateOpenWindow(WindowIndex.FieldMenu, (w) =>
            {
                var fieldMenu = w as FieldMenu;
                fieldMenu.SetupFieldData();
            });
            Close();
        }, null);
    }
}
