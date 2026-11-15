using me.tooster.sdf.Primitives;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
namespace me.tooster.sdf.Editor.Inspectors.Primitives {
    [CustomEditor(typeof(SdfTorusController), true)]
    class SdfTorusControllerEditor : SdfPrimitiveControllerEditor {
        // two ring handles for major and minor radius of torus
        readonly SphereBoundsHandle mainRadiusHandle = new();
        readonly SphereBoundsHandle ringRadiusHandle = new();

        protected override void OnSceneGUI() {
            base.OnSceneGUI();
            var controller = (SdfTorusController)target;

            using (new Handles.DrawingScope(controller.transform.localToWorldMatrix)) {
                using (var mainRadiusCheck = new EditorGUI.ChangeCheckScope()) {
                    mainRadiusHandle.radius = controller.MainRadius;
                    mainRadiusHandle.axes = PrimitiveBoundsHandle.Axes.All ^ PrimitiveBoundsHandle.Axes.Y;
                    mainRadiusHandle.center = Vector3.zero; // TODO: remove alltogether and the handle will be pinned to the opposite side
                    // TODO, depending on player prefs, use the this to display on the surface: Vector3.up * controller.RingRadius;

                    mainRadiusHandle.DrawHandle();

                    if (mainRadiusCheck.changed) {
                        Undo.RecordObject(controller, "Resize torus");
                        controller.MainRadius = mainRadiusHandle.radius;
                    }
                }

                using (var ringRadiusCheck = new EditorGUI.ChangeCheckScope()) {
                    ringRadiusHandle.radius = controller.RingRadius;
                    ringRadiusHandle.axes = PrimitiveBoundsHandle.Axes.All ^ PrimitiveBoundsHandle.Axes.X;
                    ringRadiusHandle.center = Vector3.forward * controller.MainRadius;

                    ringRadiusHandle.DrawHandle();

                    if (ringRadiusCheck.changed) {
                        Undo.RecordObject(controller, "Resize torus");
                        controller.RingRadius = ringRadiusHandle.radius;
                    }
                }

                if (controller.Variant == SdfTorusController.TorusVariant.CAPPED)
                    DrawCapHandles(controller);
            }
        }

        /// <summary>
        ///     Draws the arc that a capped torus keeps, on its main radius: <see cref="SdfTorusController.Cap" /> radians on
        ///     both sides of the z axis. A handle at each end of the arc sets the cap, so the end follows the cursor.
        /// </summary>
        static void DrawCapHandles(SdfTorusController controller) {
            var radius = controller.MainRadius;
            var cap = Mathf.Clamp(controller.Cap, 0, Mathf.PI);
            var start = ArcEnd(cap, -1);

            using (new Handles.DrawingScope(Handles.color * new Color(1, 1, 1, 0.1f)))
                Handles.DrawSolidArc(Vector3.zero, Vector3.up, start, 2 * cap * Mathf.Rad2Deg, radius);
            Handles.DrawWireArc(Vector3.zero, Vector3.up, start, 2 * cap * Mathf.Rad2Deg, radius);

            foreach (var side in new[] { -1f, 1f }) {
                var end = ArcEnd(cap, side) * radius;
                Handles.DrawLine(Vector3.zero, end);

                using var capCheck = new EditorGUI.ChangeCheckScope();
                var size = HandleUtility.GetHandleSize(end) * 0.05f;
                var moved = Handles.Slider2D(end, Vector3.up, Vector3.right, Vector3.forward, size, Handles.DotHandleCap, Vector2.zero);
                if (!capCheck.changed)
                    continue;

                Undo.RecordObject(controller, "Change torus cap");
                var degrees = Handles.SnapValue(CapAt(moved, side, cap) * Mathf.Rad2Deg, EditorSnapSettings.rotate);
                controller.Cap = Mathf.Clamp(degrees * Mathf.Deg2Rad, 0, Mathf.PI);
            }
        }

        /// <summary>The direction to the end of the arc on the <paramref name="side" /> of the z axis, 1 for +x and -1 for -x.</summary>
        static Vector3 ArcEnd(float cap, float side) => new(side * Mathf.Sin(cap), 0, Mathf.Cos(cap));

        /// <summary>
        ///     The cap that puts the end of the arc on the <paramref name="side" /> of the z axis, 1 for +x and -1 for -x, in
        ///     the direction of <paramref name="point" /> in the xz plane. Past the other side of the z axis, the cap stays at
        ///     0 or π, whichever is closer to <paramref name="previousCap" />.
        /// </summary>
        internal static float CapAt(Vector3 point, float side, float previousCap) {
            // compares the sign instead of the angle, because Atan2(-0, z) is -π behind the center
            if (side * point.x < 0)
                return previousCap > Mathf.PI / 2 ? Mathf.PI : 0;
            return Mathf.Atan2(Mathf.Abs(point.x), point.z);
        }

        [MenuItem("GameObject/SDF/Primitives/Torus")]
        static void Instantiate(MenuCommand command) => SdfPrimitiveControllerEditor.InstantiatePrimitive<SdfTorusController>("torus", command);
    }
}
