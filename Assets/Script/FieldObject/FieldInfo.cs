﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//フィールド各種データを持つデータクラス
public class FieldInfo : MonoBehaviour
{
	[SerializeField] SlimeStartPoint startPoint;
	[SerializeField] SlimeEndArea endArea;
	[SerializeField] DamageArea[] damageArea;
	[SerializeField] SlimeCatchArea slimeCatchArea;
	[Header("フィールドの機能オブジェクトの親ルート")]
	[SerializeField] GameObject fieldEntityRoot;
	[Header("フィールドの通常オブジェクトの親ルート")]
	[SerializeField] GameObject fieldEvnObjRoot;
	public List<GameObject> friendSlimeList;
	public List<GameObject> enemySlimeList;

	public FieldObjectBase GetStartPoint()
	{
		return startPoint;
	}
	public SlimeEndArea GetEndArea()
	{
		return endArea;
	}
}
