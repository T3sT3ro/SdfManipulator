using System;
using System.Linq;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.API;
using UnityEngine;
using Attribute = me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific.Attribute;
namespace me.tooster.sdf.Editor.Controllers {
    public static class SyntaxExtensions {
        public static PredefinedTypeToken hlslTypeToken(this Property p) => p switch
        {
            Property<int>        => new IntKeyword(),
            Property<float>      => new FloatKeyword(),
            Property<bool>       => new BoolKeyword(),
            Property<Vector2>    => new VectorTypeToken { type = Constants.ScalarKind.@float, arity = 2 },
            Property<Vector3>    => new VectorTypeToken { type = Constants.ScalarKind.@float, arity = 3 },
            Property<Vector4>    => new VectorTypeToken { type = Constants.ScalarKind.@float, arity = 4 },
            Property<Vector2Int> => new VectorTypeToken { type = Constants.ScalarKind.@int, arity = 2 },
            Property<Vector3Int> => new VectorTypeToken { type = Constants.ScalarKind.@int, arity = 3 },
            Property<Matrix4x4>  => new MatrixTypeToken { type = Constants.ScalarKind.@float, cols = 4, rows = 4 },
            _                    => throw new ArgumentOutOfRangeException(nameof(p), p, "No type token for this unity type"),
        };

        public static PredefinedTypeToken hlslTypeToken<T>() where T : struct => typeof(T) switch
        {
            { } t when t == typeof(int) => new IntKeyword(),
            { } t when t == typeof(float) => new FloatKeyword(),
            { } t when t == typeof(bool) => new BoolKeyword(),
            { } t when t == typeof(Vector2) => new VectorTypeToken { type = Constants.ScalarKind.@float, arity = 2 },
            { } t when t == typeof(Vector3) => new VectorTypeToken { type = Constants.ScalarKind.@float, arity = 3 },
            { } t when t == typeof(Vector4) => new VectorTypeToken { type = Constants.ScalarKind.@float, arity = 4 },
            { } t when t == typeof(Vector2Int) => new VectorTypeToken { type = Constants.ScalarKind.@int, arity = 2 },
            { } t when t == typeof(Vector3Int) => new VectorTypeToken { type = Constants.ScalarKind.@int, arity = 3 },
            { } t when t == typeof(Matrix4x4) => new MatrixTypeToken { type = Constants.ScalarKind.@float, cols = 4, rows = 4 },
            _ => throw new ArgumentOutOfRangeException(nameof(T), typeof(T), "No type token for this unity type"),
        };

        #region attribute helpers

        // some more info can be found here: https://docs.unity3d.com/ScriptReference/MaterialPropertyDrawer.html
        // TODO: assure that strings fit pattern of AttributeStringLiteral
        public static Attribute headerAttribute(string headerName) => new()
        {
            id = "Header",
            arguments = (Attribute.Value)headerName.sanitizeToIdentifierString(),
        };

        public static Attribute spaceAttribute()  => new() { id = "Space" };
        public static Attribute toggleAttribute() => new() { id = "Toggle" };

        public static Attribute tooltipAttribute(string tooltip) => new()
        {
            id = "Tooltip",
            arguments = (Attribute.Value)tooltip,
        };

        public static Attribute enumAttribute<T>() where T : Enum => new()
        {
            id = "Enum",
            arguments = (Attribute.Value)typeof(T).FullName!,
        };

        public static Attribute keyEnumAttribute(params string[] values) => new()
        {
            id = "KeyEnum",
            arguments = values.Select(s => new Attribute.Value { value = s }).ToArray(),
        };

        #endregion
    }
}
