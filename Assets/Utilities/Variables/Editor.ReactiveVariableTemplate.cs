#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEditor;

namespace GusteruStudio.ReactiveVariables
{
    public abstract partial class ReactiveVariableTemplate<T>
    {
        private void OnEnable()
        {
            SubToEvents();
        }

        private void OnDisable()
        {
            UnSubFromEvents();
        }

        private void SubToEvents()
        {
            UnSubFromEvents();
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
        }

        private void UnSubFromEvents()
        {
            EditorApplication.playModeStateChanged -= PlayModeStateChanged;
        }

        private void PlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
                ResetValueToDefault();
        }

        [Button]
        private void SetValueAsDefault()
        {
            _defaultValue = _value;
        }

        private void ResetValueToDefault()
        {
            _value = _defaultValue;
        }
    }
}
#endif