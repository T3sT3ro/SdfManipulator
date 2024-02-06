using System;
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
            _          => throw new ArgumentOutOfRangeException(nameof(p), p, "No type token for this unity type")
        };

        public static Attribute headerAttribute(string headerName) => new Attribute
        {
            id = "Header",
            arguments = (Attribute.Value)headerName
        };

        public static Attribute spaceAttribute() => new Attribute { id = "Space" };
    }
}
