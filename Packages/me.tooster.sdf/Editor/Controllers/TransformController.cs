#nullable enable
using System.Collections.Generic;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using me.tooster.sdf.Editor.API;
using UnityEditor;
using UnityEngine;
using Extensions = me.tooster.sdf.Editor.API.Extensions;

namespace me.tooster.sdf.Editor.Controllers {
    /**
     * A basic controller for sdf primitives and positionable elements.
     */
    [ExecuteInEditMode]
    [RequireComponent(typeof(Transform))]
    public class TransformController : Controller {
        // TODO: consolidate variables and properties? Should properties be responsible for their final values
        public Vector3 size = Vector3.one / 4;
        
        // IMPORTANT: transform property should be an INVERSE, because we transform the space, not the object 
        private Property<Matrix4x4> transformProperty =
            new Property<Matrix4x4>("transform", "Transform", Matrix4x4.identity);


        public override IEnumerable<Property> Properties {
            get { yield return transformProperty; }
        }

        private void Update() {
            if (SdfScene == null) return;

            var spaceTransform = Matrix4x4.Rotate(Quaternion.Inverse(transform.localRotation));
            spaceTransform *=
                Matrix4x4.Translate(Vector3.Scale(-transform.localPosition, SdfScene.transform.localScale));

            transformProperty.UpdateValue(this, spaceTransform);
        }

        // TODO: introduce "*Data" interface as a contract with controlled Slots that take in correct data?
        public Expression<hlsl> SpaceTransformExpression(Expression<hlsl> coordinateExpression) =>
            AST.Hlsl.Extensions.FunctionCall("mul", coordinateExpression,
                AST.Hlsl.Extensions.FunctionCall("float4",
                    new Identifier { id = SdfScene.GetIdentifier(transformProperty) },
                    new LiteralExpression { literal = (IntLiteral)1 }
                )
            );
    }

    [UnityEditor.CustomEditor(typeof(TransformController))]
    public class TransformControllerGui : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var controller = (TransformController)target;
            Handles.color = Color.red;
            Handles.DrawWireCube(Vector3.zero, controller.size);
        }
    }
}
