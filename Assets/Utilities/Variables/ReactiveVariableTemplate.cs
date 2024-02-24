using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GusteruStudio.ReactiveVariables
{
    public abstract partial class ReactiveVariableTemplate<T> : ReactiveVariable
    {
        public override event Action onValueChanged;

        [BoxGroup("Config")][SerializeField] private T _defaultValue;
        [BoxGroup("Config")][SerializeField] private T _value;

        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                onValueChanged?.Invoke();
            }
        }

        public void SetWithoutNotify(T value)
        {
            _value = value;
        }
    }
}
