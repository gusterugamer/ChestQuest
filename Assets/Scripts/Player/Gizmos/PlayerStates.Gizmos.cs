#if UNITY_EDITOR

using GusteruStudio.Editor;
using OriginData.Utilities;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GusteruStudio.PlayerStates
{

    [InitializeOnLoad]
    public partial class PlayerStates
    {
        PlayerStates()
        {
            AssetsDataBaseRefreshListener.onDatabaseRefreshed += UpdateListOfStates;
        }

        ~PlayerStates()
        {
            AssetsDataBaseRefreshListener.onDatabaseRefreshed -= UpdateListOfStates;
        }

        [Button]
        private void UpdateListOfStates()
        {
            _states = EditorCustomUtilities.LoadAllScriptableObjectsOfType<PlayerBaseState>();
        }
    }
}

#endif
