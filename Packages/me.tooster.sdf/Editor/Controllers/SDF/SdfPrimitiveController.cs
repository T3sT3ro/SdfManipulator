using me.tooster.sdf.Editor.Controllers.Data;
using UnityEditor;
using UnityEngine;
namespace me.tooster.sdf.Editor.Controllers.SDF {
    public abstract class SdfPrimitiveController : Controller, IModifier<VectorData, ScalarData> {
        public abstract ScalarData Apply(VectorData input, Processor processor);

        public static void InstantiatePrimitive<TController>(string name) where TController : SdfPrimitiveController, new() {
            var target = Selection.activeGameObject;

            var sdf = new GameObject(name);
            ObjectNames.SetNameSmart(sdf, name);
            if (target) sdf.transform.SetParent(Selection.activeTransform);
            sdf.transform.localPosition = Vector3.zero;

            var transformController = sdf.AddComponent<TransformController>();
            var primitiveSdfController = sdf.AddComponent<TController>();
            var sdfObjectController = sdf.AddComponent<SdfController>();

            sdfObjectController.sdfModifiers = new Controller[] { transformController, primitiveSdfController };
        }
    }
}
