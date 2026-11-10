#nullable enable
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace me.tooster.sdf.Editor {
    /// <summary>
    ///     In Prefab Mode, regenerates the shader of the edited SDF scene after changes that don't go through the
    ///     controllers' property setters: undo and redo, added, removed and reordered components, and created, destroyed
    ///     and re-parented objects. Outside Prefab Mode, the structure of a scene only changes when its prefab is imported.
    /// </summary>
    /// <remarks>
    ///     Changes to properties aren't handled here, because most of them only change uniforms: controllers report those,
    ///     and <see cref="Controller" />.OnValidate sets all uniforms after any change to serialized data. A structural
    ///     field changed without its property setter, e.g. a list of modifiers in the default inspector, regenerates on
    ///     save. Telling these changes apart needs the structural fingerprint of #84.
    /// </remarks>
    [InitializeOnLoad]
    static class SdfSceneChangeTracker {
        static SdfSceneChangeTracker() {
            ObjectChangeEvents.changesPublished += OnChangesPublished;
            Undo.undoRedoEvent += OnUndoRedo;
        }

        static void OnUndoRedo(in UndoRedoInfo info) {
            if (EditedScene() is { } scene)
                MarkForRegeneration(scene);
        }

        static void OnChangesPublished(ref ObjectChangeEventStream stream) {
            if (EditedScene() is not { } scene)
                return;

            var stageScene = scene.gameObject.scene;
            for (var i = 0; i < stream.length; i++) {
                if (IsStructuralChangeIn(ref stream, i, stageScene)) {
                    MarkForRegeneration(scene);
                    return;
                }
            }
        }

        static bool IsStructuralChangeIn(ref ObjectChangeEventStream stream, int index, Scene scene) {
            switch (stream.GetEventType(index)) {
                // a component was added, removed or reordered
                case ObjectChangeKind.ChangeGameObjectStructure:
                    stream.GetChangeGameObjectStructureEvent(index, out var structure);
                    return structure.scene == scene;
                case ObjectChangeKind.ChangeGameObjectStructureHierarchy:
                    stream.GetChangeGameObjectStructureHierarchyEvent(index, out var hierarchy);
                    return hierarchy.scene == scene;
                case ObjectChangeKind.ChangeGameObjectParent:
                    stream.GetChangeGameObjectParentEvent(index, out var parent);
                    return parent.newScene == scene || parent.previousScene == scene;
                case ObjectChangeKind.CreateGameObjectHierarchy:
                    stream.GetCreateGameObjectHierarchyEvent(index, out var created);
                    return created.scene == scene;
                case ObjectChangeKind.DestroyGameObjectHierarchy:
                    stream.GetDestroyGameObjectHierarchyEvent(index, out var destroyed);
                    return destroyed.scene == scene;
                default:
                    return false;
            }
        }

        /// The SDF scene at the root of the open prefab stage, if any.
        static SdfScene? EditedScene()
            => PrefabStageUtility.GetCurrentPrefabStage() is { prefabContentsRoot: { } root } && root.TryGetComponent<SdfScene>(out var scene)
                ? scene
                : null;

        // SdfScene regenerates in its next LateUpdate, which in edit mode only runs when something asks for it
        static void MarkForRegeneration(SdfScene scene) {
            scene.RequiresRegeneration = true;
            EditorApplication.QueuePlayerLoopUpdate();
        }
    }
}
