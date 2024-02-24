using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GusteruStudio.ReactiveVariables
{
    [CreateAssetMenu(menuName = "GusteruStudio/ReactiveVariables/Vector2Variable")]
    public class Vector2Variable : ReactiveVariableTemplate<Vector2>
    {
        public static implicit operator Vector2(Vector2Variable fVar) => fVar.Value;
        public static implicit operator string(Vector2Variable fVar) => fVar.Value.ToString();
    }
}
