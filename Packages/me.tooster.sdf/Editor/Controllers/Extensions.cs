using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using me.tooster.sdf.Editor.API;
using UnityEngine;
using Attribute = me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific.Attribute;
using Type = System.Type;

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
            _                    => throw new ArgumentOutOfRangeException(nameof(p), p, "No type token for this unity type")
        };

        public static Attribute headerAttribute(string headerName) => new Attribute
        {
            id = "Header",
            arguments = (Attribute.Value)headerName
        };

        public static Attribute spaceAttribute() => new Attribute { id = "Space" };
    }

    public static class Extensions {
        /// <summary>
        /// Collects all variables present on the node that should be exposed on the material as properties
        /// </summary>
        [Obsolete("this was taken from the previous graph model, not needed for now")]
        public static IEnumerable<Property> CollectProperties(object o) {
            return o.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(f => f.FieldType.IsSubclassOf(typeof(Property)))
                .SelectMany(f => (IEnumerable<Property>)f.GetValue(o))
                .Where(property => property is not null);
        }

        // TODO: remove or replace with getter instead of attribute?
        public static ISet<string> CollectIncludes(Type type) {
            return type.GetCustomAttributes(typeof(ShaderIncludeAttribute), true)
                .OfType<ShaderIncludeAttribute>()
                .SelectMany(attr => attr.ShaderIncludes)
                .Where(property => property is not null)
                .ToHashSet();
        }

        // TODO: remove or replace with getter instead of attribute?
        public static ISet<string> CollectDefines(object o) {
            // return values of static fields annotated with ShaderDefineAttribute
            return o.GetType().GetFields(BindingFlags.Static)
                .Where(info => info.GetCustomAttributes<ShaderGlobalAttribute>().Any())
                .Select(field => (string)field.GetValue(o))
                .Where(property => property is not null)
                .ToHashSet();
        }
    }
}
