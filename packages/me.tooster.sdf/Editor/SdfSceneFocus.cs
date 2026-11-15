#nullable enable
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Editor {
    /// <summary>
    ///     Shows the selected SDF objects on their own as translucent ghosts in the scene view, also where other objects
    ///     hide them or an operation removes them, e.g. an object subtracted from another. An inverted object shows as
    ///     its shape in red, which is the volume that it removes. <i>Tools &gt; SDF &gt; Show Selected Objects</i> turns
    ///     it on and off.
    /// </summary>
    [InitializeOnLoad]
    static class SdfSceneFocus {
        /// <summary>The name of the pass that draws one object on its own.</summary>
        public const string PassName = "SdfFocus";

        const string MenuPath      = "Tools/SDF/Show Selected Objects";
        const string PreferenceKey = "me.tooster.sdf.ShowSelectedObjects";

        static readonly int focusId       = Shader.PropertyToID("_SdfFocusId");
        static readonly int focusInverted = Shader.PropertyToID("_SdfFocusInverted");

        static bool Enabled {
            get => EditorPrefs.GetBool(PreferenceKey, true);
            set => EditorPrefs.SetBool(PreferenceKey, value);
        }

        static SdfSceneFocus() => SceneView.duringSceneGui += OnSceneGUI;

        [MenuItem(MenuPath)]
        static void Toggle() {
            Enabled = !Enabled;
            SceneView.RepaintAll();
        }

        [MenuItem(MenuPath, true)]
        static bool UpdateToggle() {
            Menu.SetChecked(MenuPath, Enabled);
            return true;
        }

        static void OnSceneGUI(SceneView sceneView) {
            if (!Enabled || Event.current.type != EventType.Repaint)
                return;
            foreach (var go in Selection.gameObjects)
                if (go.TryGetComponent<SdfController>(out var sdf) && sdf.SdfScene is { } scene)
                    Draw(scene, sdf, sceneView.camera);
        }

        /// <summary>
        ///     Draws <paramref name="sdf" /> of <paramref name="scene" /> on its own, with the current view and projection
        ///     matrices. Returns false if the scene has no generated data for the object, or if no renderer of the scene has
        ///     the focus pass.
        /// </summary>
        internal static bool Draw(SdfScene scene, SdfController sdf, Camera? camera = null) {
            if (scene.sceneData is null || !scene.sceneData.controllers.TryGetValue(sdf, out var data))
                return false;
            return SdfSceneDrawing.ForEachRenderer(scene, PassName, camera, (renderer, mesh, source, pass) => {
                    var material = SdfSceneDrawing.MaterialWithUniforms(source, renderer, scene);
                    material.SetFloat(focusId, data.numericId);
                    material.SetFloat(focusInverted, sdf.Inverted ? 1 : 0);
                    material.SetPass(pass);
                    Graphics.DrawMeshNow(mesh, renderer.localToWorldMatrix);
                }
            );
        }
    }
}
