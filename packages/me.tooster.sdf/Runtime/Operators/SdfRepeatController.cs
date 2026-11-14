#nullable enable
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Data;
using me.tooster.sdf.Util;
using Unity.Properties;
using UnityEngine;
using Extensions = me.tooster.sdf.AST.Hlsl.Extensions;

namespace me.tooster.sdf.Operators {
    /// <summary>
    ///     Repeats the space in cells of <see cref="Period" />, so the modifiers after it in the SDF stack, e.g. a primitive,
    ///     appear in every cell. A limited repetition makes <see cref="Count" /> copies on each side of the original on
    ///     each axis.
    /// </summary>
    [AddComponentMenu("SDF/Operators/Repeat")]
    [GeneratePropertyBag]
    public partial class SdfRepeatController : SdfDomainOperator {
        static readonly PropertyPath periodProperty = new(nameof(Period));
        static readonly PropertyPath countProperty  = new(nameof(Count));

        [SerializeField] [DontCreateProperty] Vector3 period = Vector3.one * 2;
        [SerializeField] [DontCreateProperty] bool    limited = true;
        [SerializeField] [DontCreateProperty] Vector3 count   = Vector3.one;

        /// The size of a cell on each axis
        [CreateProperty] [ShaderProperty(Description = "Period")]
        public Vector3 Period {
            get => period;
            set => SetField(ref period, value, false);
        }

        /// Whether the repetition stops after <see cref="Count" /> copies on each side
        [CreateProperty] [ShaderStructural]
        public bool Limited {
            get => limited;
            set => SetField(ref limited, value, true);
        }

        /// The number of copies on each side of the original on each axis, for a limited repetition
        [CreateProperty] [ShaderProperty(Description = "Count")]
        public Vector3 Count {
            get => count;
            set => SetField(ref count, value, false);
        }

        public override VectorData Apply(VectorData input, Processor processor) {
            processor.HandleRequirement(new IncludeRequirement(this, SdfPaths.ShaderLibrary.Operators));
            return input with
            {
                evaluationExpression = Limited
                    ? Extensions.FunctionCall(
                        "sdf::operators::repeatLimited",
                        input.evaluationExpression,
                        (Identifier)this[periodProperty].identifier,
                        (Identifier)this[countProperty].identifier
                    )
                    : Extensions.FunctionCall("sdf::operators::repeat", input.evaluationExpression, (Identifier)this[periodProperty].identifier),
            };
        }

        void OnDrawGizmosSelected() {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = new Color(1, 1, 1, 0.3f);
            var cells = limited ? count : Vector3.zero;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.Scale(period, cells * 2 + Vector3.one));
        }
    }
}
