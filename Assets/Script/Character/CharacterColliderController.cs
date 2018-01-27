using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterColliderController : MonoBehaviour
{
    float maxDistace = 20f;

    public int CountColliderHit(float radius, int? layer)
    {
        var goList = Physics.OverlapSphere(this.transform.position, radius);

        if (layer.HasValue)
        {
            return goList.Count(x => x.gameObject.layer == layer.Value);
        }
        else
        {
            return goList.Length;
        }
    }
}
