using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeCatchArea : FieldObjectBase
{
    void OnEnable()
    {
        if(onTriggleEnterFix == null)
        {
            onTriggleEnterFix = CheckFriendSlimeJumping;
        }   
    }

    void CheckFriendSlimeJumping(Collider other)
    {
        var friendAI = other.gameObject.GetComponent<AIFriendSlime>();
        if(friendAI != null)
        {
            //todo 味方のスライムがこの中に入ってきた
        }
    }
}
