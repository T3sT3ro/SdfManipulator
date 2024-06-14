using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.Controllers.Data;
using Unity.Properties;
using UnityEngine;
using static me.tooster.sdf.AST.Hlsl.Extensions;
using Type = System.Type;

namespace me.tooster.sdf.Editor.Controllers.SDF.Operators {
    /// Cheaply bend the space
    [GeneratePropertyBag]
    public partial class SdfBendController : Controller, IModifier<VectorData, VectorData> {
        static readonly                       PropertyPath bendFactorProperty = new(nameof(BendFactor));
        [SerializeField] [DontCreateProperty] float        bendFactor;

        [CreateProperty] [ShaderProperty(Description = "Bend factor")]
        public float BendFactor {
            get => bendFactor;
            set => SetField(ref bendFactor, value, false);
        }

        public VectorData Apply(VectorData input, Processor processor) {
            processor.HandleRequirement(new IncludeRequirement(this, "Packages/me.tooster.sdf/Editor/Resources/Includes/operators.hlsl"));

            return new VectorData
            {
                evaluationExpression = FunctionCall(
                    "sdf::operators::cheap_bend",
                    input.evaluationExpression,
                    (Identifier)this[bendFactorProperty].identifier
                ),
            };
        }

        public override IData Apply(IData input, Processor processor) => Apply((VectorData)input, processor);
        public override Type  GetInputType()                          => typeof(VectorData);
        public override Type  GetOutputType()                         => typeof(VectorData);
    }
}
