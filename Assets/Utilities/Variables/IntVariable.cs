using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GusteruStudio.ReactiveVariables
{
    [CreateAssetMenu(menuName = "GusteruStudio/ReactiveVariables/IntVariable")]
    public class IntVariable : ReactiveVariableTemplate<int>
    {
        public static implicit operator bool(IntVariable fVar) => fVar.Value != 0;
        public static implicit operator string(IntVariable fVar) => fVar.Value.ToString();
        public static implicit operator int(IntVariable fVar) => fVar.Value;

        public static explicit operator float(IntVariable fVar) => fVar.Value;
        public static explicit operator double(IntVariable fVar) => fVar.Value;

        public static IntVariable operator +(IntVariable lhs, object rhs)
        {
            lhs.Value += (int)rhs;
            return lhs;
        }
        public static IntVariable operator -(IntVariable lhs, object rhs)
        {
            lhs.Value += (int)rhs;
            return lhs;
        }

        public static IntVariable operator /(IntVariable lhs, object rhs)
        {
            lhs.Value += (int)rhs;
            return lhs;
        }

        public static IntVariable operator *(IntVariable lhs, object rhs)
        {
            lhs.Value *= (int)rhs;
            return lhs;
        }

        public static IntVariable operator %(IntVariable lhs, object rhs)
        {
            lhs.Value %= (int)rhs;
            return lhs;
        }
    }
}
