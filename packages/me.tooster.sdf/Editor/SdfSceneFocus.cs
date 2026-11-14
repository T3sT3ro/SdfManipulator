#nullable enable
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Editor {
    /// <summary>
    ///     Shows the selected SDF objects on their own as translucent ghosts in the scene view, also where other objects
    ///     hide them or an operation removes them, e.g. an object subtracted from another. <i>Tools &gt; SDF &gt; Show
    ///     Selected Objects</i> turns it on and off.
    /// </summary>
    [InitializeOnLoad]
    static class SdfSceneFocus {
        /// <summary>The name of the pass that draws one object on its own.</summary>
        public const string PassName = "SdfFocus";

        const string MenuPath      = "Tools/SDF/Show Selected Objects";
        const string PreferenceKey = "me.tooster.sdf.ShowSelectedObjects";

        static readonly int focusId = Shader.PropertyToID("_SdfFocusId");

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
                if (go.TryGetComponent<SdfController>(out var sdf)
                 && sdf.SdfScene is { sceneData: not null } scene
                 && scene.sceneData.controllers.TryGetValue(sdf, out var data))
                    Draw(scene, data.numericId, sceneView.camera);
        }

        /// <summary>
        ///     Draws the object with <paramref name="numericId" /> of <paramref name="scene" /> on its own, with the current
        ///     view and projection matrices. Returns false if no renderer of the scene has the focus pass.
        /// </summary>
        internal static bool Draw(SdfScene scene, int numericId, Camera? camera = null)
            => SdfSceneDrawing.ForEachRenderer(scene, PassName, camera, (renderer, mesh, source, pass) => {
                    var material = SdfSceneDrawing.MaterialWithUniforms(source, renderer, scene);
                    material.SetFloat(focusId, numericId);
                    material.SetPass(pass);
                    Graphics.DrawMeshNow(mesh, renderer.localToWorldMatrix);
                }
            );
    }
}
