﻿using System.Collections;
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
                var loadWindow = WindowManager.Get().GetWindow(WindowIndex.LoadWindow) as LoadWindow;
                if (loadWindow != null && loadWindow.isActiveAndEnabled)
                {
                    loadWindow.OnClose = () =>
                    {
                        WindowManager.Get().CreateOpenWindow(WindowIndex.FieldMenu, (w) =>
                        {
                            var fieldMenu = w as FieldMenu;
                            fieldMenu.SetupFieldData();
                            FieldManager.Get().RequestUpdateFieldInfo();
                        });
                    };
                }

            },
            () =>
            {
                //onError
            });
        };
    }
}
