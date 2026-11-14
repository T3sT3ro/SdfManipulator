#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Editor {
    /// <summary>
    ///     Outlines the selected objects of SDF scenes in the scene view. The scene view only outlines renderers, and the
    ///     objects of an SDF scene have none: the scene's renderer draws all of them. While objects of a scene are
    ///     selected, this class draws the outline of the scene's renderer again, with a global switch that makes the
    ///     scene selection pass keep only the selected objects. A selected combination outlines all of its objects.
    /// </summary>
    [InitializeOnLoad]
    static class SdfSceneOutline {
        static readonly int outlineSelectedId = Shader.PropertyToID("_SdfOutlineSelected");
        static readonly int outlinedId        = Shader.PropertyToID("_SdfOutlined");

        static SdfSceneOutline() => SceneView.duringSceneGui += OnSceneGUI;

        static void OnSceneGUI(SceneView sceneView) {
            if (Event.current.type != EventType.Repaint)
                return;

            var selectedByScene = Selection.gameObjects
                .Select(go => (go, scene: go.GetComponentInParent<SdfScene>()))
                // the scene view already outlines a selected scene, and objects without a controller draw nothing
                .Where(s => s.scene != null && s.go != s.scene.gameObject && s.go.TryGetComponent<Controller>(out _))
                .GroupBy(s => s.scene!, s => s.go);

            foreach (var selected in selectedByScene) {
                var scene = selected.Key;
                if (scene.sceneData == null || scene.Renderers.Count == 0)
                    continue;

                Shader.SetGlobalFloatArray(outlinedId, OutlineFlags(scene, selected));
                Shader.SetGlobalFloat(outlineSelectedId, 1);
                try {
                    Handles.DrawOutline(scene.Renderers.ToArray(), SelectedOutlineColor());
                } finally {
                    // the scene view outlines selected scenes with the same pass
                    Shader.SetGlobalFloat(outlineSelectedId, 0);
                }
            }
        }

        /// <summary>
        ///     Returns a flag for each object ID of <paramref name="scene" />: 1 for the controllers of the
        ///     <paramref name="selected" /> objects and of their children, 0 for the others.
        /// </summary>
        internal static float[] OutlineFlags(SdfScene scene, IEnumerable<GameObject> selected) {
            var flags = new float[SdfScenePicking.MaxObjectIds];
            foreach (var go in selected)
            foreach (var controller in go.GetComponentsInChildren<Controller>())
                if (scene.sceneData.controllers.TryGetValue(controller, out var data) && data.numericId < flags.Length)
                    flags[data.numericId] = 1;
            return flags;
        }

        /// <summary>The color of selection outlines from the preferences, which store it as <c>name;r;g;b;a</c>.</summary>
        static Color SelectedOutlineColor() {
            var parts = EditorPrefs.GetString("Scene/Selected Outline").Split(';');
            if (parts.Length >= 4
             && float.TryParse(parts[^4], NumberStyles.Float, CultureInfo.InvariantCulture, out var r)
             && float.TryParse(parts[^3], NumberStyles.Float, CultureInfo.InvariantCulture, out var g)
             && float.TryParse(parts[^2], NumberStyles.Float, CultureInfo.InvariantCulture, out var b)
             && float.TryParse(parts[^1], NumberStyles.Float, CultureInfo.InvariantCulture, out var a))
                return new Color(r, g, b, a);
            return new Color(1, 0.4f, 0, 1); // the default of the preference
        }
    }
}
