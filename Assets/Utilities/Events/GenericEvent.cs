using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GusteruStudio.Selection
{
    public abstract class GenericEvent<T> : ScriptableObject
    {
        [NonSerialized] private UnityEvent<T> _simpleEvent = new UnityEvent<T>();

        public void Subscribe(UnityAction<T> callBack)
        {
            _simpleEvent.AddListener(callBack);
        }

        public void Unsubscribe(UnityAction<T> callBack)
        {
            _simpleEvent.RemoveListener(callBack);
        }

        public void Invoke(T arg)
        {
            _simpleEvent.Invoke(arg);
        }
    }
}
