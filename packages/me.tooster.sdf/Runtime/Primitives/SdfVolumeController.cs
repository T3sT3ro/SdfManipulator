#nullable enable
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Data;
using me.tooster.sdf.Util;
using Unity.Properties;
using UnityEngine;
using static me.tooster.sdf.AST.Hlsl.Extensions;

namespace me.tooster.sdf.Primitives {
    /// <summary>
    ///     An SDF stored in a 3D texture, e.g. one that Unity's SDF Bake Tool made from a mesh. The texture fills a box
    ///     centered at the object, and its red channel holds the distances.
    /// </summary>
    [GeneratePropertyBag]
    public partial class SdfVolumeController : SdfPrimitiveController {
        static readonly PropertyPath volumeProperty        = new(nameof(Volume));
        static readonly PropertyPath sizeProperty          = new(nameof(Size));
        static readonly PropertyPath distanceScaleProperty = new(nameof(DistanceScale));

        [SerializeField] [DontCreateProperty] Texture3D? volume;
        [SerializeField] [DontCreateProperty] Vector3    size          = Vector3.one;
        [SerializeField] [DontCreateProperty] float      distanceScale = 1;

        [CreateProperty] [ShaderProperty(Description = "The 3D texture whose red channel holds the distances")]
        public Texture3D? Volume {
            get => volume;
            set => SetField(ref volume, value, false);
        }

        [CreateProperty] [ShaderProperty(Description = "The size of the box that the texture fills")]
        public Vector3 Size {
            get => size;
            set => SetField(ref size, value, false);
        }

        [CreateProperty] [ShaderProperty(Description = "The distance that a stored value of 1 stands for. For textures from "
                                                     + "the SDF Bake Tool, the largest side of the size.")]
        public float DistanceScale {
            get => distanceScale;
            set => SetField(ref distanceScale, value, false);
        }

        public override ScalarData Apply(VectorData input, Processor processor) {
            processor.HandleRequirement(new IncludeRequirement(this, SdfPaths.ShaderLibrary.Primitives));

            return new ScalarData
            {
                evaluationExpression = FunctionCall(
                    "sdf::primitives3D::volume",
                    input.evaluationExpression,
                    (Identifier)this[volumeProperty].identifier,
                    (Identifier)this[sizeProperty].identifier,
                    (Identifier)this[distanceScaleProperty].identifier
                ),
            };
        }
    }
}
