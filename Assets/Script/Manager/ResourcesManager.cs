using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
public enum WindowIndex
{
    TitleWindow,
    LoadWindow,
    ResultWindow,
	PauseWindow,
    FieldMenu,
    Max,
}
public enum FieldObjectIndex
{
    SlimeMainChara,
    SlimeCharacterData00,
    SlimeCharacterCollderController,
    SlimeFriend,
    TestDungeon,
}
public enum SourceType
{
    Dungeon,
}
public class ResourcesManager : SingleToneBase<ResourcesManager>
{
    //============= WindowManager===================//
    [SerializeField] GameObject UIWindowRoot;

    Dictionary<WindowIndex, string> windowPathDict = new Dictionary<WindowIndex, string>()
    {
        {WindowIndex.TitleWindow,"Assets/ExternalResources/UI/Window/TitleWindow.prefab"},
        {WindowIndex.LoadWindow,"Assets/ExternalResources/UI/Window/LoadWindow.prefab"},
        {WindowIndex.ResultWindow,"Assets/ExternalResources/UI/Window/ResultWIndow.prefab"},
		{WindowIndex.PauseWindow,"Assets/ExternalResources/UI/Window/Part_PauseMenu/PauseWindow.prefab"},
        {WindowIndex.FieldMenu,"Assets/ExternalResources/UI/Window/Part_FieldMenu/FieldMenu.prefab"},
    };

    //サイズ分確保
    List<WindowBase> windowList;
    public void ChecktInitWindowList()
    {
        if (windowList == null)
        {
            InitWindowList();
        }
    }
    void InitWindowList()
    {
        windowList = new List<WindowBase>();

        for (int idx = 0, idMax = (int)(WindowIndex.Max); idx < idMax; idx++)
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
    public bool IsWindowActive(WindowIndex index)
    {
        try
        {
            var w = windowList[(int)index];

            if (w == null) { return false; }

            return w.gameObject.activeInHierarchy;
        }
        catch
        {
            return false;
        }
    }
    public WindowBase GetWindow(WindowIndex index)
    {
        try
        {
            return windowList[(int)index];
        }
        catch
        {
            return null;
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

    //============= normal Load ====================//
    [SerializeField] GameObject FieldObjectRoot;
    Dictionary<FieldObjectIndex, string> fieldObjectPathDict = new Dictionary<FieldObjectIndex, string>()
    {
        {FieldObjectIndex.SlimeMainChara, "Assets/Resoures/Character/MainSlime.prefab"},
        {FieldObjectIndex.SlimeCharacterData00, "Assets/Resoures/CharacterData/CharacterData_Slime00.prefab"},
        {FieldObjectIndex.SlimeCharacterCollderController, "Assets/Resoures/Character/SlimeCoreHitController.prefab"},
        {FieldObjectIndex.TestDungeon, "Assets/Resoures/Dungeon/Dungeon00.prefab"},
        {FieldObjectIndex.SlimeFriend,"Assets/Resoures/Character/FriendSlime.prefab"},
    };
    Dictionary<FieldObjectIndex, GameObject> fieldObjectPrefabHolder = new Dictionary<FieldObjectIndex, GameObject>();
    public GameObject CreateInstance(FieldObjectIndex index, Transform parent = null, bool saveCache = true)
    {
        if (parent == null)
        {
            parent = FieldObjectRoot.transform;
        }

        GameObject prefab;
        if (fieldObjectPrefabHolder.ContainsKey(index) && fieldObjectPrefabHolder[index] != null)
        {
            prefab = fieldObjectPrefabHolder[index];
            return GameObject.Instantiate(prefab, parent.transform);
        }
        else
        {
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>(fieldObjectPathDict[index]);

            if (prefab == null)
            {
                Debug.LogError("could not load resource " + fieldObjectPathDict[index]);
            }

            if (fieldObjectPrefabHolder.ContainsKey(index))
            {
                fieldObjectPrefabHolder[index] = prefab;
            }
            else
            {
                fieldObjectPrefabHolder.Add(index, prefab);
            }

            prefab = fieldObjectPrefabHolder[index];
            return GameObject.Instantiate(prefab, parent.transform);
        }
    }
    //resource Path
    public static string GetDungeonLoadPath(string dungeonName)
    {
        return string.Format("Assets/Resources/Dungeon/{0}.prefab", dungeonName);
    }
    //ソースタイプ

    //普通のロード
    public static GameObject LoadSourcePrefab(string sourceName, SourceType type)
    {
        string loadPath = "";
        switch (type)
        {
            case SourceType.Dungeon:
                loadPath = GetDungeonLoadPath(sourceName);
                break;
        }

        return AssetDatabase.LoadAssetAtPath<GameObject>(loadPath);
    }
    public static T CreateGetComponent<T>(GameObject prefab) where T : class
    {
        var instance = GameObject.Instantiate(prefab) as GameObject;
        instance.transform.position = Vector3.zero;
        instance.transform.rotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;

        return instance.GetComponent<T>();
    }
}
