using UnityEditor;
using UnityEditor.Overlays;
namespace me.tooster.sdf.Editor {
    [Overlay(typeof(SceneView), "Editor Camera Tools")]
    public class EditorCameraToolbarOverlay : ToolbarOverlay { }
}
