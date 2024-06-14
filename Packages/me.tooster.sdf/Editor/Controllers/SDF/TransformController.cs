#nullable enable
using System.Linq;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.Controllers.Data;
using me.tooster.sdf.Util;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using Type = System.Type;
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

        protected override void OnValidate() {
            base.OnValidate();
            transform.hasChanged = true;

            // mean square error of transform.localScale components and 1 to ensure uniform scale
            var error = (transform.localScale - Vector3.one).sqrMagnitude;

            if (Mathf.Abs(error) > 0.001f) {
                Debug.LogWarning(
                    $"Non-unifom scale on an SDF component '{name}' (local scale is {transform.localScale}). This may cause visual bugs."
                  + $"\nPath to this component: '{PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject)}' » "
                  + $"{transform.AncestorsAndSelf().Select(t => t.name).JoinToString(" > ")}",
                    this
                );
            }
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

        public override IData Apply(IData input, Processor processor) => Apply((VectorData)input, processor);
        public override Type  GetInputType()                          => typeof(VectorData);
        public override Type  GetOutputType()                         => typeof(VectorData);
    }
}
