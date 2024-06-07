using System;
using System.Linq;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Shaderlab.Syntax;
using UnityEngine;
using Attribute = me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific.Attribute;
using FloatKeyword = me.tooster.sdf.AST.Hlsl.Syntax.FloatKeyword;
using IntKeyword = me.tooster.sdf.AST.Hlsl.Syntax.IntKeyword;
using Type = System.Type;
namespace me.tooster.sdf.Editor.Controllers {
    public static class SyntaxUnityExtensions {
        public static PredefinedTypeToken hlslTypeToken(this Type t)
            => t switch
            {
                not null when t == typeof(int) => new IntKeyword(),
                not null when t == typeof(float) => new FloatKeyword(),
                not null when t == typeof(bool) => new BoolKeyword(),
                not null when t == typeof(Vector2) => new VectorTypeToken { type = Constants.ScalarKind.@float, arity = 2 },
                not null when t == typeof(Vector3) => new VectorTypeToken { type = Constants.ScalarKind.@float, arity = 3 },
                not null when t == typeof(Vector4) => new VectorTypeToken { type = Constants.ScalarKind.@float, arity = 4 },
                not null when t == typeof(Vector2Int) => new VectorTypeToken { type = Constants.ScalarKind.@int, arity = 2 },
                not null when t == typeof(Vector3Int) => new VectorTypeToken { type = Constants.ScalarKind.@int, arity = 3 },
                not null when t == typeof(Matrix4x4) => new MatrixTypeToken { type = Constants.ScalarKind.@float, cols = 4, rows = 4 },
                _ => throw new ArgumentOutOfRangeException(nameof(t), $"Can't express type in hlsl: {t?.FullName}"),
            };

        public static TypeKeyword shaderlabTypeKeyword(this Type t)
            => t switch
            {
                not null when t == typeof(int)                        => new AST.Shaderlab.Syntax.IntKeyword(),
                not null when t == typeof(float) || t == typeof(bool) => new AST.Shaderlab.Syntax.FloatKeyword(),
                not null when t == typeof(Vector2) || t == typeof(Vector3) || t == typeof(Vector4)
                 || t == typeof(Vector2Int) || t == typeof(Vector3Int) => new VectorKeyword(),
                _ => throw new ArgumentOutOfRangeException($"Can't express type in shaderlab: {t?.FullName}"),
            };


        #region attribute helpers

        // some more info can be found here: https://docs.unity3d.com/ScriptReference/MaterialPropertyDrawer.html
        // TODO: assure that strings fit pattern of AttributeStringLiteral
        public static Attribute headerAttribute(string headerName)
            => new()
            {
                id = "Header",
                arguments = (Attribute.Value)headerName.sanitizeToIdentifierString(),
            };

        public static Attribute spaceAttribute()  => new() { id = "Space" };
        public static Attribute toggleAttribute() => new() { id = "Toggle" };

        public static Attribute tooltipAttribute(string tooltip)
            => new()
            {
                id = "Tooltip",
                arguments = (Attribute.Value)tooltip,
            };

        public static Attribute enumAttribute<T>() where T : Enum
            => new()
            {
                id = "Enum",
                arguments = (Attribute.Value)typeof(T).FullName!,
            };

        public static Attribute keyEnumAttribute(params string[] values)
            => new()
            {
                id = "KeyEnum",
                arguments = values.Select(s => new Attribute.Value { value = s }).ToArray(),
            };

        #endregion
    }
}
