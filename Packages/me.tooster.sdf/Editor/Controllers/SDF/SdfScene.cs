#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.Controllers.Data;
using me.tooster.sdf.Editor.Controllers.SDF.Operators;
using me.tooster.sdf.Editor.Controllers.ShaderPartials;
using me.tooster.sdf.Editor.Util;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using Object = System.Object;
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
    public class SdfScene : MonoBehaviour { // TODO: inherit from Component to disallow enabling/disabling
        [SerializeField]
        private bool ensureReadOnly = true;
        public RaymarchingShader raymarchingShader;

        public Shader controlledShader;
        // owned material pattern from https://docs.unity3d.com/ScriptReference/HideFlags.html
        public Material controlledMaterial;

        private record ControllerData(int sceneUniqueId, string uniqueIdentifier);
        private record PropertyData(Controller declaringController, string uniqueIdentifier, int shaderPropertyId);

        /// controllers mapping to their ID in sdfScene
        private readonly Dictionary<Controller, ControllerData> controllerData = new();
        private readonly Dictionary<Property, PropertyData> propertyData         = new();
        private          HashSet<Property>                  queuedPropertyUdates = new();

        public string GetPropertyIdentifier(Property property)       => propertyData[property].uniqueIdentifier;
        public string GetControllerIdentifier(Controller controller) => controllerData[controller].uniqueIdentifier;

        public bool IsDirty { get; set; }

        // TODO: use more event-driven architecture where creation, move, rename and deletion of individual controllers is detected. 
        private void OnValidate() {
            raymarchingShader = RaymarchingShader.instance;
            if (!PrefabUtility.IsOutermostPrefabInstanceRoot(gameObject)
             && PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject) is { } prefabPath) {
                // var allSubAssets = AssetDatabase.LoadAllAssetsAtPath(prefabPath);
                // var uniqueShaders = new HashSet<string>();
                // // ensures there is just one associated generated shader, 
                // foreach (var asset in allSubAssets.OfType<Shader>()) {
                //     if (asset is not Shader) continue;
                //     if (!uniqueShaders.Add(asset.name)) {
                //         AssetDatabase.RemoveObjectFromAsset(asset);
                //     }
                // }

                var previousShader = controlledShader;
                var previousMaterial = controlledMaterial;

                // nothing is attached
                if (controlledShader == null) controlledShader = AssetDatabase.LoadAssetAtPath<Shader>(prefabPath);
                if (controlledMaterial == null) controlledMaterial = AssetDatabase.LoadAssetAtPath<Material>(prefabPath);

                // asset doesn't exist
                if (controlledShader == null) {
                    controlledShader = ShaderUtil.CreateShaderAsset(AssembleShaderSource());
                    AssetDatabase.AddObjectToAsset(controlledShader, prefabPath);
                }

                if (controlledMaterial == null) {
                    controlledMaterial = new Material(controlledShader) { name = controlledShader.name };
                    AssetDatabase.AddObjectToAsset(controlledMaterial, prefabPath);
                }

                if (previousShader != controlledShader || previousMaterial != controlledMaterial)
                    EditorUtility.SetDirty(this);
            }

            if (PrefabUtility.IsOutermostPrefabInstanceRoot(gameObject))
                IsDirty = true;

            RefreshSceneData();
            QueuePropertyForUpdate(propertyData.Keys);
        }

        private void UpdateProperty(Property property) { // TODO: store properties in typed containers t oavoid runtime checks
            if (!controlledMaterial || !propertyData.TryGetValue(property, out var pd))
                return;
            var shaderPropertyId = pd.uniqueIdentifier; //pd.shaderPropertyId;
            switch (property) {
                case Property<int> p:
                    controlledMaterial.SetInteger(shaderPropertyId, p.Value);
                    break;
                case Property<float> p:
                    controlledMaterial.SetFloat(shaderPropertyId, p.Value);
                    break;
                case Property<bool> p:
                    controlledMaterial.SetFloat(shaderPropertyId, p.Value ? 1 : 0);
                    break;
                case Property<Vector2> p:
                    controlledMaterial.SetVector(shaderPropertyId, p.Value);
                    break;
                case Property<Vector3> p:
                    controlledMaterial.SetVector(shaderPropertyId, p.Value);
                    break;
                case Property<Vector4> p:
                    controlledMaterial.SetVector(shaderPropertyId, p.Value);
                    break;
                case Property<Vector2Int> p:
                    controlledMaterial.SetVector(shaderPropertyId, (Vector2)p.Value);
                    break;
                case Property<Vector3Int> p:
                    controlledMaterial.SetVector(shaderPropertyId, (Vector3)p.Value);
                    break;
                case Property<Matrix4x4> p:
                    controlledMaterial.SetMatrix(shaderPropertyId, p.Value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(property), property, "Unhandled property update");
            }
        }

        private void HandleStructuralChange(Controller source) => RefreshSceneData();

        internal void RefreshSceneData() {
            if (this == null) return;
            raymarchingShader = RaymarchingShader.instance;
            propertyData.Clear();
            controllerData.Clear();

            foreach (var controller in GetComponentsInChildren<Controller>())
                Register(controller);
        }

        private void LateUpdate() {
            if (IsDirty) {
                IsDirty = false;
                RefreshSceneData();
                RebuildShader();
            }

            UpdateShaderUniforms();
        }

        internal void UpdateShaderUniforms() {
            if (queuedPropertyUdates.Count <= 0)
                return;

            // var updateTable = new StringBuilder("Updates:");
            foreach (var p in queuedPropertyUdates) {
                UpdateProperty(p);
                if (propertyData.TryGetValue(p, out var pData))
                    EditorGUIUtility.PingObject(pData.declaringController);
                else
                    Debug.LogWarning($"Tried updating non-existent property: {p}");
                // updateTable.Append(
                //     $"\nController: '{controllerData[pData.declaringController].uniqueIdentifier}'\tProperty: {pData.uniqueIdentifier}\n{p.CurrentValue}"
                // );
            }

            // Debug.Log(updateTable);
            queuedPropertyUdates.Clear();
        }

        public void QueuePropertyForUpdate(Property p)                       => queuedPropertyUdates.Add(p);
        public void QueuePropertyForUpdate(IEnumerable<Property> properties) => queuedPropertyUdates.UnionWith(properties);


        /// fixme: move to ScriptableObject
        internal void RebuildShader() {
            // https://forum.unity.com/threads/onvalidate-alternative-to-allow-structural-changes.1521247/
            if (this == null) return; // yeah, wtf but this is needed, sometimes objects are destroyed...
            if (!controlledShader) return;
            if (PrefabStageUtility.GetCurrentPrefabStage() is { } stage && stage.prefabContentsRoot != gameObject)
                return;
            var shaderText = AssembleShaderSource();
            Debug.LogFormat("Shader code:\n---\n{0}\n---", shaderText);

            ShaderUtil.ClearCachedData(controlledShader);
            var shaderPath = AssetDatabase.GetAssetPath(controlledShader);
            if (PrefabUtility.IsPartOfPrefabAsset(controlledShader)) {
                ShaderUtil.UpdateShaderAsset(controlledShader, shaderText);
            } else {
                var fileInfo = new FileInfo(shaderPath) { IsReadOnly = false };
                File.WriteAllText(shaderPath, shaderText);
                fileInfo.IsReadOnly = ensureReadOnly;
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            controlledMaterial = new Material(controlledShader);

            controlledMaterial.shader = controlledShader;
            QueuePropertyForUpdate(propertyData.Keys);
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

        internal string AssembleShaderSource() =>
            "// GENERATED SHADER CONTENT. ANY MODIFICATIONS WILL BE OVERWRITTEN\n" +
            raymarchingShader.MainShader(this);

        /* Used to register a controller under the scene */
        public void Register(Controller controller) {
            if (!controllerData.ContainsKey(controller)) {
                controller.onStructureChanged -= HandleStructuralChange;
                controller.onStructureChanged += HandleStructuralChange;
            }

            var uniqueIdentifier = GetControllerPathIdentifier(controller);
            controllerData[controller] = new ControllerData(controllerData.Count, uniqueIdentifier);
            foreach (var property in controller.Properties) {
                var pId = $"{uniqueIdentifier}_{property.InternalName.sanitizeToIdentifierString()}";
                propertyData[property] = new PropertyData(controller, pId, Shader.PropertyToID(pId));
                QueuePropertyForUpdate(property);
            }
        }

        /// <summary>
        /// Returns SdfData representing the scene. TODO: separate into required component of SdfCobineController with simple union type
        /// </summary>
        public SdfData SceneSdfData() {
            var sdfDatas = gameObject.GetImmediateChildrenComponents<SdfController>().Select(c => c.sdfData).ToArray();
            if (sdfDatas.Length == 0)
                return SdfData.Empty;
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

        private string GetControllerPathIdentifier(Controller controller) {
            var pathFromParent = GetControllerPath(controller).ToArray();
            var indexedPath = pathFromParent.Select(t => t.GetSiblingIndex().ToString("X")).JoinToString("_");
            return pathFromParent.Select(t => $"{t.name}")
                .Prepend($"_{indexedPath}")
                .Select(API.Extensions.sanitizeToIdentifierString)
                .JoinToString("_");
        }

        public int GetControllerId(SdfController sdfController) =>
            controllerData[sdfController].sceneUniqueId;
    }


    [CustomEditor(typeof(SdfScene))]
    public class SdfSceneEditor : UnityEditor.Editor {
        private bool propertiesFold = false;
        private Dictionary<Controller, bool> individualFolds = new();
        private void OnValidate() { individualFolds = (target as SdfScene).Properties.ToDictionary(g => g.Key, g => false); }

        public override void OnInspectorGUI() {
            var sdfScene = (SdfScene)target;

            if (!sdfScene.controlledShader) {
                EditorGUILayout.HelpBox("missing controlled shader asset", MessageType.Error);
                base.OnInspectorGUI();
                return;
            }

            if (!sdfScene.raymarchingShader) {
                EditorGUILayout.HelpBox("Raymarching shader asset required", MessageType.Error);
                base.OnInspectorGUI();
                return;
            }

            if (GUILayout.Button("Rebuild shader")) {
                sdfScene.RefreshSceneData();
                sdfScene.RebuildShader();
                sdfScene.UpdateShaderUniforms();
            }
            if (GUILayout.Button("Open generated shader")) // TODO: open temporary file with this text
                AssetDatabase.OpenAsset(sdfScene.controlledShader);
            if (GUILayout.Button("Assign material to renderer")) {
                var renderer = sdfScene.GetComponent<Renderer>();
                if (renderer)
                    renderer.material = sdfScene.controlledMaterial;
                else
                    EditorGUILayout.HelpBox("No renderer found", MessageType.Error);
            }
            GUILayout.Space(16);
            base.OnInspectorGUI();
            if (propertiesFold = EditorGUILayout.BeginFoldoutHeaderGroup(propertiesFold, "Managed properties"))
                foreach (var props in sdfScene.Properties) {
                    var ctr = props.Key;
                    if (!(individualFolds[ctr] = EditorGUILayout.Foldout(individualFolds[ctr], sdfScene.GetControllerIdentifier(ctr))))
                        continue;

                    foreach (var prop in ctr.Properties) {
                        EditorGUILayout.LabelField(prop.DisplayName, EditorStyles.boldLabel);
                        if (GUILayout.Button("trigger update for this property"))
                            ctr.SdfScene.QueuePropertyForUpdate(prop);
                        EditorGUILayout.TextArea(prop.CurrentValue.ToString());
                    }
                }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void OpenGeneratedShader() {
            var sdfScene = (SdfScene)target;

            var shaderText = sdfScene.AssembleShaderSource();
            var assetName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(sdfScene.controlledShader));
            var path = $"Temp/GeneratedSdfScene-{assetName.Replace(" ", "")}.shader";
            WriteToFile(path, shaderText);
            OpenFile(path);
        }

        // From ShaderGraph GraphUtil.cs
        public static bool WriteToFile(string path, string content) {
            try {
                File.WriteAllText(path, content);
                return true;
            } catch (Exception e) {
                Debug.LogError(e);
                return false;
            }
        }

        // From ShaderGraph GraphUtil.cs
        public static void OpenFile(string path) {
            var filePath = Path.GetFullPath(path);
            if (!File.Exists(filePath)) {
                Debug.LogError(string.Format("Path {0} doesn't exists", path));
                return;
            }

            var externalScriptEditor = ScriptEditorUtility.GetExternalScriptEditor();
            if (externalScriptEditor != "internal") {
                InternalEditorUtility.OpenFileAtLineExternal(filePath, 0);
            } else {
                var p = new Process();
                p.StartInfo.FileName = filePath;
                p.EnableRaisingEvents = true;
                p.Exited += (Object obj, EventArgs args) => {
                    if (p.ExitCode != 0)
                        Debug.LogWarningFormat("Unable to open {0}: Check external editor in preferences", filePath);
                };
                p.Start();
            }
        }

        private void OnSceneGUI() {
            var scene = (SdfScene)target;
            scene.UpdateShaderUniforms();
        }
    }
}
