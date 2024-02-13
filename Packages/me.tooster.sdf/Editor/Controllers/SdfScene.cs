#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.Controllers.Data;
using me.tooster.sdf.Editor.Controllers.ShaderPartials;
using UnityEditor;
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
        [SerializeField] private bool              ensureReadOnly = true;
        public                   RaymarchingShader raymarchingShader;

        public Shader controlledShader;
        // owned material pattern from https://docs.unity3d.com/ScriptReference/HideFlags.html
        public Material controlledMaterial;

        private HashSet<Controller> controllers = new();

        // TODO: property data cache for storing their id, owner and what not. Would be revised on hierarchy change
        private readonly Dictionary<Property, Controller> propertyDeclarations = new();

        public string GetIdentifier(Property property) => API.Extensions.sanitizeNameToIdentifier(propertyDeclarations[property].name);

        private void OnEnable() {
            controlledMaterial = new Material(controlledShader)
            {
                name = controlledShader.name,
                hideFlags = HideFlags.HideAndDontSave,
            };
        }

        private void OnDisable() { DestroyImmediate(controlledMaterial); }

        private void OnValidate() {
            raymarchingShader = RaymarchingShader.instance;
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
            var shaderPath = AssetDatabase.GetAssetPath(controlledShader);
            var fileInfo = new FileInfo(shaderPath) { IsReadOnly = false };
            File.WriteAllText(shaderPath, shaderText);
            fileInfo.IsReadOnly = ensureReadOnly;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        // collect all "Properties" in all children components
        // TODO: make it return cached properties, update them when children are changed
        public ILookup<Controller, Property> Properties => controllers
            .SelectMany(c => c.Properties.Select(p => new { controller = c, property = p }))
            .ToLookup(cp => cp.controller, cp => cp.property);

        public IEnumerable<string> Includes => controllers
            .SelectMany(c => Extensions.CollectIncludes(c.GetType()).AsEnumerable());

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

        /// <summary>
        /// Returns SdfData representing the scene.
        /// </summary>
        public SdfData SceneSdf { get; }
    }
}
