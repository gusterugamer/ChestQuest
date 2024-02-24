using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace GusteruStudio.Editor
{
#if UNITY_EDITOR
    public class AssetsDataBaseRefreshListener : AssetPostprocessor
    {
        public static event UnityAction onDatabaseRefreshed;

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
        //    Debug.Log("Assets database refreshed");
            onDatabaseRefreshed?.Invoke();
        }
    }
#endif
}
