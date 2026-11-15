using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Editor.Inspectors.Primitives {
    /// <summary>
    ///     A scene handle for the radius of a rounded corner, like the corner widgets of vector drawing apps. A ring sits at
    ///     the center of the rounding, and dragging it toward the inside of the shape makes the rounding larger. Arcs show
    ///     the rounding between each pair of faces that meet at the corner.
    /// </summary>
    static class CornerRadiusHandle {
        /// <summary>
        ///     Draws the handle for the sharp corner at <paramref name="corner" />, in the space of
        ///     <see cref="Handles.matrix" />, and returns the radius that the user dragged it to. <paramref name="outward" />
        ///     are the unit normals of the faces that meet at the corner: three for the corner of a box, or two for the edge
        ///     of a cylinder in profile.
        /// </summary>
        public static float Draw(Vector3 corner, Vector3[] outward, float radius, float maxRadius) {
            maxRadius = Mathf.Max(maxRadius, 0);
            radius = Mathf.Clamp(radius, 0, maxRadius);
            var center = corner - radius * Diagonal(outward);

            for (var i = 0; i < outward.Length; i++)
                for (var j = i + 1; j < outward.Length; j++)
                    Handles.DrawWireArc(center, Vector3.Cross(outward[i], outward[j]), outward[i], 90, radius);

            var size = HandleUtility.GetHandleSize(center) * 0.06f;
            var moved = Handles.Slider(center, -Diagonal(outward).normalized, size, RingCap, 0);
            return Mathf.Clamp(Handles.SnapValue(RadiusAt(moved, corner, outward), EditorSnapSettings.move.x), 0, maxRadius);
        }

        /// <summary>
        ///     The radius that puts the center of the rounding of <paramref name="corner" /> closest to
        ///     <paramref name="point" />. It's negative when the point is outside the corner.
        /// </summary>
        internal static float RadiusAt(Vector3 point, Vector3 corner, Vector3[] outward) {
            var diagonal = Diagonal(outward);
            return Vector3.Dot(corner - point, diagonal) / diagonal.sqrMagnitude;
        }

        /// <summary>
        ///     The position of the camera in the space of <see cref="Handles.matrix" />, or for an orthographic camera a
        ///     direction toward it. Handles use it to sit on the side of a shape that faces the camera.
        /// </summary>
        public static Vector3 TowardCamera() {
            var camera = Camera.current;
            if (camera == null)
                return Vector3.one;
            return camera.orthographic
                ? Handles.inverseMatrix.MultiplyVector(-camera.transform.forward)
                : Handles.inverseMatrix.MultiplyPoint(camera.transform.position);
        }

        /// <summary>The right direction of the camera in the space of <see cref="Handles.matrix" />.</summary>
        public static Vector3 CameraRight() {
            var camera = Camera.current;
            return camera == null ? Vector3.right : Handles.inverseMatrix.MultiplyVector(camera.transform.right);
        }

        /// <summary>-1 for negative values and 1 for the others, so that a handle picks a side also at 0.</summary>
        public static float Side(float value) => value < 0 ? -1 : 1;

        static Vector3 Diagonal(Vector3[] outward) {
            var sum = Vector3.zero;
            foreach (var normal in outward)
                sum += normal;
            return sum;
        }

        /// <summary>A ring that faces the camera, which tells the handle apart from the dots of the bounds handles.</summary>
        static void RingCap(int controlId, Vector3 position, Quaternion rotation, float size, EventType eventType) {
            var camera = Camera.current;
            var facing = camera == null ? rotation : Quaternion.LookRotation(Handles.inverseMatrix.MultiplyVector(camera.transform.forward));
            Handles.CircleHandleCap(controlId, position, facing, size, eventType);
        }
    }
}
