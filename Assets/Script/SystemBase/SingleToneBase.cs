using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleToneBase<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Get()
    {
        if (_instance == null)
        {
            _instance = (T)FindObjectOfType(typeof(T));
            if (_instance == null)
            {
                Debug.LogError("could not get Manger " + typeof(T));
            }
        }
        return _instance;
    }
}
