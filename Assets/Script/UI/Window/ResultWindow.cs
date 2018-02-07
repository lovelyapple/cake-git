using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ResultWindow : WindowBase
{
    [SerializeField] Text savedSlimeCountLabel;
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
    public void SetUp(uint savedSlimeCount)
    {
        if (savedSlimeCountLabel != null)   
        {  
            savedSlimeCountLabel.text = savedSlimeCount.ToString();
        }
    }
}
