using System;
using API;
using UnityEngine;
using static System.FormattableString;

// Jumbled as a single visitor because properties are sealed for extension
namespace BuiltInTarget.Variables {
    public static class HlslVariable {
        public static string GetHlslType(Variable variable) => variable switch
        {
            Variable<int> => "int",
            Variable<float> => "float",
            Variable<Vector4> => "float4",
            Variable<Vector3> => "float3",
            Variable<Vector2> => "float2",
            Variable<Color> => "float4",
            Variable<Texture2D> => "texture2d",
            _ => throw new ArgumentOutOfRangeException(nameof(variable), variable, "variable type not supported")
        };

        public static Evaluator GetEvaluator(Variable v) =>
            () => Invariant($"{GetHlslType(v)} {v.InternalName}_{v.Guid.ToString()}");

        public delegate string Evaluator();
    }
}
