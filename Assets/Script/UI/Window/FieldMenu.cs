using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldMenu : WindowBase
{
    [SerializeField] Slider hpGageSlider;
    [SerializeField] Text friendSlimeLfet;

    public void OnUpdateHpSlider(CharacterData data)
    {
        if (hpGageSlider != null && data != null)
        {
            var maxHp = data.GetHp((uint)data.maxLevel);
            var currentHp = data.GetHp();
            hpGageSlider.value = currentHp;
            hpGageSlider.maxValue = maxHp;
        }
    }
    public void OnUpdateFriendSlimeLeftCount(uint count)
    {
        if (friendSlimeLfet != null)
        {
            friendSlimeLfet.text = count.ToString();
        }
    }

    public void SetUpCharacterData()
    {
        var mainCharaJellyMesh = FieldManager.Get().GetMainChara();
        if (mainCharaJellyMesh == null)
        {
            Debug.LogError("mainCharaJellyMesh is null");
            return;
        }

		mainCharaJellyMesh.OnStatusChanged += OnUpdateHpSlider;
    }
}
