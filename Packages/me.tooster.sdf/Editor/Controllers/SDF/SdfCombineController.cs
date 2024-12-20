using System;
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using me.tooster.sdf.Editor.Controllers.Data;
using me.tooster.sdf.Editor.Util.Controllers;
using Unity.Properties;
using UnityEngine;
using static me.tooster.sdf.Editor.Controllers.SDF.SdfCombineController.CombinationOperation;
using Extensions = me.tooster.sdf.AST.Hlsl.Extensions;


namespace me.tooster.sdf.Editor.Controllers.SDF {
    [GeneratePropertyBag]
    public partial class SdfCombineController : SdfController {
        public enum CombinationOperation {
            [InspectorName("Simple union")] SIMPLE_UNION = 0,
            [InspectorName("Intersection")] INTERSECTION = 1,
            [InspectorName("Smooth union")] SMOOTH_UNION = 2,
        }



        static readonly PropertyPath blendFactorProperty = new(nameof(BlendFactor));


        [SerializeField] [DontCreateProperty] CombinationOperation operation   = SIMPLE_UNION;
        [SerializeField] [DontCreateProperty] float                blendFactor = 1;
        [Tooltip("Generate one ID for the combination")]
        [SerializeField] [DontCreateProperty] bool singleId;

        [CreateProperty] [ShaderStructural]
        public CombinationOperation Operation {
            get => operation;
            set => SetField(ref operation, value, true);
        }

        [CreateProperty] [ShaderProperty(Description = "Blend factor")]
        [VisibleWhen(nameof(operation), (int)SMOOTH_UNION)]
        public float BlendFactor {
            get => blendFactor;
            set => SetField(ref blendFactor, value, false);
        }

        [CreateProperty] [ShaderStructural]
        public bool SingleId {
            get => singleId;
            set => SetField(ref singleId, value, true);
        }

        public Controller[] children = Array.Empty<Controller>();

        void OnTransformChildrenChanged() {
            refreshChildren();
            OnStructureChanged();
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

        protected override void OnEnable() {
            base.OnEnable();
            refreshChildren();
        }

        protected override void OnValidate() {
            base.OnValidate();
            refreshChildren();
            foreach (var child in children) {
                child.StructureChanged -= OnStructureChanged;
                child.StructureChanged += OnStructureChanged;
            }
        }

        void refreshChildren() {
            children = GetFirstNestedComponents<IModifier<VectorData, SdfData>>(transform)
                .OfType<Controller>()
                .Where(c => c != null) // checks if child was destroyed
                .ToArray();
        }

        public override SdfData Apply(VectorData input, Processor processor) {
            if (children.Length == 0)
                throw new ArgumentException("SdfCombineController must have at least one child");

            processor.HandleRequirement(new IncludeRequirement(this, "Packages/me.tooster.sdf/Editor/Resources/Includes/operators.hlsl"));

            var combinatorFunction = Operation switch
            {
                SIMPLE_UNION => "sdf::operators::unionSimple",
                INTERSECTION => "sdf::operators::intersectSimple",
                SMOOTH_UNION => "sdf::operators::unionSmooth",
                _            => throw new ArgumentOutOfRangeException(),
            };

            var result = (Identifier)"result";
            // SdfResult result = <children[0]>(<input>);
            var body = new List<Statement<hlsl>>
            {
                Extensions.Var(
                    SdfData.sdfReturnType,
                    result.id.Text,
                    ((SdfData)children[0].Apply(input, processor)).evaluationExpression
                ),
            };

            // arguments[1] is mutating slot for successive sdfResults 
            var arguments = Operation == SMOOTH_UNION
                ? new Expression<hlsl>[] { result, null, (Identifier)this[blendFactorProperty].identifier }
                : new Expression<hlsl>[] { result, null };

            foreach (var sdfChild in children.Skip(1)) {
                arguments[1] = ((SdfData)sdfChild.Apply(input, processor)).evaluationExpression;
                // result = <combine>(result, <primitive>(p), [optional blendFactor])
                body.Add(
                    Extensions.Assignment(
                        result.id.Text,
                        Extensions.FunctionCall(combinatorFunction, arguments)
                    )
                );
            }

            // if inverted => result.distance *= -1;
            if (Inverted) {
                body.Add(
                    Extensions.Assignment(
                        $"result.{SdfData.sdfResultDistanceMemberName}",
                        new Unary { operatorToken = new MinusToken(), expression = (LiteralExpression)1 },
                        new AsteriskEqualsToken()
                    )
                );
            }

            if (SingleId) {
                body.Add(
                    Extensions.Assignment(
                        $"result.{SdfData.sdfResultIdMemberName}",
                        HlslExtensions.VectorConstructor(0, 0, 0, SdfScene.sceneData.controllers[this].numericId)
                    )
                );
            }


            // return result;
            body.Add(new Return { expression = new Identifier { id = result.id.Text } });

            var combineFunctionName = SdfScene.sceneData.controllers[this].identifier;
            processor.HandleRequirement(new FunctionDefinitionRequirement(this, createSdfFunction(combineFunctionName, body)));

            return new SdfData
            {
                evaluationExpression = Extensions.FunctionCall(combineFunctionName, (Identifier)SdfData.pParamName),
            };
        }
    }
}
