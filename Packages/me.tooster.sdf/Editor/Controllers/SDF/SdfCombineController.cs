using System;
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using me.tooster.sdf.Editor.Controllers.Data;
using me.tooster.sdf.Editor.Util.Controllers;
using Unity.Properties;
using UnityEngine;
using static me.tooster.sdf.Editor.Controllers.SDF.SdfCombineController.CombinationOperation;


namespace me.tooster.sdf.Editor.Controllers.SDF {
    [GeneratePropertyBag]
    public partial class SdfCombineController : SdfController {
        public enum CombinationOperation {
            [InspectorName("Simple union")] SIMPLE_UNION = 0,
            [InspectorName("Intersection")] INTERSECTION = 1,
            [InspectorName("Smooth union")] SMOOTH_UNION = 2,
        }



        static readonly PropertyPath blendFactorPropertyPath = new(nameof(BlendFactor));


        [SerializeField] [DontCreateProperty] CombinationOperation operation = SIMPLE_UNION;

        [SerializeField] [DontCreateProperty] float blendFactor = 1;

        [CreateProperty] [ShaderStructural]
        public CombinationOperation Operation {
            get => operation;
            set => SetField(ref operation, value, true);
        }

        [CreateProperty] [ShaderProperty(Description = "Blend factor")]
        public float BlendFactor {
            get => blendFactor;
            set => SetField(ref blendFactor, value, false);
        }

        [Tooltip("If enabled, one SdfResult.id will return only one value for this ")]
        [SerializeField] [DontCreateProperty] bool singleId;

        [CreateProperty] [ShaderStructural]
        public bool SingleId {
            get => singleId;
            set => SetField(ref singleId, value, true);
        }

        [CreateProperty]
        public IEnumerable<ISdfDataSource> SdfControllers => GetFirstNestedComponents<ISdfDataSource>(transform);


        public override SdfData sdfData {
            get {
                var merged = Combinators.binaryCombine(
                    MergeData,
                    SdfControllers.Select(p => p.sdfData).ToArray()
                );

                var functionBody = new List<Statement<hlsl>>();
                // SdfRestult result = <whatever MergeData evaluation expression is>
                functionBody.Add(AST.Hlsl.Extensions.Var(SdfData.sdfReturnType, "result", merged.evaluationExpression(SdfData.pData)));
                // if inverted => result.distance = -result.distance;
                if (Inverted) {
                    functionBody.Add(
                        AST.Hlsl.Extensions.Assignment(
                            $"result.{SdfData.sdfResultDistanceMemberName}",
                            new Unary
                            {
                                operatorToken = new MinusToken(),
                                expression = AST.Hlsl.Extensions.LValue($"result.{SdfData.sdfResultDistanceMemberName}"),
                            }
                        )
                    );
                }
                // if SingleId => 
                if (SingleId) {
                    functionBody.Add(
                        AST.Hlsl.Extensions.Assignment(
                            $"result.{SdfData.sdfResultIdMemberName}",
                            HlslExtensions.VectorConstructor(0, 0, 0, SdfScene.sceneData.controllers[this].numericId)
                        )
                    );
                }
                functionBody.Add((Return)new Identifier { id = "result" });

                var sdfCombinationFunction = SdfData.createSdfFunction(controllerIdentifier, functionBody);

                return new SdfData
                {
                    evaluationExpression = vd => AST.Hlsl.Extensions.FunctionCall(controllerIdentifier, vd.evaluationExpression),
                    Requirements = merged.Requirements.Append(
                        new HlslIncludeFileRequirement("Packages/me.tooster.sdf/Editor/Resources/Includes/primitives.hlsl")
                    ).Append(
                        new HlslFunctionRequirement { functionDefinition = sdfCombinationFunction }
                    ),
                };
            }
        }

        static IEnumerable<T> GetFirstNestedComponents<T>(Transform root) {
            foreach (Transform child in root) {
                if (child.GetComponent<T>() is { } component)
                    yield return component;
                else {
                    foreach (var c in GetFirstNestedComponents<T>(child))
                        yield return c;
                }
            }
        }

        SdfData MergeData(SdfData d1, SdfData d2)
            => Operation switch
            {
                SIMPLE_UNION => Combinators.CombineWithExternalFunction("sdf::operators::unionSimple", d1, d2),
                INTERSECTION => Combinators.CombineWithExternalFunction("sdf::operators::intersectSimple", d1, d2),
                SMOOTH_UNION => Combinators.CombineWithExternalFunction(
                    "sdf::operators::unionSmooth",
                    d1,
                    d2,
                    (Identifier)this[blendFactorPropertyPath].identifier
                ),
                _ => throw new ArgumentOutOfRangeException(),
            };
    }
}
