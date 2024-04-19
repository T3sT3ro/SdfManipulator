using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.Controllers.Data;
using Unity.Properties;
using UnityEngine;
using static me.tooster.sdf.AST.Hlsl.Extensions;

namespace me.tooster.sdf.Editor.Controllers.SDF {
    public partial class SdfSphereController : SdfController {
        static readonly                       PropertyPath radiusPropertyPath = new(nameof(Radius));
        [SerializeField] [DontCreateProperty] float        radius             = 1f;

        [CreateProperty] [ShaderProperty(Description = "Sphere radius")]
        public float Radius {
            get => radius;
            set => SetField(ref radius, value, false);
        }


        public override SdfData sdfData => new()
        {
            evaluationExpression = p => FunctionCall(sdfFunctionIdentifier, p.evaluationExpression),
            Requirements = new API.Data.Requirement[]
            {
                new HlslIncludeFileRequirement("Packages/me.tooster.sdf/Editor/Resources/Includes/primitives.hlsl"),
                createSdfFunctionRequirement(
                    p => new ScalarData()
                    {
                        evaluationExpression = FunctionCall(
                            "sdf::primitives3D::sphere",
                            p.evaluationExpression,
                            new Identifier { id = SdfScene.sceneData.controllers[this].properties[radiusPropertyPath].identifier }
                        ),
                    }
                ),
            },
        };
    }
}
