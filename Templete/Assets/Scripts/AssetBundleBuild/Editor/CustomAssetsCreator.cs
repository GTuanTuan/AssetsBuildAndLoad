using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
namespace Templete
{
    public class CustomAssetsCreator
    {
        [MenuItem("Assets/Create/CreateCustomAsset")]
        public static void CreateCustomAsset()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj is MonoScript)
            {
                MonoScript script = obj as MonoScript;
                Type t = script.GetClass();
                if (t != null && t.IsSubclassOf(typeof(ScriptableObject)))
                {
                    ScriptableObject scriptableObject = ScriptableObject.CreateInstance(t);
                    string path = $"{Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]))}/{script.name}.asset";
                    AssetDatabase.CreateAsset(scriptableObject, path);
                }
            }
        }
    }
}
