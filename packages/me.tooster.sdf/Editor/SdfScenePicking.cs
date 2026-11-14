#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace me.tooster.sdf.Editor {
    /// <summary>
    ///     Lets the scene view pick the objects of SDF scenes. The picking pass of a generated shader identifies the
    ///     renderer, which stands for the whole scene. While the scene view picks, this class draws each scene again with
    ///     the object picking pass, which writes a picking index for each object ID, so a click on an SDF surface finds
    ///     the object that the ray hit.
    /// </summary>
    /// <remarks>
    ///     The scene view applies its usual rules to the result: <see cref="SdfScene" /> is a selection base, so the first
    ///     click selects the scene and the next click the object. Objects that the scene view skips, e.g. objects with
    ///     picking disabled or objects already passed while clicking through overlapping objects, aren't drawn, so the
    ///     scene view picks the scene itself there, and then what lies behind it.
    /// </remarks>
    [InitializeOnLoad]
    static class SdfScenePicking {
        /// <summary>The name of the pass that writes the picking index of each object.</summary>
        public const string PassName = "SdfObjectPicking";

        /// <summary>Object IDs from this one up can't be picked or outlined. Matches <c>SDF_MAX_OBJECT_IDS</c> in scenePicking.hlsl.</summary>
        public const int MaxObjectIds = 512;

        static readonly int pickingIndexId = Shader.PropertyToID("_SdfPickingIndex");
        static readonly int pickableId     = Shader.PropertyToID("_SdfPickable");

        // the objects that the last render drew, by picking index minus the first index of that render
        static readonly List<GameObject?> pickableObjects = new();
        static readonly float[] pickable = new float[MaxObjectIds];

        static SdfScenePicking() => HandleUtility.RegisterRenderPickingCallback(RenderForPicking);

        static RenderPickingResult RenderForPicking(in RenderPickingArgs args) {
            var objectSet = args.renderObjectSet;
            var onlyFromSet = args.renderPickingType == RenderPickingType.RenderFromFilterSet;
            var scenes = StageUtility.GetCurrentStageHandle().FindComponentsOfType<SdfScene>();

            var count = Render(scenes, args.pickingIndex, IsPickable, Camera.current);
            return count == 0 ? RenderPickingResult.NoOperation : new RenderPickingResult(count, Resolve);

            bool IsPickable(GameObject go) {
                var visibility = SceneVisibilityManager.instance;
                if (visibility.IsHidden(go) || visibility.IsPickingDisabled(go))
                    return false;
                var inSet = objectSet != null && objectSet.Contains(go);
                return onlyFromSet ? inSet : !inSet;
            }
        }

        /// <summary>
        ///     Draws the object picking pass of <paramref name="scenes" /> into the active render target, with the current
        ///     view and projection matrices.
        /// </summary>
        /// <param name="firstPickingIndex">The picking index of the first object.</param>
        /// <param name="isPickable">Decides which objects the pass draws.</param>
        /// <param name="camera">Renderers on layers that this camera doesn't render aren't drawn.</param>
        /// <returns>How many picking indices the drawn scenes use, from <paramref name="firstPickingIndex" /> on.</returns>
        internal static int Render(IEnumerable<SdfScene> scenes, int firstPickingIndex, Func<GameObject, bool> isPickable, Camera? camera = null) {
            pickableObjects.Clear();
            foreach (var scene in scenes) {
                // isActiveAndEnabled stays false until OnEnable runs, which it doesn't for prefab contents loaded by a script
                if (scene == null || !scene.enabled || !scene.gameObject.activeInHierarchy || scene.sceneData == null)
                    continue;

                var objects = ObjectsByNumericId(scene);
                for (var id = 0; id < MaxObjectIds; id++)
                    pickable[id] = id < objects.Length && objects[id] is { } go && isPickable(go) ? 1 : 0;

                var first = firstPickingIndex + pickableObjects.Count;
                var drawn = SdfSceneDrawing.ForEachRenderer(scene, PassName, camera, (renderer, mesh, source, pass) => {
                        // both picking passes have to march the same rays
                        var material = SdfSceneDrawing.MaterialWithUniforms(source, renderer, scene);
                        material.SetInteger(pickingIndexId, first);
                        material.SetFloatArray(pickableId, pickable);
                        material.SetPass(pass);
                        Graphics.DrawMeshNow(mesh, renderer.localToWorldMatrix);
                    }
                );

                if (drawn)
                    pickableObjects.AddRange(objects);
            }
            return pickableObjects.Count;
        }

        /// <summary>Returns the object of a picking index of the last <see cref="Render" />, counted from its first index.</summary>
        internal static Object? Resolve(int localPickingIndex)
            => localPickingIndex >= 0 && localPickingIndex < pickableObjects.Count ? pickableObjects[localPickingIndex] : null;

        // index 0 stays empty: object IDs start at 1
        static GameObject?[] ObjectsByNumericId(SdfScene scene) {
            var objects = new GameObject?[scene.sceneData.controllers.Count + 1];
            foreach (var (controller, data) in scene.sceneData.controllers)
                if (controller != null && data.numericId < objects.Length)
                    objects[data.numericId] = controller.gameObject;
            return objects;
        }
    }
}
