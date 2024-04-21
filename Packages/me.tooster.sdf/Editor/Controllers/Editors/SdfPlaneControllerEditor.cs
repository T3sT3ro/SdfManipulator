using me.tooster.sdf.Editor.Controllers.SDF;
using UnityEditor;
namespace me.tooster.sdf.Editor.Controllers.Editors {
    [CustomEditor(typeof(SdfPlaneController), true)]
    public class SdfPlaneControllerEditor : SdfControllerEditor {
        [MenuItem("GameObject/SDF/Primitives/Plane", priority = -20)]
        public static void Instantiate() => Controller.TryInstantiate<SdfPlaneController>("plane");
    }
}
