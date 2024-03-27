using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.Controllers.Data;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using static me.tooster.sdf.AST.Hlsl.Extensions;

namespace me.tooster.sdf.Editor.Controllers.SDF.Primitives {
    [ShaderInclude("Packages/me.tooster.sdf/Editor/Resources/Includes/primitives.hlsl")]
    [DisallowMultipleComponent]
    public class SdfBoxController : SdfController {
        private Bounds boxBounds = new(Vector3.zero, Vector3.one);

        private readonly Property<Vector3> boxSize = new("boxsize", "Box size", Vector3.one / 4);

        public override IEnumerable<Property> Properties => base.Properties.Append(boxSize);

        public override SdfData sdfData => new()
        {
            evaluationExpression = p => FunctionCall(sdfFunctionIdentifier, p.evaluationExpression),
            requiredFunctionDefinitions = new[]
            {
                generateSdfFunction(new SdfData
                {
                    evaluationExpression = inputVector => FunctionCall(
                        "sdf::primitives3D::box",
                        inputVector.evaluationExpression,
                        new Identifier { id = SdfScene.GetPropertyIdentifier(boxSize) }
                    ),
                }),
            },
        };

        protected override void Update() {
            if (!transform.hasChanged) return;
            transform.hasChanged = false;
            UpdateTransformProperty();
            UpdateBoxSizeProperty();
        }

        private void UpdateBoxSizeProperty() {
            boxSize.Value = transform.localScale / 2;
            SdfScene.QueuePropertyUpdates(boxSize);
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = GizmoColor;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }

        [MenuItem("GameObject/SDF/Primitives/Box", priority = 200)]
        public static void CreateSdfBox() => TryCreateController<SdfBoxController>("box");
    }

    [CustomEditor(typeof(SdfBoxController), true)]
    internal class SdfBoxControllerEditor : ControllerEditor {
        private BoxBoundsHandle boxHandle = new();
        protected override void OnSceneGUI() {
            base.OnSceneGUI();
            var controller = (SdfBoxController)target;
            var tr = controller.transform;

            using (var check = new EditorGUI.ChangeCheckScope()) {
                Handles.matrix = Matrix4x4.TRS(tr.position, tr.rotation, Vector3.one);
                boxHandle.center = Vector3.zero;
                boxHandle.size = tr.localScale;
                boxHandle.DrawHandle();
                if (check.changed)
                    Undo.RecordObject(controller.transform, "Change SDF Box bounds");
                tr.localPosition += tr.rotation * boxHandle.center / 2;
                // TODO: possibly handle size and transform scale produce weird values
                tr.localScale = boxHandle.size;
            }
        }
    }
}
