using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.Controllers.Data;
using UnityEditor;
using UnityEngine;
using static me.tooster.sdf.AST.Hlsl.Extensions;

namespace me.tooster.sdf.Editor.Controllers.SDF.Primitives {
    [RequireComponent(typeof(TransformController))]
    [ShaderInclude("Packages/me.tooster.sdf/Editor/Resources/Includes/primitives.hlsl")]
    [DisallowMultipleComponent]
    public class SdfSphereController : SdfController {
        internal readonly Property<float> radius = new("radius", "Sphere radius", 1f);

        public override IEnumerable<Property> Properties => base.Properties.Append(radius);

        public override SdfData sdfData => new()
        {
            evaluationExpression = p => FunctionCall(sdfFunctionIdentifier, p.evaluationExpression),
            requiredFunctionDefinitions = new[]
            {
                generateSdfFunction(new SdfData
                {
                    evaluationExpression = inputVector => FunctionCall("sdf::primitives3D::sphere",
                        inputVector.evaluationExpression,
                        new Identifier { id = SdfScene.GetPropertyIdentifier(radius) }
                    ),
                }),
            },
        };

        protected override void Update() {
            if (!transform.hasChanged) return;
            transform.hasChanged = false;
            UpdateTransformProperty();
            UpdateRadiusProperty();
        }

        private void OnDrawGizmosSelected() { }

        internal void UpdateRadiusProperty() {
            // TODO: handle uniform scale, use elipsoid if non-uniform
            transform.localScale = Vector3.one * radius.Value;
            SdfScene.QueuePropertyForUpdate(radius);
        }

        [MenuItem("GameObject/SDF/Primitives/Sphere", priority = -20)]
        public static void CreateSdfBox() => TryCreateController<SdfSphereController>("sphere");
    }

    [CustomEditor(typeof(SdfSphereController), true)]
    internal class SdfSphereControllerEditor : ControllerEditor {
        protected override void OnSceneGUI() {
            base.OnSceneGUI();
            var controller = (SdfSphereController)target;
            using (var scope = new EditorGUI.ChangeCheckScope()) {
                var radius = Handles.RadiusHandle(Quaternion.identity, controller.transform.position, controller.radius.Value);
                if (!scope.changed) return;

                Undo.RecordObject(controller.transform, "Changed SDF Sphere size");
                controller.radius.Value = radius;
                controller.UpdateRadiusProperty();
            }
        }
    }
}
