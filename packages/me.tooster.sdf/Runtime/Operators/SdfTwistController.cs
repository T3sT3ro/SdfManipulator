#nullable enable
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Data;
using me.tooster.sdf.Util;
using Unity.Properties;
using UnityEngine;
using Extensions = me.tooster.sdf.AST.Hlsl.Extensions;

namespace me.tooster.sdf.Operators {
    /// <summary>
    ///     Twists the space around the y axis by <see cref="Amount" /> radians per unit of height. A twist bends distances,
    ///     so strong twists may need a smaller hit distance or more steps.
    /// </summary>
    [AddComponentMenu("SDF/Operators/Twist")]
    [GeneratePropertyBag]
    public partial class SdfTwistController : SdfDomainOperator {
        static readonly PropertyPath amountProperty = new(nameof(Amount));

        [SerializeField] [DontCreateProperty] float amount = 1;

        /// The rotation in radians for each unit along the y axis
        [CreateProperty] [ShaderProperty(Description = "Twist amount")]
        public float Amount {
            get => amount;
            set => SetField(ref amount, value, false);
        }

        public override VectorData Apply(VectorData input, Processor processor) {
            processor.HandleRequirement(new IncludeRequirement(this, SdfPaths.ShaderLibrary.Operators));
            return input with
            {
                evaluationExpression = Extensions.FunctionCall("sdf::operators::twist", input.evaluationExpression, (Identifier)this[amountProperty].identifier),
            };
        }
    }
}
