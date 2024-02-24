using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GusteruStudio.ReactiveVariables
{
    [CreateAssetMenu(menuName = "GusteruStudio/ReactiveVariables/BoolVariable")]
    public class BoolVariable : ReactiveVariableTemplate<bool>
    {
        public static implicit operator bool(BoolVariable fVar) => fVar.Value;
        public static implicit operator string(BoolVariable fVar) => fVar.Value.ToString();
    }
}
