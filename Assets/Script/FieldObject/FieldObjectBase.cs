﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldObjectBase : MonoBehaviour
{

    [SerializeField] MeshRenderer meshRenderer;
    public bool IsUnvisble;
    protected Action<Collider> onTriggleEnterFix;
    protected Action<Collider> onTriggleStayFix;
    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
    }
    // Use this for initialization
    void Start()
    {
        if (IsUnvisble && meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider other)
    {

        if (onTriggleEnterFix != null)
        {
            onTriggleEnterFix(other);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (onTriggleStayFix != null)
        {
            onTriggleStayFix(other);
        }
    }
    void OnTriggerExit(Collider other)
    {
    }
}
