using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageArea : SlimeCatchArea
{
	void OnCollisionStay(Collision other)
	{
		if(other.gameObject.layer == LayerUtility.SlimeIdx)
		{
			
		}
	}
}
