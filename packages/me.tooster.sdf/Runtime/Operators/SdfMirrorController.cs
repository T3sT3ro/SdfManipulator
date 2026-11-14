#nullable enable
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Data;
using me.tooster.sdf.Util;
using Unity.Properties;
using UnityEngine;
using Extensions = me.tooster.sdf.AST.Hlsl.Extensions;

namespace me.tooster.sdf.Operators {
    /// <summary>
    ///     Mirrors the space across the planes of the chosen axes, so the modifiers after it in the SDF stack show on both
    ///     sides of each plane, e.g. a symmetric shape from half of it. The axes are a uniform, so changing them doesn't
    ///     regenerate the shader.
    /// </summary>
    [AddComponentMenu("SDF/Operators/Mirror")]
    [GeneratePropertyBag]
    public partial class SdfMirrorController : SdfDomainOperator {
        static readonly PropertyPath axesProperty = new(nameof(Axes));

        [SerializeField] [DontCreateProperty] bool x = true;
        [SerializeField] [DontCreateProperty] bool y;
        [SerializeField] [DontCreateProperty] bool z;

        /// 1 for each axis to mirror across, 0 for the others
        [CreateProperty] [ShaderProperty(Description = "Mirrored axes")]
        public Vector3 Axes => new(x ? 1 : 0, y ? 1 : 0, z ? 1 : 0);

        public bool MirrorX {
            get => x;
            set => SetField(ref x, value, false, nameof(Axes));
        }

        public bool MirrorY {
            get => y;
            set => SetField(ref y, value, false, nameof(Axes));
        }

        public bool MirrorZ {
            get => z;
            set => SetField(ref z, value, false, nameof(Axes));
        }

        public override VectorData Apply(VectorData input, Processor processor) {
            processor.HandleRequirement(new IncludeRequirement(this, SdfPaths.ShaderLibrary.Operators));
            return input with
            {
                evaluationExpression = Extensions.FunctionCall("sdf::operators::mirror", input.evaluationExpression, (Identifier)this[axesProperty].identifier),
            };
        }
    }
}
