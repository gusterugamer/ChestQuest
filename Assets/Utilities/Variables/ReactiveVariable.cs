using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GusteruStudio.ReactiveVariables
{
    public abstract class ReactiveVariable : ScriptableObject
    {
        public abstract event Action onValueChanged;
    }
}
