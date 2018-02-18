using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorResourceLoader
{
    public static GameObject LoadResource(string filePath)
    {
        return AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
    }
}
