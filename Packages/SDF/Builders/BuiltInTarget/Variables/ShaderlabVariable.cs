using System;
using API;
using UnityEngine;
using static System.FormattableString;

// Jumbled as a single visitor because properties are sealed for extension
namespace Builders.BuiltInTarget.Variables {
    public static class ShaderlabVariable {
        private static string GetShaderlabType(Variable variable) => variable switch
        {
            Variable<int> => "Integer",
            Variable<float> => "Float",
            Variable<Vector4> => "Vector",
            Variable<Vector3> => "Vector",
            Variable<Vector2> => "Vector",
            Variable<Color> => "Color",
            Variable<Texture2D> => "2D",
            _ => throw new ArgumentOutOfRangeException(nameof(variable), variable, "variable type not supported")
        };

        private static FormattableString ParseShaderlabValue(Variable variable) => variable switch
        {
            Variable<int>       v => $"{v.DefaultValue}",
            Variable<float>     v => $"{v.DefaultValue:F}",
            Variable<Vector4>   v => $"({v.DefaultValue.x:F},{v.DefaultValue.y:F},{v.DefaultValue.z:F}, {v.DefaultValue.w:F})",
            Variable<Color>     v => $"({v.DefaultValue.r:F},{v.DefaultValue.g:F},{v.DefaultValue.b:F}, {v.DefaultValue.a:F})",
            Variable<Texture2D> v => $"\"\" {{}}",
            _ => throw new ArgumentOutOfRangeException(nameof(variable), variable, "variable type not supported")
        };


        public static Evaluator GetEvaluator(Variable v) => v.Exposed
            ? () => ""
            : () => Invariant($"{v.InternalName}_{v.Guid.ToString()} (\"{v.DisplayName}\", {GetShaderlabType(v)}) = {ParseShaderlabValue(v)}");

        public delegate string Evaluator();
    }
}
