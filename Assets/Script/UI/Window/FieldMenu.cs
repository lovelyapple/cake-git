using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class FieldMenu : WindowBase
{
    [SerializeField] Slider hpGageSlider;
    [SerializeField] Text friendSlimeLfet;
    [SerializeField] GameObject JoyPadObj;

    void OnDisable()
    {
    }

    public void OnUpdateHpSlider(CharacterData data)
    {
        if (hpGageSlider != null && data != null)
        {
            var maxHp = data.GetHp((uint)data.maxLevel);
            var currentHp = data.GetHp();
            hpGageSlider.minValue = data.GetHp(1);
            hpGageSlider.maxValue = (float)maxHp;
            hpGageSlider.value = (float)currentHp;
        }
    }
    public void OnUpdateFriendSlimeLeftCount(uint count)
    {
        if (friendSlimeLfet != null)
        {
            friendSlimeLfet.text = count.ToString();
        }
    }
    public void SetupFieldData()
    {
        FieldManager.Get().SetUpOnUpdateMainCharaStatusLevel(OnUpdateHpSlider);
        FieldManager.Get().OnUpdateFriendCount += OnUpdateFriendSlimeLeftCount;
    }
    public void OnClickPause()
    {
        WindowManager.CreateOpenWindow(WindowIndex.PauseWindow, (w) =>
         {
             StateConfig.IsPausing = true;
         });
    }
}
