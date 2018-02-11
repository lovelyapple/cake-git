using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public enum FieldObjectIndex
{
    SlimeMainChara,
    SlimeCharacterData00,
    SlimeCharacterCollderController,
    SlimeFriend,
    SlimeEnemy,
    TestDungeon,
}
public enum SourceType
{
    Dungeon,
}
public class ResourcesManager : SingleToneBase<ResourcesManager>
{

    //============= normal Load ====================//
    [SerializeField] GameObject FieldObjectRoot;
    Dictionary<FieldObjectIndex, string> fieldObjectPathDict = new Dictionary<FieldObjectIndex, string>()
    {
        {FieldObjectIndex.SlimeMainChara, "Assets/Resoures/Character/MainSlime.prefab"},
        {FieldObjectIndex.SlimeCharacterData00, "Assets/Resoures/CharacterData/CharacterData_Slime00.prefab"},
        {FieldObjectIndex.SlimeCharacterCollderController, "Assets/Resoures/Character/SlimeCoreHitController.prefab"},
        {FieldObjectIndex.TestDungeon, "Assets/Resoures/Dungeon/Dungeon00.prefab"},
        {FieldObjectIndex.SlimeFriend,"Assets/Resoures/Character/FriendSlime.prefab"},
        {FieldObjectIndex.SlimeEnemy,"Assets/Resoures/Character/EnemySlime.prefab"},
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
