using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowBase : MonoBehaviour
{
    public virtual void Open()
    {
        UIUtility.SetActive(this.gameObject, true);
    }
    public virtual void Close()
    {
        UIUtility.SetActive(this.gameObject, false);
    }
    public void MoveToTop()
    {
        gameObject.transform.SetAsLastSibling();
    }

}
