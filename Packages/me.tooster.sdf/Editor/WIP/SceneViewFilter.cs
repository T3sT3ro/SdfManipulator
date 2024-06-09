using UnityEditor;
using UnityEngine;
namespace me.tooster.sdf.Editor {
    /// <summary>
    /// <a href="Source https://www.youtube.com/watch?v=iZ6ARyKnD-k&t=354s">Source</a>
    /// </summary>
    public class SceneViewFilter : MonoBehaviour {
#if UNITY_EDITOR
        bool hasChanged = false;

        public virtual void OnValidate() { hasChanged = true; }

        static SceneViewFilter() => SceneView.duringSceneGui += CheckMe;

        static void CheckMe(SceneView sv) {
            if (Event.current.type != EventType.Layout)
                return;
            if (!Camera.main)
                return;
            // Get a list of everything on the main camera that should be synced.
            SceneViewFilter[] cameraFilters = Camera.main.GetComponents<SceneViewFilter>();
            SceneViewFilter[] sceneFilters = sv.camera.GetComponents<SceneViewFilter>();

            // Let's see if the lists are different lengths or something like that. 
            // If so, we simply destroy all scene filters and recreate from maincame
            if (cameraFilters.Length != sceneFilters.Length) {
                Recreate(sv);
                return;
            }
            for (var i = 0; i < cameraFilters.Length; i++) {
                if (cameraFilters[i].GetType() != sceneFilters[i].GetType()) {
                    Recreate(sv);
                    return;
                }
            }

            // Ok, WHICH filters, or their order hasn't changed.
            // Let's copy all settings for any filter that has changed.
            for (int i = 0; i < cameraFilters.Length; i++)
                if (cameraFilters[i].hasChanged || sceneFilters[i].enabled != cameraFilters[i].enabled) {
                    EditorUtility.CopySerialized(cameraFilters[i], sceneFilters[i]);
                    cameraFilters[i].hasChanged = false;
                }
        }

        static void Recreate(SceneView sv) {
            SceneViewFilter filter;
            while (filter = sv.camera.GetComponent<SceneViewFilter>())
                DestroyImmediate(filter);

            foreach (var f in Camera.main.GetComponents<SceneViewFilter>()) {
                SceneViewFilter newFilter = sv.camera.gameObject.AddComponent(f.GetType()) as SceneViewFilter;
                EditorUtility.CopySerialized(f, newFilter);
            }
        }
#endif
    }
}
