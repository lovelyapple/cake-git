using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUtility
{
    static public void SetActive(GameObject obj, bool state)
    {
        if (obj == null) { return; }
        if (obj.activeSelf == state) { return; }

        obj.SetActive(state);
    }
}
