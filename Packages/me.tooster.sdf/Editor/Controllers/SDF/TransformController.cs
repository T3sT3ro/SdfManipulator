#nullable enable
using System.Collections.Generic;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.Controllers.Data;
using UnityEngine;

namespace me.tooster.sdf.Editor.Controllers.SDF {
    /**
     * A basic controller for sdf primitives and positionable elements.
     */
    [ExecuteInEditMode]
    [RequireComponent(typeof(Transform))]
    public class TransformController : Controller {
        // IMPORTANT: transform property should be an INVERSE, because we transform the space, not the object 
        private readonly Property<Matrix4x4> transformProperty = new("transform", "Transform", Matrix4x4.identity)
            { IsExposed = false };
        // TODO: handle local scale

        public override IEnumerable<Property> Properties {
            get { yield return transformProperty; }
        }

        private void Update() {
            if (SdfScene == null) return;

            /* // ORIGINAL space transformation logic for über shader and invariant scale handling
             var spaceTransform = Matrix4x4.Rotate(Quaternion.Inverse(transform.localRotation));
            spaceTransform *= Matrix4x4.Translate(Vector3.Scale(-transform.localPosition, SdfScene.transform.localScale));
            */

            var spaceTransform = Matrix4x4.Rotate(Quaternion.Inverse(transform.rotation));
            spaceTransform *= Matrix4x4.Translate(-transform.position);


            transformProperty.UpdateValue(transformProperty, spaceTransform);
        }

        protected VectorData ApplyTransform(VectorData vd) => new()
        {
            evaluationExpression = AST.Hlsl.Extensions.FunctionCall("sdf::operators::transform",
                vd.evaluationExpression, new Identifier { id = SdfScene.GetPropertyIdentifier(transformProperty) }
            ),
        };
    }
}
