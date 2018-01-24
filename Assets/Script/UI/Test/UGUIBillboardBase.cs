using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UGUIBillboardBase : MonoBehaviour
{
	RectTransform rectTransform = null;
	public Camera targetCamera;
	[SerializeField] Transform target = null;

	void Awake()
	{
		rectTransform = GetComponent<RectTransform> ();
	}

	void Update()
	{
		rectTransform.position = RectTransformUtility.WorldToScreenPoint (targetCamera, target.position);
	}
}
