using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageArea : FieldObjectBase
{
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
	float secOne = 2f;
    float timeRemaining = 2f;
    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        if (base.onTriggleEnterFix == null)
        {
            onTriggleEnterFix = CheckPlayerIn;
        }
    }
    void Update()
    {

    }
    void CheckPlayerIn(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            FieldManager.Get().RequestDamageMainCharaSLime(1);
        }
    }

}
