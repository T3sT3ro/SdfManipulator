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

        private void Awake() {
            PrefabStage.prefabSaved += OnPrefabSaved;
            // To present things 
            GenerateSceneAssets();
            RefreshSceneData();
            UpdateShaderUniforms();
        }

        private static void OnPrefabSaved(GameObject go) {
            var scene = go.GetComponent<SdfScene>();
            scene.RefreshSceneData();
            scene.UpdateShaderUniforms(true);
        }

        // TODO: use more event-driven architecture where creation, move, rename and deletion of individual controllers is detected. 
        private void OnValidate() {
            raymarchingShader = RaymarchingShader.instance;
            RefreshSceneData();
            UpdateShaderUniforms();
        }

        private void UpdateProperty(Property property) { // TODO: store properties in typed containers t oavoid runtime checks
            if (!controlledMaterial)
                Debug.LogError("SdfScene is missing controlled material to update!");

            if (!propertyData.TryGetValue(property, out var pd)) {
                Debug.LogError($"Tried updating untracked property '{property}'");
                return;
            }

#if UNITY_EDITOR
            EditorGUIUtility.PingObject(pd.declaringController); // just for debug purposes
#endif

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

        private void HandleStructuralChange(Controller source) {
            RefreshSceneData();
            GenerateSceneAssets();
            UpdateShaderUniforms();
        }

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
                GenerateSceneAssets();
            }

            UpdateShaderUniforms();
        }

        internal void UpdateShaderUniforms(bool forceUpdateAll = false) {
            if (forceUpdateAll) QueuePropertyUpdates(propertyData.Keys);
            if (queuedPropertyUdates.Count <= 0)
                return;

            // var updateTable = new StringBuilder("Updates:");
            foreach (var p in queuedPropertyUdates)
                UpdateProperty(p);

            // Debug.Log(updateTable);
            queuedPropertyUdates.Clear();
        }

        public void QueuePropertyUpdates(Property p)                       => queuedPropertyUdates.Add(p);
        public void QueuePropertyUpdates(IEnumerable<Property> properties) => queuedPropertyUdates.UnionWith(properties);

        /// fixme: move to ScriptableObject
        /*internal void RebuildShader() {
            // https://forum.unity.com/threads/onvalidate-alternative-to-allow-structural-changes.1521247/
            if (this == null) return; // yeah, wtf but this is needed, sometimes objects are destroyed...
            if (!controlledShader) return;
            if (PrefabStageUtility.GetCurrentPrefabStage() is { } stage && stage.prefabContentsRoot != gameObject)
                return;
            var shaderText = AssembleShaderSource();
            Debug.LogFormat("Shader code:\n---\n{0}\n---", shaderText);

            ShaderUtil.ClearCachedData(controlledShader);
            var shaderPath = AssetDatabase.GetAssetPath(controlledShader);
            if (PrefabUtility.IsPartOfPrefabAsset(controlledShader))
                ShaderUtil.UpdateShaderAsset(controlledShader, shaderText);
            else {
                var fileInfo = new FileInfo(shaderPath) { IsReadOnly = false };
                File.WriteAllText(shaderPath, shaderText);
                fileInfo.IsReadOnly = ensureReadOnly;
            }
            AssetDatabase.SaveAssets();
            controlledMaterial = new Material(controlledShader);
            controlledMaterial.shader = controlledShader;
            QueuePropertyForUpdate(propertyData.Keys);
        }*/

#if UNITY_EDITOR
        /// <summary>
        /// Generates the Shader and Material as sub-assets of the prefab, if inside the prefab stage.
        /// Outside of prefab stage, this function doesn't trigger and print a warning instead.
        /// The intent behind that is that shaders are compiled, so there is 1 to 1 mapping between prefab scene and generated shader.
        /// Any instance with changed structure would require another shader asset, or otherwise would conflict with the prefab's shader.
        /// </summary>
        /// <exception cref="SdfException"></exception>
        internal void GenerateSceneAssets() {
            var prefabStage = PrefabStageUtility.GetPrefabStage(gameObject);
            if (prefabStage == null) {
                Debug.LogWarning("Assets can be only edited in the prefab stage");
                return;
            }

            var sdfScenePrefabAssetPath = prefabStage.assetPath;
            var shader = AssetDatabase.LoadAssetAtPath<Shader>(sdfScenePrefabAssetPath);
            var material = AssetDatabase.LoadAssetAtPath<Material>(sdfScenePrefabAssetPath);

            try {
                var shaderSource = AssembleShaderSource();

                Debug.LogFormat("Shader code:\n---\n{0}\n---", shaderSource); // TODO: after debugging is done

                if (shader == null) {
                    shader = ShaderUtil.CreateShaderAsset(shaderSource);
                    AssetDatabase.AddObjectToAsset(shader, sdfScenePrefabAssetPath);
                } else
                    ShaderUtil.UpdateShaderAsset(shader, shaderSource);

                if (material == null) {
                    material = new Material(shader);
                    AssetDatabase.AddObjectToAsset(material, sdfScenePrefabAssetPath);
                } else
                    material.shader = shader;

                // AssetDatabase.SaveAssets();
            } catch (Exception e) {
                throw new SdfException("Shader generation error", e);
            }
        }
#endif

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

        internal string AssembleShaderSource() => $@"// GENERATED SHADER CONTENT. ANY MODIFICATIONS WILL BE OVERWRITTEN.
// Last modification: {DateTime.Now}

{raymarchingShader.MainShader(this)}
"; // ensure empty line at the bottom

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
                QueuePropertyUpdates(property);
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
                sdfScene.GenerateSceneAssets();
                sdfScene.UpdateShaderUniforms();
            }
            // TODO: open temporary file with this text
            if (GUILayout.Button("Open generated shader")) {
                // AssetDatabase.OpenAsset(sdfScene.controlledShader);
                OpenGeneratedShader();
            }

            if (GUILayout.Button("Assign material to renderer")) {
                var renderer = sdfScene.GetComponent<Renderer>();
                if (renderer)
                    renderer.material = sdfScene.controlledMaterial;
                else
                    EditorGUILayout.HelpBox("No renderer found", MessageType.Error);
            }
            GUILayout.Space(16);
            base.OnInspectorGUI();
            if (propertiesFold = EditorGUILayout.BeginFoldoutHeaderGroup(propertiesFold, "Managed properties")) {
                foreach (var props in sdfScene.Properties) {
                    var ctr = props.Key;
                    if (!(individualFolds[ctr] = EditorGUILayout.Foldout(individualFolds[ctr], sdfScene.GetControllerIdentifier(ctr))))
                        continue;


                    foreach (var prop in ctr.Properties) {
                        EditorGUILayout.LabelField(prop.DisplayName, EditorStyles.boldLabel);
                        if (GUILayout.Button("trigger update for this property"))
                            ctr.SdfScene.QueuePropertyUpdates(prop);
                        EditorGUILayout.TextArea(prop.CurrentValue.ToString());
                    }
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
        public static void WriteToFile(string path, string content) {
            try {
                File.WriteAllText(path, content);
            } catch (Exception e) {
                Debug.LogError(e);
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
            if (externalScriptEditor != "internal")
                InternalEditorUtility.OpenFileAtLineExternal(filePath, 0);
            else {
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
