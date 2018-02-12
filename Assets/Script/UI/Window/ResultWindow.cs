using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ResultWindow : WindowBase
{
    [SerializeField] Text titleLabel;
    [SerializeField] Text savedSlimeCountLabel;
    public void OnClickRestart()
    {
        FieldManager.Get().ReSetMap(() =>
        {
            GameMainObject.Get().RequestChangeStateWithoutFade(GameState.Game, null);
            WindowManager.CreateOpenWindow(WindowIndex.FieldMenu, (w) =>
            {
                var fieldMenu = w as FieldMenu;
                fieldMenu.SetupFieldData();
            });
            Close();
        }, null);
    }
    public void SetUp(uint savedSlimeCount, bool isClear = true)
    {
        if (savedSlimeCountLabel != null)
        {
            savedSlimeCountLabel.text = savedSlimeCount.ToString();
        }

        if (titleLabel != null)
        {
            titleLabel.text = isClear ? StringTable.MissionClear : StringTable.MissionFailed;
        }

        StateConfig.IsPausing = true;
    }
}
