#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.Controllers.Data;
using me.tooster.sdf.Editor.Controllers.ShaderPartials;
using me.tooster.sdf.Editor.Util;
using UnityEditor;
using UnityEngine;
namespace me.tooster.sdf.Editor.Controllers.SDF {
    /// <summary>
    /// SdfSceneController is a game object that handles displaying and controling SdfScene asset.
    /// TODO: split into asset (holding shader + scene description) and the controller "skeleton" (prefab?) + material (? default only?)
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [DisallowMultipleComponent]
    [SelectionBase]
    [ExecuteAlways] /* using ExecuteInEditMode has documented problems in prefab edit scene */
    [Icon("Packages/me.tooster.sdf/Editor/Resources/Icons/sdf-scene-914.png")]
    public class SdfScene : MonoBehaviour {
        [SerializeField]
        private bool ensureReadOnly = true;
        public RaymarchingShader raymarchingShader;

        public Shader controlledShader;
        // owned material pattern from https://docs.unity3d.com/ScriptReference/HideFlags.html
        public Material controlledMaterial;

        private record ControllerData(int sceneUniqueId, string uniqueIdentifier);
        /// controllers mapping to their ID in sdfScene
        private readonly Dictionary<Controller, ControllerData> controllerData = new();
        private record PropertyData(Controller owner, string uniqueIdentifier, int shaderPropertyId);

        // TODO: property data cache for storing their id, owner and what not. Would be revised on hierarchy change
        private readonly IReadOnlyDictionary<Property, PropertyData> propertyData = new Dictionary<Property, PropertyData>();


        public string GetPropertyIdentifier(Property property)       => propertyData[property].uniqueIdentifier;
        public string GetControllerIdentifier(Controller controller) => controllerData[controller].uniqueIdentifier;
        private void OnEnable() {
            var isPrefab = PrefabUtility.IsOutermostPrefabInstanceRoot(gameObject);
            var prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
            if (isPrefab) {
                controlledShader = AssetDatabase.LoadAssetAtPath<Shader>(prefabPath);
                controlledMaterial = AssetDatabase.LoadAssetAtPath<Material>(prefabPath);

                if (controlledShader == null) {
                    controlledShader = ShaderUtil.CreateShaderAsset(AssembleShaderSource());
                    AssetDatabase.AddObjectToAsset(controlledShader, prefabPath);
                }

                if (controlledMaterial == null) {
                    controlledMaterial = new Material(controlledShader) { name = controlledShader.name };
                    AssetDatabase.AddObjectToAsset(controlledMaterial, prefabPath);
                }
                AssetDatabase.SaveAssets();
            }
        }

        // TODO: use more event-driven architecture where creation, move, rename and deletion of individual controllers is detected. 
        private void OnValidate() {
            if (!PrefabUtility.IsOutermostPrefabInstanceRoot(gameObject)
             && PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject) is { } prefabPath) {
                var allSubAssets = AssetDatabase.LoadAllAssetsAtPath(prefabPath);
                var uniqueShaders = new HashSet<string>();
                foreach (var asset in allSubAssets.OfType<Shader>()) {
                    if (asset is not Shader) continue;
                    if (!uniqueShaders.Add(asset.name)) {
                        AssetDatabase.RemoveObjectFromAsset(asset);
                    }
                }
            }

            raymarchingShader = RaymarchingShader.instance;
            ((Dictionary<Property, PropertyData>)propertyData).Clear();
            controllerData.Clear();
            foreach (var controller in GetComponentsInChildren<Controller>())
                RegisterController(controller);

            if (PrefabUtility.IsOutermostPrefabInstanceRoot(gameObject)) {
                RegisterForRegeneration();
            }
        }

        /// Schedules editor for regenerating the shader
        public void RegisterForRegeneration() {
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
            // var shaderPath = AssetDatabase.GetAssetPath(controlledShader);
            ShaderUtil.UpdateShaderAsset(controlledShader, shaderText);
            // var fileInfo = new FileInfo(shaderPath) { IsReadOnly = false };
            // File.WriteAllText(shaderPath, shaderText);
            // fileInfo.IsReadOnly = ensureReadOnly;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        // collect all "Properties" in all children components
        // TODO: make it return cached properties, update them when children are changed
        public ILookup<Controller, Property> Properties => controllerData.Keys
            .SelectMany(c => c.Properties.Select(p => new { controller = c, property = p }))
            .ToLookup(cp => cp.controller, cp => cp.property);

        public IEnumerable<string> Includes => controllerData.Keys
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
        public void RegisterController(Controller controller) {
            var uniqueIdentifier = GetControllerPathIdentifier(controller);
            controllerData[controller] = new ControllerData(controllerData.Count, uniqueIdentifier);
            foreach (var property in controller.Properties) {
                var pId = $"{uniqueIdentifier}_{property.InternalName.sanitizeToIdentifierString()}";
                ((Dictionary<Property, PropertyData>)propertyData)[property] = new PropertyData(controller, pId, Shader.PropertyToID(pId));
                RegisterPropertyUpdateHandler(property);
            }
        }

        public void RegisterPropertyUpdateHandler(Property property) {
            var id = propertyData[property].shaderPropertyId;
            // @formatter off
            switch (property) {
                case Property<int> p1:          p1.onValueChanged += HandlePropertyUpdate; break;
                case Property<float> p2:        p2.onValueChanged += HandlePropertyUpdate; break;
                case Property<bool> p3:         p3.onValueChanged += HandlePropertyUpdate; break;
                case Property<Vector2> p4:      p4.onValueChanged += HandlePropertyUpdate; break;
                case Property<Vector3> p5:      p5.onValueChanged += HandlePropertyUpdate; break;
                case Property<Vector4> p6:      p6.onValueChanged += HandlePropertyUpdate; break;
                case Property<Vector2Int> p7:   p7.onValueChanged += HandlePropertyUpdate; break;
                case Property<Vector3Int> p8:   p8.onValueChanged += HandlePropertyUpdate; break;
                case Property<Matrix4x4> p9:    p9.onValueChanged += HandlePropertyUpdate; break;
                default: throw new ArgumentOutOfRangeException(nameof(property), property, "Unhandled property update");
            }
            // @formatter on
        }

        public void HandlePropertyUpdate(Property<int> property, int value) =>
            controlledMaterial?.SetInteger(propertyData[property].shaderPropertyId, value);
        public void HandlePropertyUpdate(Property<float> property, float value) =>
            controlledMaterial?.SetFloat(propertyData[property].shaderPropertyId, value);
        public void HandlePropertyUpdate(Property<bool> property, bool value) =>
            controlledMaterial?.SetFloat(propertyData[property].shaderPropertyId, value ? 1 : 0);
        public void HandlePropertyUpdate(Property<Vector2> property, Vector2 value) =>
            controlledMaterial?.SetVector(propertyData[property].shaderPropertyId, value);
        public void HandlePropertyUpdate(Property<Vector3> property, Vector3 value) =>
            controlledMaterial?.SetVector(propertyData[property].shaderPropertyId, value);
        public void HandlePropertyUpdate(Property<Vector4> property, Vector4 value) =>
            controlledMaterial?.SetVector(propertyData[property].shaderPropertyId, value);
        public void HandlePropertyUpdate(Property<Vector2Int> property, Vector2Int value) =>
            controlledMaterial?.SetVector(propertyData[property].shaderPropertyId, (Vector2)value);
        public void HandlePropertyUpdate(Property<Vector3Int> property, Vector3Int value) =>
            controlledMaterial?.SetVector(propertyData[property].shaderPropertyId, (Vector3)value);
        public void HandlePropertyUpdate(Property<Matrix4x4> property, Matrix4x4 value) =>
            controlledMaterial?.SetMatrix(propertyData[property].shaderPropertyId, value);


        /// <summary>
        /// Returns SdfData representing the scene.
        /// </summary>
        public SdfData SceneSdfData() {
            var sdfDatas = controllerData.Keys.OfType<SdfPrimitiveController>().Select(c => c.sdfData).ToArray();
            if (sdfDatas.Length == 0) {
                return SdfData.Empty;
            }
            var combined = Combinators.binaryCombine(Combinators.CombineWithSimpleUnion, sdfDatas);
            return combined;
        }

        /// <summary>
        /// Returns a sequence of transforms starting (and excluding) scene name down to the controller.
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public IEnumerable<Transform> GetControllerPath(Controller controller) =>
            controller.transform.AncestorsAndSelf().TakeWhile(t => t != transform).Reverse();

        private string GetControllerPathIdentifier(Controller controller) =>
            GetControllerPath(controller)
                .Select(t => $"{t.name}_{t.GetSiblingIndex()}")
                .Prepend("sdfScene")
                .Select(API.Extensions.sanitizeToIdentifierString)
                .JoinToString("_");

        public int GetControllerId(SdfPrimitiveController sdfPrimitiveController) =>
            controllerData[sdfPrimitiveController].sceneUniqueId;
    }
}
