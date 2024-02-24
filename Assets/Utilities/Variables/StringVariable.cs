using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GusteruStudio.ReactiveVariables
{
    [CreateAssetMenu(menuName = "GusteruStudio/ReactiveVariables/StringVariable")]
    public class StringVariable : ReactiveVariableTemplate<string>
    {
        public static implicit operator string(StringVariable fVar) => fVar.Value;
    }
}
