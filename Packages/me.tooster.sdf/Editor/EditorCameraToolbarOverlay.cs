using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
namespace me.tooster.sdf.Editor {
    [Overlay(typeof(SceneView), "Editor Camera Tools")]
    public class EditorCameraToolbarOverlay : ToolbarOverlay {
        public EditorCameraToolbarOverlay() : base() { }
    }

    [EditorToolbarElement(id, typeof(SceneView))]
    internal class ShowFullsccreenScene : EditorToolbarButton, IAccessContainerWindow {
        public const string id = "SdfToolbar/ShowFullscreenScene";

        public EditorWindow containerWindow { get; set; }
    }
}
