#nullable enable
using System.Linq;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Data;
using me.tooster.sdf.Util;
using Unity.Properties;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Extensions = me.tooster.sdf.AST.Hlsl.Extensions;
using Type = System.Type;
namespace me.tooster.sdf {
    /**
     * A basic controller for sdf primitives and positionable elements.
     */
    [ExecuteAlways]
    [GeneratePropertyBag]
    public partial class TransformController : Controller, IModifier<VectorData, VectorData> {
        static readonly PropertyPath spaceTransformProperty = new(nameof(SpaceTransform));

        /// <summary>
        ///     Maps the space of the scene's root to the space of this object. Relative to the root, so moving the whole
        ///     scene only changes the scene's own matrix, not the uniforms of its controllers.
        /// </summary>
        [CreateProperty] [ShaderProperty(Description = "Space Transform")]
        public virtual Matrix4x4 SpaceTransform
            => SdfScene != null ? transform.worldToLocalMatrix * SdfScene.transform.localToWorldMatrix : transform.worldToLocalMatrix;

        /// <summary>
        ///     Converts the distances of the SDF in this object's space into distances in the space of the scene's root:
        ///     the object's scale relative to the root. It's exact for uniform scale and a lower bound otherwise, so
        ///     rays don't march past surfaces of scaled objects.
        /// </summary>
        [CreateProperty] [ShaderProperty(Description = "Distance Scale")]
        public virtual float DistanceScale => DistanceScaleOf(SpaceTransform);

        /// <summary>
        ///     The factor that turns distances in the space that <paramref name="m" /> maps to into lower bounds of the
        ///     distances in the space that it maps from: the inverse of the largest factor by which <paramref name="m" />
        ///     stretches a vector, which is its largest singular value. It's exact for rotations with uniform scale.
        /// </summary>
        public static float DistanceScaleOf(Matrix4x4 m) {
            // the squared singular values are the eigenvalues of the symmetric matrix B = AᵀA, where A is the linear part
            Vector3 c0 = m.GetColumn(0), c1 = m.GetColumn(1), c2 = m.GetColumn(2);
            double b00 = Vector3.Dot(c0, c0), b11 = Vector3.Dot(c1, c1), b22 = Vector3.Dot(c2, c2);
            double b01 = Vector3.Dot(c0, c1), b02 = Vector3.Dot(c0, c2), b12 = Vector3.Dot(c1, c2);
            // the largest eigenvalue of a symmetric 3x3 matrix, with the trigonometric solution of its characteristic cubic
            double largest;
            var offDiagonal = b01 * b01 + b02 * b02 + b12 * b12;
            if (offDiagonal < 1e-12 * (b00 * b00 + b11 * b11 + b22 * b22)) {
                largest = System.Math.Max(b00, System.Math.Max(b11, b22));
            } else {
                var q = (b00 + b11 + b22) / 3;
                var p = System.Math.Sqrt(((b00 - q) * (b00 - q) + (b11 - q) * (b11 - q) + (b22 - q) * (b22 - q) + 2 * offDiagonal) / 6);
                // C = (B - qI) / p, and r = det(C) / 2
                double c00 = (b00 - q) / p, c11 = (b11 - q) / p, c22 = (b22 - q) / p, c01 = b01 / p, c02 = b02 / p, c12 = b12 / p;
                var r = (c00 * (c11 * c22 - c12 * c12) - c01 * (c01 * c22 - c12 * c02) + c02 * (c01 * c12 - c11 * c02)) / 2;
                largest = q + 2 * p * System.Math.Cos(System.Math.Acos(System.Math.Max(-1, System.Math.Min(1, r))) / 3);
            }
            var scale = 1 / System.Math.Sqrt(largest);
            // the Standalone API level of the project has no double.IsFinite
            return double.IsNaN(scale) || double.IsInfinity(scale) ? 1 : (float)scale;
        }

        void OnTransformChanged() {
            OnPropertyChanged(nameof(SpaceTransform));
            OnPropertyChanged(nameof(DistanceScale));
        }

        [CreateProperty]
        public Vector3 LocalPosition {
            get => transform.localPosition;
            set {
                if (value == transform.localPosition)
                    return;
                transform.localPosition = value;
                OnTransformChanged();
            }
        }

        [CreateProperty]
        public Quaternion LocalRotation {
            get => transform.localRotation;
            set {
                if (value == transform.localRotation)
                    return;
                transform.localRotation = value;
                OnTransformChanged();
            }
        }

        [CreateProperty]
        public Vector3 LocalScale {
            get => transform.localScale;
            set {
                if (value == transform.localScale)
                    return;
                transform.localScale = value;
                OnTransformChanged();
            }
        }

        void Awake() {
#if UNITY_EDITOR
            TransformUtils.SetConstrainProportions(transform, true); // inspector-only: lock proportional scaling
#endif
        }

        protected virtual void Update() {
            if (!transform.hasChanged)
                return;
            OnTransformChanged();
        }

        protected virtual void LateUpdate() {
            if (transform.hasChanged)
                transform.hasChanged = false;
        }

        void OnDrawGizmos() => Gizmos.DrawIcon(transform.position, SdfPaths.Icons.Sdf, true);

#if UNITY_EDITOR
        protected override void OnValidate() {
            base.OnValidate();
            transform.hasChanged = true;

            // uniform scale only multiplies the distances, which DistanceScale undoes
            var scale = transform.localScale;
            var smallest = Mathf.Min(Mathf.Abs(scale.x), Mathf.Min(Mathf.Abs(scale.y), Mathf.Abs(scale.z)));
            var largest = Mathf.Max(Mathf.Abs(scale.x), Mathf.Max(Mathf.Abs(scale.y), Mathf.Abs(scale.z)));

            if (largest > 1.001f * smallest) {
                Debug.LogWarning(
                    $"Non-uniform scale on an SDF component '{name}' (local scale is {transform.localScale}). The distances "
                  + "of a stretched SDF aren't exact, so the object marches slower, and blends and roundings stretch with it."
                  + $"\nPath to this component: '{PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject)}' » "
                  + $"{transform.AncestorsAndSelf().Select(t => t.name).JoinToString(" > ")}",
                    this
                );
            }
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }
#endif


        public VectorData Apply(VectorData input, Processor processor) {
            processor.HandleRequirement(new IncludeRequirement(this, SdfPaths.ShaderLibrary.Operators));

            return input with
            {
                evaluationExpression = Extensions.FunctionCall(
                    "sdf::operators::transform",
                    input.evaluationExpression,
                    (Identifier)this[spaceTransformProperty].identifier
                ),
            };
        }

        public override IData Apply(IData input, Processor processor) => Apply((VectorData)input, processor);
        public override Type  GetInputType()                          => typeof(VectorData);
        public override Type  GetOutputType()                         => typeof(VectorData);
    }
}
