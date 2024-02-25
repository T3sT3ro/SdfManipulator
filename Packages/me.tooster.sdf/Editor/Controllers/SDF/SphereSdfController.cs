using System;
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.Controllers.Data;
using UnityEditor;
using UnityEngine;
using static me.tooster.sdf.AST.Hlsl.Extensions;

namespace me.tooster.sdf.Editor.Controllers.SDF {
    [RequireComponent(typeof(TransformController))]
    [ShaderInclude("Packages/me.tooster.sdf/Editor/Resources/Includes/primitives.hlsl")]
    public class SphereSdfController : SdfPrimitiveController {
        private readonly Property<float> radius = new("radius", "Sphere radius", 1f);

        public override IEnumerable<Property> Properties => base.Properties.Append(radius);

        public override SdfData sdfData => new()
        {
            evaluationExpression = p => FunctionCall(primitiveMethodIdentifier, p.evaluationExpression),
            requiredFunctionDefinitions = new[]
            {
                generatePrimitiveFunction(new SdfData
                {
                    evaluationExpression = inputVector => FunctionCall("sdf::primitives3D::sphere",
                        inputVector.evaluationExpression,
                        new Identifier { id = SdfScene.GetPropertyIdentifier(radius) }
                    ),
                }),
            },
        };

        [MenuItem("GameObject/SDF/Primitives/Sphere", priority = -20)]
        public static void CreateSdfBox() => TryCreateNode<SphereSdfController>("box");
    }
}
