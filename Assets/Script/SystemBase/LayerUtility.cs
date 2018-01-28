using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerUtility
{
    static Dictionary<string, int> layerDict = new Dictionary<string, int>();
    public static int FieldEnvObjectLayerMask()
    {
        if (!layerDict.ContainsKey("FieldEnvObject"))
        {
            var layer = LayerMask.NameToLayer("FieldEnvObject");
            layerDict.Add("FieldEnvObject", 1 << layer);
        }

        return layerDict["FieldEnvObject"];
    }
    public string GetFieldEnvObjectString()
    {
        if (!layerDict.ContainsKey("FieldEnvObject"))
        {
            var layer = LayerMask.NameToLayer("FieldEnvObject");
            layerDict.Add("FieldEnvObject", layer);
        }

        return "FieldEnvObject";
    }
}
