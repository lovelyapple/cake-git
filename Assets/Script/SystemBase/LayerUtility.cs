using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Layerをキャッシュしといたクラス
 * 基本Layerの取得はこのクラスとる。
 */
public class LayerUtility
{
    public class LayerProperty
    {
        public string layerName;
        public int layerIdx;
        public LayerMask layerMask;
        public LayerProperty(string name)
        {
            this.layerName = name;
            this.layerIdx = LayerMask.NameToLayer(name);
            this.layerMask = 1 << layerIdx;
        }
    }
    static Dictionary<string, LayerProperty> layerDict = new Dictionary<string, LayerProperty>()
    {
        {"FieldEnvObject" ,new LayerProperty("FieldEnvObject")},
        {"Slime" ,new LayerProperty("Slime")},
    };
    public static int FieldEnvObjectIdx { get { return GetLayerIdx("FieldEnvObject"); } }
    public static LayerMask FieldEnvObjectMask { get { return GetLayerMask("FieldEnvObject"); } }
    public static int SlimeIdx { get { return GetLayerIdx("Slime"); } }
    public static LayerMask SlimeMask { get { return GetLayerMask("Slime"); } }
    public static LayerProperty GetNewLayer(string name)
    {
        Debug.LogWarning("coudl not find layer in cache looking for it " + name);
        var newLayer = new LayerProperty(name);

        if (newLayer.layerIdx < 0)
        {
            Debug.LogError("could not find Layer " + name);
            return null;
        }

        layerDict.Add(name, newLayer);
        return newLayer;
    }
    public static LayerMask GetLayerMask(string layerName)
    {
        LayerProperty outD;
        if (layerDict.TryGetValue(layerName, out outD))
        {
            return outD.layerMask;
        }
        else
        {
            outD = GetNewLayer(layerName);
            if (outD != null)
            {
                return outD.layerMask;
            }
        }
        return 0;
    }
    public static int GetLayerIdx(string layerName)
    {
        LayerProperty outD;
        if (layerDict.TryGetValue(layerName, out outD))
        {
            return outD.layerIdx;
        }
        else
        {
            outD = GetNewLayer(layerName);
            if (outD != null)
            {
                return outD.layerIdx;
            }
        }
        return -1;
    }
    public static string GetLayeName(int idx)
    {
        if (idx < 0) { return string.Empty; }

        foreach (var n in layerDict.Keys)
        {
            if (layerDict[n].layerIdx == idx)
            {
                return layerDict[n].layerName;
            }
        }

        Debug.LogError("could not find layer by idx " + idx);
        return string.Empty;
    }


}
