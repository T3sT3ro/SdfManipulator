#nullable enable
using System.Linq;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.Controllers.Data;
using Unity.Properties;
using UnityEngine;
namespace me.tooster.sdf.Editor.Controllers.SDF {
    /**
     * A basic controller for sdf primitives and positionable elements.
     */
    [ExecuteAlways]
    public partial class SdfTransformController : Controller {
        static readonly PropertyPath spaceTransformPropertyPath = new(nameof(SpaceTransform));
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

        protected virtual void Update() {
            if (SdfScene == null) return;
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
            transform.localScale = Vector3.one;
        }

        protected VectorData TransformVectorData(VectorData vd)
            => vd with
            {
                evaluationExpression = AST.Hlsl.Extensions.FunctionCall(
                    "sdf::operators::transform",
                    vd.evaluationExpression,
                    new Identifier
                    {
                        id = SdfScene.sceneData.controllers[this].properties[spaceTransformPropertyPath].identifier,
                    }
                ),
                Requirements = vd.Requirements.Append(
                    new HlslIncludeFileRequirement("Packages/me.tooster.sdf/Editor/Resources/Includes/operators.hlsl")
                ),
            };
    }
}
