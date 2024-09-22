#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace OriginData.Utilities
{
    public static class EditorCustomUtilities
    {
        public static T LoadAsset<T>(string name) where T : Object
        {
            string[] platformPrefabGUID = AssetDatabase.FindAssets(name.Remove(name.IndexOf('.')));

            string platformPrefabPath = "";
            foreach (string guid in platformPrefabGUID)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains(name))
                {
                    platformPrefabPath = AssetDatabase.GUIDToAssetPath(guid);
                    break;
                }
            }

            Object asset = AssetDatabase.LoadAssetAtPath<Object>(platformPrefabPath);

            if (asset is T)
                return (T)AssetDatabase.LoadAssetAtPath<Object>(platformPrefabPath);
            if (asset is GameObject)
            {
                GameObject assetGO = asset as GameObject;
                if (assetGO.GetComponentInChildren<T>() != null)
                    return assetGO.GetComponentInChildren<T>();
            }

            return null;
        }

        public static List<T> LoadAllScriptableObjectsOfType<T>() where T : Object
        {
            List<T> list = new List<T>();
            list.Clear();
            string[] pathsToSOs = Directory.GetFiles(Application.dataPath, "*.asset", SearchOption.AllDirectories);

            foreach (string path in pathsToSOs)
            {
                string correctedPath = "Assets" + path.Replace(Application.dataPath, "").Replace("\\", "/");
                Object so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(correctedPath);
                if (so is T)
                {
                    list.Add((T)so);
                }
            }

            return list;
        }

        public static List<T> LoadAllAssetsOfType<T>(string extension) where T :Object
        {
            List<T> list = new List<T>();
            list.Clear();
            string[] pathsToSOs = Directory.GetFiles(Application.dataPath, extension, SearchOption.AllDirectories);

            foreach (string path in pathsToSOs)
            {
                string correctedPath = "Assets" + path.Replace(Application.dataPath, "").Replace("\\", "/");
                Object so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(correctedPath);
                if (so is T)
                {
                    list.Add((T)so);
                }
            }

            return list;
        }
    }
}
#endif