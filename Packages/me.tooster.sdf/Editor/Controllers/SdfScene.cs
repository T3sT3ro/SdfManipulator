#nullable enable
using System.Collections.Generic;
using System.IO;
using System.Linq;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.Controllers.ShaderPartials;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.VersionControl;
using UnityEngine;

namespace me.tooster.sdf.Editor.Controllers {
    /// <summary>
    /// SdfSceneController is a game object that handles displaying and controling SdfScene asset.
    /// TODO: split into asset (holding shader + scene description) and the controller "skeleton" (prefab?) + material (? default only?)
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [DisallowMultipleComponent]
    [SelectionBase]
    [ExecuteAlways] /* using ExecuteInEditMode has documented problems in prefab edit scene */
    public class SdfScene : MonoBehaviour, IPropertyIdentifierProvider {
        [SerializeField] private RaymarchingShader raymarchingShader;

        public Shader   controlledShader;
        public Material controlledMaterial;

        private HashSet<Controller> controllers = new();

        // TODO: property data cache for storing their id, owner and what not. Would be revised on hierarchy change
        private readonly Dictionary<Property, Controller> propertyDeclarations = new();

        public string GetIdentifier(Property property) {
            return Extensions.sanitizeNameToIdentifier(propertyDeclarations[property].name);
        }


        private void OnValidate() {
            if (!raymarchingShader) raymarchingShader = RaymarchingShader.instance;
            RegenerateAssetsSafely();
            controllers = GetComponentsInChildren<Controller>().ToHashSet();
            propertyDeclarations.Clear();
            foreach (var controller in controllers) {
                foreach (var property in controller.Properties)
                    propertyDeclarations[property] = controller;
            }
        }

        public void RegenerateAssetsSafely() {
            EditorApplication.delayCall -= RebuildShader;
            EditorApplication.delayCall += RebuildShader;
        }

        /// fixme: move to ScriptableObject
        private void RebuildShader() {
            // https://forum.unity.com/threads/onvalidate-alternative-to-allow-structural-changes.1521247/
            if (this == null) return; // yeah, wtf but this is needed, sometimes objects are destroyed...
            if (!controlledShader) return;

            var shaderText = AssembleShaderSource();
            Debug.LogFormat("Shader code:\n---\n{0}\n---", shaderText);

            ShaderUtil.ClearCachedData(controlledShader);
            File.WriteAllText(AssetDatabase.GetAssetPath(controlledShader), shaderText);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            controlledMaterial = new Material(controlledShader) { name = controlledShader.name };
        }

        // collect all "Properties" in all children components
        // TODO: make it return cached properties, update them when children are changed
        public ILookup<Controller, Property> Properties => GetComponentsInChildren<Controller>()
            .SelectMany(c => c.Properties.Select(p => new { controller = c, property = p }))
            .ToLookup(cp => cp.controller, cp => cp.property);

        // TODO: add shortcut accelerators to this and nodes (when sdf editing is enabled)
        [MenuItem("GameObject/SDF/Scene")]
        private static void CreateSdfScene() {
            var scene = GameObject.CreatePrimitive(PrimitiveType.Cube);
            scene.name = "SDF Scene";
            scene.AddComponent<SdfScene>();
        }

        private string AssembleShaderSource() =>
            "// GENERATED SHADER CONTENT. ANY MODIFICATIONS WILL BE OVERWRITTEN\n" +
            raymarchingShader.MainShader(this);

        /* Used to register a controller under the scene */
        public void AttachController(Controller controller) {
            controller.SdfScene = this;
            RegenerateAssetsSafely();
        }
    }

    [CustomEditor(typeof(SdfScene))]
    public class SDfSceneControllerEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            var sdfScene = (SdfScene)target;

            if (!sdfScene.controlledShader) {
                EditorGUILayout.HelpBox("missing controlled shader asset", MessageType.Error);
                base.OnInspectorGUI();
                return;
            }

            if (GUILayout.Button("Rebuild shader"))
                sdfScene.RegenerateAssetsSafely();
            GUILayout.Space(16);

            base.OnInspectorGUI();
        }
    }
}
