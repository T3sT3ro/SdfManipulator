#nullable enable
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.Controllers.Data;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
namespace me.tooster.sdf.Editor.Controllers.SDF {
    /**
     * A basic controller for sdf primitives and positionable elements.
     */
    [ExecuteAlways]
    [GeneratePropertyBag]
    public partial class TransformController : Controller, IModifier<VectorData, VectorData> {
        static readonly PropertyPath spaceTransformProperty = new(nameof(SpaceTransform));
        [CreateProperty] [ShaderProperty(Description = "Space Transform")]
        public virtual Matrix4x4 SpaceTransform => transform.localToWorldMatrix.inverse;

        [CreateProperty]
        public Vector3 LocalPosition {
            get => transform.localPosition;
            set {
                if (value == transform.localPosition) return;
                transform.localPosition = value;
                OnPropertyChanged(nameof(SpaceTransform));
            }
        }

        [CreateProperty]
        public Quaternion LocalRotation {
            get => transform.localRotation;
            set {
                if (value == transform.localRotation) return;
                transform.localRotation = value;
                OnPropertyChanged(nameof(SpaceTransform));
            }
        }

        [CreateProperty]
        public Vector3 LocalScale {
            get => transform.localScale;
            set {
                if (value == transform.localScale) return;
                transform.localScale = value;
                OnPropertyChanged(nameof(SpaceTransform));
            }
        }

        void Awake() { TransformUtils.SetConstrainProportions(transform, true); }

        protected virtual void Update() {
            if (!transform.hasChanged) return;
            OnPropertyChanged(nameof(SpaceTransform));
        }

        protected virtual void LateUpdate() {
            if (transform.hasChanged)
                transform.hasChanged = false;
        }

        void OnDrawGizmos() => Gizmos.DrawIcon(transform.position, "Packages/me.tooster.sdf/Editor/Resources/Icons/sdf-icon-256.png", true);

        protected virtual void OnValidate() {
            transform.hasChanged = true;

            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (transform.localScale.x != transform.localScale.y || transform.localScale.y != transform.localScale.z)
                Debug.LogWarning($"Non-unifom component scale in game object '{name}'. This may cause visual bugs", gameObject);
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }


        public VectorData Apply(VectorData input, Processor processor) {
            processor.HandleRequirement(new IncludeRequirement(this, "Packages/me.tooster.sdf/Editor/Resources/Includes/operators.hlsl"));

            return input with
            {
                evaluationExpression = AST.Hlsl.Extensions.FunctionCall(
                    "sdf::operators::transform",
                    input.evaluationExpression,
                    (Identifier)this[spaceTransformProperty].identifier
                ),
            };
        }
    }
}
