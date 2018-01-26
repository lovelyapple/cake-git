using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
public enum WindowIndex
{
    LoadWindow = 0,
    TitleWindow = 1,
    Max,
}
public class ResourcesManager : SingleToneBase<ResourcesManager>
{
    //============= WindowManager===================//
    [SerializeField] GameObject UIWindowRoot;

    Dictionary<WindowIndex, string> windowPathDict = new Dictionary<WindowIndex, string>()
    {
        {WindowIndex.TitleWindow,"Assets/ExternalResources/UI/Window/TitleMenuWindow.prefab"},
        {WindowIndex.LoadWindow,"Assets/ExternalResources/UI/Window/LoadWindow.prefab"},
    };

    //サイズ分確保
    List<WindowBase> windowList;
    void Start()
    {
        InitWindowList();
    }
    void InitWindowList()
    {
        windowList = new List<WindowBase>();

        for (int idx = 0, idMax = (int)(WindowIndex.Max - 1); idx < idMax; idx++)
        {
            windowList.Add(null);
        }
    }
    public void CreateOpenWindow(WindowIndex index, Action<WindowBase> onLoad)
    {
        if (UIWindowRoot == null)
        {
            Debug.LogError("there is no root for Window UI");
            return;
        }

        if (windowList[(int)index] == null)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(windowPathDict[index]);

            if (prefab == null)
            {
                Debug.LogError("could not find window Prefab " + index.ToString());
                return;
            }

            var windowInstance = GameObject.Instantiate(prefab, UIWindowRoot.transform).GetComponent<WindowBase>();

            if (windowInstance == null)
            {
                Debug.LogError("could not fine WIndowBase in " + index.ToString());
                return;
            }

            windowList[(int)index] = windowInstance;
        }
        else
        {
            if (windowList[(int)index].gameObject.activeSelf == true)
            {
                Debug.LogWarning(" window is active " + index.ToString());
            }
        }

        windowList[(int)index].Open();

        if (onLoad != null)
        {
            onLoad(windowList[(int)index]);
        }
    }
    public void CloseWindow(WindowIndex index)
    {
        if (windowList[(int)index] == null)
        {
            Debug.LogWarning("could not find wnd Instace when close " + index.ToString());
            return;
        }

        windowList[(int)index].Close();
    }
    public void ClearAll()
    {
        foreach (var wnd in windowList)
        {
            GameObject.Destroy(wnd.gameObject);
        }
        InitWindowList();
    }
    //============= WindowManager===================//
}
