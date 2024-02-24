using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GusteruStudio.ReactiveVariables
{
    [CreateAssetMenu(menuName = "GusteruStudio/ReactiveVariables/FloatVariable")]
    public class FloatVariable : ReactiveVariableTemplate<float>
    {
        public static implicit operator float(FloatVariable fVar) => fVar.Value;
        public static implicit operator bool(FloatVariable fVar) => fVar.Value != 0f;
        public static implicit operator string(FloatVariable fVar) => fVar.Value.ToString();

        public static explicit operator int(FloatVariable fVar) => (int)fVar.Value;
        public static explicit operator double(FloatVariable fVar) => fVar.Value;

        public static FloatVariable operator +(FloatVariable lhs, object rhs)
        {
            lhs.Value += (float)rhs;
            return lhs;
        }
        public static FloatVariable operator -(FloatVariable lhs, object rhs)
        {
            lhs.Value += (float)rhs;
            return lhs;
        }

        public static FloatVariable operator /(FloatVariable lhs, object rhs)
        {
            lhs.Value += (float)rhs;
            return lhs;
        }

        public static FloatVariable operator *(FloatVariable lhs, object rhs)
        {
            lhs.Value *= (float)rhs;
            return lhs;
        }

        public static FloatVariable operator %(FloatVariable lhs, object rhs)
        {
            lhs.Value %= (float)rhs;
            return lhs;
        }
    }
}
