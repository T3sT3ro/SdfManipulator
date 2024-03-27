#nullable enable
using System;
using System.Collections.Generic;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.Controllers.Data;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Editor.Controllers.SDF {
    /**
     * A basic controller for sdf primitives and positionable elements.
     */
    [ExecuteAlways]
    public class TransformController : Controller {
        // IMPORTANT: transform property should be an INVERSE, because we transform the space, not the object 
        private readonly Property<Matrix4x4> transformProperty = new("transform", "Transform", Matrix4x4.identity)
            { IsExposed = false };

        public override IEnumerable<Property> Properties {
            get { yield return transformProperty; }
        }

        // TODO: mark properties for update in update, emit the regeneration events in LateUpdate
        protected virtual void Update() {
            if (SdfScene == null) return;
            if (!transform.hasChanged) return;
            transform.hasChanged = false;
            UpdateTransformProperty();
        }

        protected virtual void OnValidate() { transform.hasChanged = true; }

        protected void UpdateTransformProperty() {
            /* // ORIGINAL space transformation logic for über shader and invariant scale handling
             var spaceTransform = Matrix4x4.Rotate(Quaternion.Inverse(transform.localRotation));
            spaceTransform *= Matrix4x4.Translate(Vector3.Scale(-transform.localPosition, SdfScene.transform.localScale));
            */

            var spaceTransform = Matrix4x4.Rotate(Quaternion.Inverse(transform.rotation));
            spaceTransform *= Matrix4x4.Translate(-transform.position);

            transformProperty.Value = spaceTransform;
            SdfScene.QueuePropertyUpdates(transformProperty);
        }

        protected VectorData ApplyTransform(VectorData vd) => new()
        {
            evaluationExpression = AST.Hlsl.Extensions.FunctionCall("sdf::operators::transform",
                vd.evaluationExpression, new Identifier { id = SdfScene.GetPropertyIdentifier(transformProperty) }
            ),
        };

        private void OnDrawGizmos() {
            Gizmos.DrawIcon(transform.position, "Packages/me.tooster.sdf/Editor/Resources/Icons/sdf-icon-256.png", true);
        }

        private void OnDrawGizmosSelected() {
            // if (Selection.activeGameObject != gameObject) return;

            Gizmos.color = Color.magenta;
            var tr = transform;
            var uniformScaleTransformMatrix = Matrix4x4.TRS(transform.position, transform.rotation, tr.localScale);
            Gizmos.matrix = uniformScaleTransformMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
    }
}
