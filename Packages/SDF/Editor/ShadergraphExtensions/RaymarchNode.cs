using System.Reflection;
using UnityEditor.ShaderGraph;

namespace SDF.ShadergraphExtensions {
    class RaymarchNode : CodeFunctionNode {
        protected override MethodInfo GetFunctionToConvert() => throw new System.NotImplementedException();

        static string SDF_Primitive_Sphere(
            [Slot(0, Binding.None)]     DynamicDimensionVector P,
            [Slot(1, Binding.None)]     DynamicDimensionVector R,
            [Slot(2, Binding.None)] out Vector1                Out
        ) => @"
{
    Out = length(P) - R;
}
";
    }
}
