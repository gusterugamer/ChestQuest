using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GusteruStudio.ReactiveVariables
{
    [CreateAssetMenu(menuName = "GusteruStudio/ReactiveVariables/Vector3Variable")]
    public class Vector3Variable : ReactiveVariableTemplate<Vector3>
    {
        public static implicit operator Vector3(Vector3Variable fVar) => fVar.Value;
        public static implicit operator string(Vector3Variable fVar) => fVar.Value.ToString();
    }
}
