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
                ResourcesManager.Get().CreateOpenWindow(WindowIndex.FieldMenu, (w) =>
                {
                    var FieldMenu = w as FieldMenu;
                    var mainCharaJellyMesh = FieldManager.Get().GetMainChara();
                });
            },
            () =>
            {
                //onError
            });
        };
    }
}
