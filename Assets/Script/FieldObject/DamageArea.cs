using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageArea : FieldObjectBase
{
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
	float secOne = 1000f;
	float timeRemaining = 1000f;
    void Update()
    {
        // if (IsPlayerInside())
        // {
        //     var chara = FieldManager.Get().GetMainChara();

        //     if (chara != null)
        //     {
		// 		if(timeRemaining <= 0f)
		// 		{
		// 			chara.ChangeCharacterStatusLevel(-1);
		// 			timeRemaining = secOne;
		// 		}
		// 		else
		// 		{
		// 			timeRemaining -= Time.deltaTime * secOne;
		// 		}
        //     }
        // }
    }

}
