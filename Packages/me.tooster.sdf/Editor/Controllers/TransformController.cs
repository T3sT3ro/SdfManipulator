#nullable enable
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.NodeGraph.Nodes;
using UnityEngine;

namespace me.tooster.sdf.Editor.Controllers {
    [ExecuteInEditMode]
    [RequireComponent(typeof(Transform))]
    public class TransformController : Controller {
        protected Property<Matrix4x4> transformProperty  = null!;
        protected MatrixPropertyNode  primitiveTransform = null!;

        protected override void Start() {
            transformProperty = new Property<Matrix4x4>("transform", "transform", Matrix4x4.identity);
            primitiveTransform = new MatrixPropertyNode(transformProperty);
        }

        protected override void UpdateUniforms() {
            if (sdfDomain == null || sdfDomain.Material == null)
                return;

            var spaceTransform = Matrix4x4.Rotate(Quaternion.Inverse(transform.localRotation));
            spaceTransform *=
                Matrix4x4.Translate(Vector3.Scale(-transform.localPosition, sdfDomain.transform.localScale));

            sdfDomain.Material.SetMatrix(((Representable)transformProperty).IdName,
                spaceTransform);
        }
    }
}
