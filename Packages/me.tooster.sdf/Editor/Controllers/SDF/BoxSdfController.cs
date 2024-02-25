using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.Controllers.Data;
using UnityEditor;
using UnityEngine;
using static me.tooster.sdf.AST.Hlsl.Extensions;

namespace me.tooster.sdf.Editor.Controllers.SDF {
    [ShaderInclude("Packages/me.tooster.sdf/Editor/Resources/Includes/primitives.hlsl")]
    public class BoxSdfController : SdfPrimitiveController {
        private readonly Property<Vector3> boxSize = new("boxsize", "Box size", Vector3.one / 4);

        public override IEnumerable<Property> Properties => base.Properties.Append(boxSize);

        public override SdfData sdfData => new()
        {
            evaluationExpression = p => FunctionCall(primitiveMethodIdentifier, p.evaluationExpression),
            requiredFunctionDefinitions = new[]
            {
                generatePrimitiveFunction(new SdfData
                {
                    evaluationExpression = inputVector => FunctionCall(
                        "sdf::primitives3D::box",
                        inputVector.evaluationExpression,
                        new Identifier { id = SdfScene.GetPropertyIdentifier(boxSize) }
                    ),
                }),
            },
        };

        [MenuItem("GameObject/SDF/Primitives/Box", priority = 200)]
        public static void CreateSdfBox() => TryCreateNode<BoxSdfController>("box");
    }
}
