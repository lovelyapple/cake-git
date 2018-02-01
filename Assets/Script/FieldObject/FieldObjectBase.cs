using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldObjectBase : MonoBehaviour
{

    [SerializeField] MeshRenderer meshRenderer;
    bool isPlayerInside = false;
    public bool IsUnvisble;
    protected Action<Collider> onTriggleEnterFix;
    public bool IsPlayerInside()
    {
        return isPlayerInside;
    }
    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        isPlayerInside = false;
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
        if (other.gameObject.tag == "Player")
        {
            isPlayerInside = true;
        }

        if (onTriggleEnterFix != null)
        {
            onTriggleEnterFix(other);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isPlayerInside = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isPlayerInside = false;
        }
    }
}
