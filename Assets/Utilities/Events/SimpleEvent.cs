using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GusteruStudio.ReactiveVariables
{
    [CreateAssetMenu(menuName = "GusteruStudio/Events/SimpleEvent")]
    public sealed class SimpleEvent : ScriptableObject
    {
        [NonSerialized]private UnityEvent _simpleEvent = new UnityEvent();

        public void Subscribe(UnityAction callBack)
        {
            _simpleEvent.AddListener(callBack);
        }

        public void Unsubscribe(UnityAction callBack)
        {
            _simpleEvent.RemoveListener(callBack);
        }

        public void Invoke()
        {
            _simpleEvent.Invoke();
        }
    }
}
