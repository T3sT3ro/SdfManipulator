#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using me.tooster.sdf.Editor.Controllers.Generators;
using me.tooster.sdf.Editor.Util;
using Unity.Properties;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Profiling;
namespace me.tooster.sdf.Editor.Controllers.SDF {
    /// <summary>
    /// SdfSceneController is a game object that handles displaying and controling SdfScene asset.
    /// TODO: split into asset (holding shader + scene description) and the controller "skeleton" (prefab?) + material (? default only?)
    /// </summary>
    [DisallowMultipleComponent]
    [SelectionBase]
    [ExecuteAlways] /* using ExecuteInEditMode has documented problems in prefab edit scene */
    [Icon("Packages/me.tooster.sdf/Editor/Resources/Icons/sdf-scene-914.png")]
    public class SdfScene : MonoBehaviour { // TODO: inherit from Component to disallow enabling/disabling

        public Shader?   controlledShader;   // TODO: remove, generate material only
        public Material? controlledMaterial; // similarity: https://docs.unity3d.com/ScriptReference/HideFlags.html


        public SdfController? sdfSceneRoot;


        [Tooltip("A target shader asset to regenerate")]
        public Shader? targetShader;
        [Tooltip("A target material to regenerate shader with")]
        public Material? targetMaterial; // Todo: Materials[], plural, support control of many, maybe?
        [Tooltip("Preset to use for shader generation")]
        [SerializeReference] public ShaderPreset? shaderPreset;


        readonly ShaderPropertyCollector shaderPropertyCollector = new();
        ShaderPropertyUpdatingVisitor?   _shaderPropertyUpdatingVisitor;

        ShaderPropertyUpdatingVisitor shaderPropertyUpdatingVisitor {
            get { return _shaderPropertyUpdatingVisitor ??= new ShaderPropertyUpdatingVisitor(this); }
        }



        [Serializable]
        public struct Diagnostic {
            public enum Severity { ERROR, WARN, INFO }

            Diagnostic(string message, Severity severity) => (this.message, this.severity) = (message, severity);
            public Severity severity;
            public string   message;

            public static Diagnostic Error(string message) => new() { severity = Severity.ERROR, message = message };
            public static Diagnostic Warn(string message)  => new() { severity = Severity.WARN, message = message };
            public static Diagnostic Info(string message)  => new() { severity = Severity.INFO, message = message };
        }



        public List<Diagnostic> diagnostics = new();

        public SceneData sceneData            { get; private set; }
        public bool      RequiresRegeneration { get; set; }
        public bool      RequiresUpdate       { get; set; }

        void Awake() {
            RequiresUpdate = true;
            shaderPreset ??= (ShaderPreset)Activator.CreateInstance(ShaderPreset.DetectedShaderPresets[0]);
        }

        void OnEnable() {
            AssemblyReloadEvents.afterAssemblyReload += MarkDirty;
            // if (!RequiresUpdate)
            RequiresUpdate = true;
            PrefabStage.prefabSaved += OnPrefabSaved;
        }


        void OnDisable() {
            AssemblyReloadEvents.afterAssemblyReload -= MarkDirty;
            RequiresUpdate = true;
            PrefabStage.prefabSaved -= OnPrefabSaved;
        }

        void MarkDirty() {
            RequiresRegeneration = true;
            RequiresUpdate = true;
        }

        void OnPrefabSaved(GameObject go) { MarkDirty(); }

        void LateUpdate() {
            var requiresForceUpdate = RequiresRegeneration;
            if (RequiresRegeneration) {
                RequiresRegeneration = false;
                RevalidateScene();
                // assure that regeneration is done only in the prefab stage
                if (PrefabStageUtility.GetCurrentPrefabStage()?.prefabContentsRoot.gameObject == gameObject)
                    GenerateSceneAssets();
            }
            UpdateShaderUniforms(forceUpdateAll: requiresForceUpdate);
        }


        // TODO: use more event-driven architecture where creation, move, rename and deletion of individual controllers is detected. 
        void OnValidate() {
            diagnostics.Clear();
            if ((shaderPreset ??= ShaderPreset.InstantiatePreset(ShaderPreset.DetectedShaderPresets[0])) == null)
                diagnostics.Add(Diagnostic.Error("No shader preset set. The generation won't proceed."));

            if (controlledMaterial == null) {
                controlledMaterial = GetComponent<Material>();
                if (controlledMaterial == null)
                    diagnostics.Add(Diagnostic.Error("No material assigned, generation won't proceed"));
            } else if (controlledMaterial.shader != controlledShader)
                diagnostics.Add(Diagnostic.Warn("Controlled material's shader is different than controlled shader."));


            if (TryGetComponent<Renderer>(out var r) && r.sharedMaterial != controlledMaterial)
                diagnostics.Add(Diagnostic.Warn("Material on SdfScene renderer is not the controlled material."));

            if (sdfSceneRoot is not { transform: { parent: { } sdfSceneRootParentTransform } } || sdfSceneRootParentTransform != transform)
                diagnostics.Add(Diagnostic.Warn("SdfScene root is not a child of SdfScene."));

            if (controlledMaterial == null || controlledShader == null)
                RequiresRegeneration = true;

            RevalidateScene(); // fixme: Revalidate triggered without regenerate will cause difference between shader content and scene data
            foreach (var diagnostic in diagnostics) {
                switch (diagnostic.severity) {
                    case Diagnostic.Severity.ERROR:
                        Debug.LogError(diagnostic.message);
                        break;
                    case Diagnostic.Severity.WARN:
                        Debug.LogWarning(diagnostic.message);
                        break;
                    case Diagnostic.Severity.INFO:
                        Debug.Log(diagnostic.message);
                        break;
                }
            }

            RequiresUpdate = true;
        }

        internal void RevalidateScene() {
            if (this == null) return; // needed for Unity reasons 

            if (sdfSceneRoot == null) sdfSceneRoot = GetComponentInChildren<SdfController>();

            var controllerData = new Dictionary<Controller, ControllerData>();

            foreach (var controller in GetComponentsInChildren<Controller>()) {
                controller.StructureChanged -= HandleStructuralChange; // prevent same callback registering twice
                controller.StructureChanged += HandleStructuralChange;

                controller.PropertyChanged -= HandlePropertyChange;
                controller.PropertyChanged += HandlePropertyChange;

                var controllerId = generateControllerIdentifier(controller);

                PropertyContainer.Accept(shaderPropertyCollector, controller);

                var propertyData = shaderPropertyCollector.ShaderProperties.Select(
                    p => {
                        var propertyIdentifier = $"{controllerId}_{p.Property.Name.sanitizeToIdentifierString()}";
                        var shaderId = Shader.PropertyToID(propertyIdentifier);
                        return new PropertyData(controller, shaderId, propertyIdentifier, p.Path, p.Property);
                    }
                );

                controllerData[controller] = new ControllerData(
                    controllerId,
                    controllerData.Count + 1, // controllers are assigned sequential IDs starting from 1
                    new PropertyCache(propertyData)
                );
            }

            sceneData = new SceneData(
                controllerData,
                new HashSet<PropertyData>(
                    controllerData.Values.SelectMany(cd => cd.properties)
                )
            );
        }

        void UpdateProperty(PropertyData pd) {
            var currentPrefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            // Trigger updates only from prefab instance outside of prefab stage or prefab root inside prefab stage.
            // Otherwise an instance of a prefab in scene would leak to prefab edited in context
            if (currentPrefabStage != null && currentPrefabStage.prefabContentsRoot != gameObject)
                return;

            if (controlledMaterial == null)
                throw new Exception("SdfScene is missing controlled material to update a property!");

            // make sure the controller hasn't been destroyed
            if (pd.controller is { } controller && controller != null)
                PropertyContainer.Accept(shaderPropertyUpdatingVisitor, ref controller, pd.path);
        }

        void HandleStructuralChange(Controller source) {
            RequiresRegeneration = true;
            // RefreshSceneData();
            // GenerateSceneAssets();
            // UpdateShaderUniforms();
        }

        void HandlePropertyChange(object sender, PropertyChangedEventArgs eventArgs) {
            QueuePropertyUpdate(sceneData.controllers[(Controller)sender].properties[new PropertyPath(eventArgs.PropertyName)]);
        }

        internal void UpdateShaderUniforms(bool forceUpdateAll = false) {
            RequiresUpdate = false;

#if UNITY_EDITOR
            forceUpdateAll = true; // TODO: this is a temporary solution, but causes poor performance
#endif

            if (forceUpdateAll) QueuePropertyUpdates(sceneData.Properties);
            if (sceneData.queuedPropertyUdates.Count <= 0)
                return;

            foreach (var p in sceneData.queuedPropertyUdates)
                UpdateProperty(p);

            sceneData.queuedPropertyUdates.Clear();
        }

        void QueuePropertyUpdate(PropertyData propertyData) {
            sceneData.queuedPropertyUdates.Add(propertyData);
            EditorApplication.QueuePlayerLoopUpdate(); // needed for smooth experience in editor
        }

        void QueuePropertyUpdates(IEnumerable<PropertyData> properties) {
            sceneData.queuedPropertyUdates.UnionWith(properties);
            EditorApplication.QueuePlayerLoopUpdate(); // needed for smooth experience in editor
        }

#if UNITY_EDITOR

        /// <summary>
        /// Generates the Shader and Material as sub-assets of the prefab, if inside the prefab stage.
        /// Outside of prefab stage, this function doesn't trigger and print a warning instead.
        /// The intent behind that is that shaders are compiled, so there is 1 to 1 mapping between prefab scene and generated shader.
        /// Any instance with changed structure would require another shader asset, or otherwise would conflict with the prefab's shader.
        /// </summary>
        /// <exception cref="ShaderGenerationException"></exception>
        internal void GenerateSceneAssets() {
            // We allow editing and regenerating shaders only in prefab stage
            if (PrefabStageUtility.GetPrefabStage(gameObject) is not { } prefabStage)
                return;

            var sdfScenePrefabAssetPath = prefabStage.assetPath;
            if (controlledShader == null)
                throw new ShaderGenerationException("Missing attached shader asset as a target for generation!");
            if (shaderPreset == null)
                throw new ShaderGenerationException("Shader preset for generation missing, generation won't proceed!");

            Profiler.BeginSample("shader text generation");
            var shaderSource = AssembleShaderSource();
            Profiler.EndSample();

            controlledShader.name = "(generated) main shader";
            if (AssetDatabase.IsSubAsset(controlledShader)) {
                ShaderUtil.UpdateShaderAsset(controlledShader, shaderSource);
                // Debug.LogFormat(
                //     "Shader sub-asset updated, Shader code:\n---\n{0}\n---",
                //     shaderSource
                // ); // debug
            }
            if (targetShader != null) {
                var path = AssetDatabase.GetAssetPath(targetShader);
                if (File.Exists(path)) {
                    File.WriteAllText(path, shaderSource);
                    AssetDatabase.SaveAssetIfDirty(targetShader);
                }
            }

            if (targetMaterial != null) {
                var path = AssetDatabase.GetAssetPath(targetMaterial);
                if (File.Exists(path)) {
                    File.WriteAllText(path, shaderSource);
                    AssetDatabase.SaveAssetIfDirty(targetMaterial);
                }
            }

            AssetDatabase.Refresh();

            // if (commonInclude == null) {
            //     var txt = new TextAsset();
            //     txt.name = "common.hlsl";
            // }

            // AssetDatabase.SaveAssetIfDirty(AssetDatabase.GUIDFromAssetPath(prefabStage.assetPath)); // this caused preventing adding objects to prefab in prefab stage 
            // AssetDatabase.SaveAssets();
        }
#endif

        internal string AssembleShaderSource()
            => $@"// GENERATED SHADER CONTENT. ANY MODIFICATIONS WILL BE OVERWRITTEN.
// Last modification: {DateTime.Now}

{shaderPreset.MainShaderForScene(this)}
";
        // ensure empty line at the bottom
        /* Used to register a controller under the scene */


        /// <summary>
        /// Returns a sequence of transforms starting (and excluding) scene name down to the controller.
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public IEnumerable<Transform> GetControllerSceneAncestors(Controller controller)
            => controller.transform.AncestorsAndSelf().TakeWhile(t => t != transform);

        /* TODO: calculate appropriate controller paths on the RebuildSceneData step to assure identifiers are unique and shortest possible
         For example let a hierarchy of Root.Character.{Leg(box),Arm(box)} instead of
         - Root_Character_Leg__size -> Leg_size
         - Root_Character_Arm__size -> Arm_size
        */
        string generateControllerIdentifier(Controller controller) {
            var pathFromParent = GetControllerSceneAncestors(controller).Reverse().ToArray();
            var indexedPath = pathFromParent.Select(t => t.GetSiblingIndex().ToString("X")).JoinToString("_");
            return
                @$"SDF_{indexedPath}__{
                    pathFromParent
                        .Select(t => $"{t.name}")
                        .Select(Extensions.sanitizeToIdentifierString)
                        .JoinToString("_")
                }__{controller.GetComponentIndex().ToString()}";
        }



        // TODO: bind action callbacks with shaderId in the closure to update the material instead of dispatching with visitor
        class ShaderPropertyUpdatingVisitor
            : PropertyVisitor,
              IVisitPropertyAdapter<int>,
              IVisitPropertyAdapter<float>,
              IVisitPropertyAdapter<bool>,
              IVisitPropertyAdapter<Vector2>,
              IVisitPropertyAdapter<Vector3>,
              IVisitPropertyAdapter<Vector4>,
              IVisitPropertyAdapter<Vector2Int>,
              IVisitPropertyAdapter<Vector3Int>,
              IVisitPropertyAdapter<Matrix4x4> {
            readonly SdfScene scene;

            public ShaderPropertyUpdatingVisitor(SdfScene scene) {
                AddAdapter(this);
                this.scene = scene;
            }

            public void Visit<TContainer>(in VisitContext<TContainer, bool> context, ref TContainer container, ref bool value)
                => scene.controlledMaterial!.SetFloat(GetShaderId(container, context.Property), value ? 1 : 0);

            public void Visit<TContainer>(in VisitContext<TContainer, float> context, ref TContainer container, ref float value)
                => scene.controlledMaterial!.SetFloat(GetShaderId(container, context.Property), value);

            public void Visit<TContainer>(in VisitContext<TContainer, int> context, ref TContainer container, ref int value)
                => scene.controlledMaterial!.SetInteger(GetShaderId(container, context.Property), value);

            public void Visit<TContainer>(in VisitContext<TContainer, Matrix4x4> context, ref TContainer container, ref Matrix4x4 value)
                => scene.controlledMaterial!.SetMatrix(GetShaderId(container, context.Property), value);

            public void Visit<TContainer>(in VisitContext<TContainer, Vector2> context, ref TContainer container, ref Vector2 value)
                => scene.controlledMaterial!.SetVector(GetShaderId(container, context.Property), value);

            public void Visit<TContainer>(
                in VisitContext<TContainer, Vector2Int> context,
                ref TContainer container,
                ref Vector2Int value
            )
                => scene.controlledMaterial!.SetVector(GetShaderId(container, context.Property), (Vector2)value);

            public void Visit<TContainer>(in VisitContext<TContainer, Vector3> context, ref TContainer container, ref Vector3 value)
                => scene.controlledMaterial!.SetVector(GetShaderId(container, context.Property), value);

            public void Visit<TContainer>(
                in VisitContext<TContainer, Vector3Int> context,
                ref TContainer container,
                ref Vector3Int value
            )
                => scene.controlledMaterial!.SetVector(GetShaderId(container, context.Property), (Vector3)value);

            public void Visit<TContainer>(in VisitContext<TContainer, Vector4> context, ref TContainer container, ref Vector4 value)
                => scene.controlledMaterial!.SetVector(GetShaderId(container, context.Property), value);

            int GetShaderId<TContainer>(TContainer controller, IProperty property)
                => scene.sceneData
                    .controllers[
                        controller as Controller
                     ?? throw new InvalidOperationException($"Controller wasn't properly registered in the scene: {controller}")
                    ].properties[property].shaderId;

            protected override void VisitProperty<TContainer, TValue>(
                Property<TContainer, TValue> property,
                ref TContainer container,
                ref TValue value
            )
                => throw new ArgumentOutOfRangeException(
                    nameof(property),
                    property,
                    $"Unhandled update for property '{property.Name}' in '{container.ToString()}'"
                );
        }



/*
 * - TODO: adopt "prop drilling" for passing props to Data classess to decouple them from the Scene
 */
        [Serializable]
        public record SceneData(
            IReadOnlyDictionary<Controller, ControllerData> controllers,
            HashSet<PropertyData> queuedPropertyUdates
        ) {
            public IEnumerable<PropertyData> Properties => controllers.Values.SelectMany(cd => cd.properties);
        }



        [Serializable]
        public record ControllerData(
            string identifier,
            int numericId,
            PropertyCache properties
        );



        [Serializable]
        public record PropertyData(
            Controller controller,
            int shaderId,
            string identifier,
            PropertyPath path,
            IProperty property
        );



        public class PropertyCache : IEnumerable<PropertyData> {
            public readonly IReadOnlyDictionary<string, PropertyData>       identifierLookup;
            public readonly IReadOnlyDictionary<PropertyPath, PropertyData> pathLookup;
            public readonly IReadOnlyDictionary<IProperty, PropertyData>    propertyLookup;

            public PropertyCache(IEnumerable<PropertyData> propertiesData) {
                var identifiers = new Dictionary<string, PropertyData>();
                var paths = new Dictionary<PropertyPath, PropertyData>();
                var properties = new Dictionary<IProperty, PropertyData>();

                foreach (var pd in propertiesData) {
                    identifiers[pd.identifier] = pd;
                    paths[pd.path] = pd;
                    properties[pd.property] = pd;
                }

                identifierLookup = identifiers;
                pathLookup = paths;
                propertyLookup = properties;
            }

            public PropertyData this[string propertyId] => identifierLookup[propertyId];
            public PropertyData this[PropertyPath propertyPath] => pathLookup[propertyPath];
            public PropertyData this[IProperty property] => propertyLookup[property];

            /// Returns all PropertyData in this cache
            public IEnumerator<PropertyData> GetEnumerator() => pathLookup.Values.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
