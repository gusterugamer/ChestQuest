#if UNITY_EDITOR
using GusteruStudio.Extensions;
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerBaseState
{
    public void EditorCheckState(CollectionChangeInfo info, List<PlayerBaseState> newAddedSubstates)
    {
        var states = new List<PlayerBaseState>(newAddedSubstates);
        if (states.Count > 0)
        {
            RemoveRootStates(newAddedSubstates);
            BlockAddingSelfAsState(newAddedSubstates);
        }
    }

    private void RemoveRootStates(List<PlayerBaseState> newAddedSubstates)
    {
        var states = new List<PlayerBaseState>(newAddedSubstates);
        foreach (PlayerBaseState state in states)
        {
            if (state.IsRootState)
            {
                string message = ($"YOU ARE NOT ALLOWED TO ADD {state.name} AS A SUBSTATE BECAUSE IS A ROOT STATE!");
                Debug.Log(message.AsHexColoredText("#c91508"));
                _validStates.Remove(state);
            }
        }
    }

    private void BlockAddingSelfAsState(List<PlayerBaseState> newAddedSubstates)
    {
        var states = new List<PlayerBaseState>(newAddedSubstates);
        foreach (PlayerBaseState state in states)
        {
            if (state.GetType() == GetType())
            {
                string message = ($"{name} CANNOT BE IS OWN SUBSTATE!");
                Debug.Log(message.AsHexColoredText("#c91508"));
                _validStates.Remove(state);
            }
        }
    }
}
#endif