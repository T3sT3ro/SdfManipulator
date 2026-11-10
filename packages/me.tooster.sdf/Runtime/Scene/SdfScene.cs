#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using me.tooster.sdf.Generators;
using me.tooster.sdf.Util;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Profiling;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
namespace me.tooster.sdf {
    /// <summary>
    ///     SdfSceneController is a game object that handles displaying and controling SdfScene asset.
    ///     TODO: split into asset (holding shader + scene description) and the controller "skeleton" (prefab?) + material (?
    ///     default only?)
    /// </summary>
    [DisallowMultipleComponent]
    [SelectionBase]
    [ExecuteAlways] /* using ExecuteInEditMode has documented problems in prefab edit scene */
    [Icon(SdfPaths.Icons.SdfScene)]
    public class SdfScene : MonoBehaviour { // TODO: inherit from Component to disallow enabling/disabling

        public SdfController? sdfSceneRoot;

        // Set only in the prefab's import result, where they point to the shader and the default material that the import
        // generated as sub-assets of the prefab. A prefab file can't reference its own generated sub-assets, so in the
        // file they are always empty.
        [SerializeField] [HideInInspector] internal Shader?   generatedShader;
        [SerializeField] [HideInInspector] internal Material? generatedMaterial;

        [Tooltip("Preset to use for shader generation")]
        [SerializeReference] public ShaderPreset? shaderPreset;



        public List<Diagnostic> diagnostics = new();


        readonly ShaderPropertyCollector shaderPropertyCollector = new();
        ShaderPropertyUpdatingVisitor?   _shaderPropertyUpdatingVisitor;

        // Uniforms go to each renderer through a property block. Writing them into the material changed the material asset
        // all the time, and made every instance of the prefab share the values of the last one that wrote them.
        MaterialPropertyBlock? propertyBlock;
        Renderer[]             renderers = Array.Empty<Renderer>();

        ShaderPropertyUpdatingVisitor shaderPropertyUpdatingVisitor {
            get { return _shaderPropertyUpdatingVisitor ??= new ShaderPropertyUpdatingVisitor(this); }
        }

        public SceneData sceneData            { get; private set; } = null!;
        public bool      RequiresRegeneration { get; set; }
        public bool      RequiresUpdate       { get; set; }

        void Awake() {
            RequiresUpdate = true;
            shaderPreset ??= new BIRPShaderPreset();
        }

        void LateUpdate() {
            var requiresForceUpdate = RequiresRegeneration;
            if (RequiresRegeneration) {
                RequiresRegeneration = false;
                RevalidateScene();
#if UNITY_EDITOR
                // assure that regeneration is done only in the prefab stage
                if (IsEditingContextOpen(out var prefabStage))
                    GenerateSceneAssets();
#endif
            }
            UpdateShaderUniforms(forceUpdateAll: requiresForceUpdate);
        }

        void OnEnable() {
            RequiresUpdate = true;
            // A player has no OnValidate, and OnDisable unsubscribes from the controllers, so rebuild the scene data here.
            RevalidateScene();
            if (renderers.Length == 0 && Application.isPlaying)
                Debug.LogWarning($"SdfScene '{name}' has no renderer, so nothing draws it.", this);
#if UNITY_EDITOR
            // Prefab Mode loads the prefab file, which can't reference the generated sub-assets, so take them from the import
            // result. Reading prefabContentsRoot isn't allowed in OnEnable, so check that this is the root of the stage.
            if (PrefabStageUtility.GetPrefabStage(gameObject) is { } stage && transform.parent == null)
                UseGeneratedAssetsOf(stage.assetPath);
#endif
        }


        void OnDisable() {
            RequiresUpdate = true;
            UnsubscribeFromControllers();
        }


        // TODO: use more event-driven architecture where creation, move, rename and deletion of individual controllers is detected. 
        void OnValidate() {
            diagnostics.Clear();

            RevalidateScene(); // fixme: Revalidate triggered without regenerate will cause difference between shader content and scene data

            if (shaderPreset == null)
                diagnostics.Add(Diagnostic.Error("No shader preset set. The generation won't proceed."));

            if (renderers.Length == 0)
                diagnostics.Add(Diagnostic.Warn("No renderer draws the SdfScene."));

            if (sdfSceneRoot is not { transform: { parent: not null } })
                diagnostics.Add(Diagnostic.Warn("SdfScene root is not a child of SdfScene."));

            /*foreach (var diagnostic in diagnostics) {
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
            }*/

            RequiresUpdate = true;
        }

        /// <summary>
        ///     Points the scene and its renderers at the generated shader and default material. A renderer keeps its
        ///     material when it has one, for example a material asset that uses the generated shader with custom settings.
        ///     Renderers without a material get the default material.
        /// </summary>
        internal void UseGeneratedAssets(Shader shader, Material material) {
            generatedShader = shader;
            generatedMaterial = material;
            foreach (var r in GetComponentsInChildren<Renderer>(true))
                if (r.sharedMaterial == null)
                    r.sharedMaterial = material;
        }

#if UNITY_EDITOR
        /// <summary>Uses the generated assets from the import result of the prefab at <paramref name="prefabPath" />.</summary>
        internal void UseGeneratedAssetsOf(string prefabPath) {
            var imported = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            var importedScene = imported != null ? imported.GetComponentInChildren<SdfScene>(true) : null;
            if (importedScene is { generatedShader: { } shader, generatedMaterial: { } material })
                UseGeneratedAssets(shader, material);
        }

        bool IsEditingContextOpen(out PrefabStage prefabStageEditingContext)
            => (prefabStageEditingContext = PrefabStageUtility.GetCurrentPrefabStage())?.prefabContentsRoot.gameObject == gameObject;
#endif

        internal void RevalidateScene() {
            if (this == null)
                return; // needed for Unity reasons 

            if (sdfSceneRoot == null)
                sdfSceneRoot = GetComponentInChildren<SdfController>();

            // the renderers of a nested scene draw that scene
            renderers = GetComponentsInChildren<Renderer>(true).Where(r => r.GetComponentInParent<SdfScene>(true) == this).ToArray();
            // uniforms may have new names, so set all of them on the next update
            propertyBlock?.Clear();
            RequiresUpdate = true;
#if UNITY_EDITOR
            EditorApplication.QueuePlayerLoopUpdate(); // in edit mode, LateUpdate only runs when something asks for it
#endif

            var controllerData = new Dictionary<Controller, ControllerData>();
            var usedIds = new HashSet<string>();

            // controllers that moved to another scene must stop updating this one
            UnsubscribeFromControllers();
            var controllers = GetComponentsInChildren<Controller>();
            for (var index = 0; index < controllers.Length; index++) {
                var controller = controllers[index];
                controller.PropertyChanged -= HandlePropertyChange; // in case an earlier revalidation failed halfway
                controller.PropertyChanged += HandlePropertyChange;

                var controllerId = ControllerIdentifier(controller, index, usedIds);

                PropertyContainer.Accept(shaderPropertyCollector, controller);

                var propertyData = shaderPropertyCollector.ShaderProperties.Select(p => {
                        var propertyIdentifier = $"{controllerId}_{p.Property.Name.sanitizeToIdentifierString()}";
                        var shaderId = Shader.PropertyToID(propertyIdentifier);
                        return new PropertyData(
                            controller,
                            shaderId,
                            propertyIdentifier,
                            p.Path,
                            p.Property
                        );
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
            // make sure the controller hasn't been destroyed
            if (pd.controller is { } controller && controller != null)
                PropertyContainer.Accept(shaderPropertyUpdatingVisitor, ref controller, pd.path);
        }

        void UnsubscribeFromControllers() {
            if (sceneData?.controllers == null)
                return;

            foreach (var controller in sceneData.controllers.Keys) {
                if (controller != null)
                    controller.PropertyChanged -= HandlePropertyChange;
            }
        }

        void HandlePropertyChange(object sender, PropertyChangedEventArgs eventArgs) {
            // controllers added after the last revalidation and properties that aren't uniforms have no entry
            if (sceneData == null
             || sender is not Controller controller
             || !sceneData.controllers.TryGetValue(controller, out var controllerData)
             || !controllerData.properties.pathLookup.TryGetValue(new PropertyPath(eventArgs.PropertyName), out var propertyData))
                return;
            QueuePropertyUpdate(propertyData);
        }

        /// <summary>
        ///     Sets the uniforms whose properties changed since the last update on the scene's renderers. Controllers report
        ///     changes, including the transforms that moved. Enabling the scene, revalidating it and any change to a
        ///     controller's serialized data (e.g. an undo) set every uniform.
        /// </summary>
        internal void UpdateShaderUniforms(bool forceUpdateAll = false) {
            forceUpdateAll |= RequiresUpdate;
            RequiresUpdate = false;

            if (sceneData == null || renderers.Length == 0)
                return;

            if (forceUpdateAll)
                sceneData.queuedPropertyUdates.UnionWith(sceneData.Properties);
            if (sceneData.queuedPropertyUdates.Count <= 0)
                return;

            propertyBlock ??= new MaterialPropertyBlock();
            foreach (var p in sceneData.queuedPropertyUdates)
                UpdateProperty(p);
            sceneData.queuedPropertyUdates.Clear();

            foreach (var r in renderers)
                if (r != null)
                    r.SetPropertyBlock(propertyBlock);
        }

        void QueuePropertyUpdate(PropertyData propertyData) {
            sceneData.queuedPropertyUdates.Add(propertyData);
#if UNITY_EDITOR
            EditorApplication.QueuePlayerLoopUpdate(); // needed for smooth experience in editor
#endif
        }

        /// <summary>The name of the shader that the last generation produced, see <see cref="GeneratedShaderName" />.</summary>
        public string ShaderName { get; private set; } = "";

        /// <param name="prefabGuid">The GUID of the scene's prefab, which makes the shader name unique.</param>
        internal string AssembleShaderSource(string? prefabGuid = null) {
            if (shaderPreset == null)
                throw new ShaderGenerationException("No shader preset attached, generation won't proceed!");
            ShaderName = GeneratedShaderName(name, prefabGuid);
            return $@"// GENERATED SHADER CONTENT. ANY MODIFICATIONS WILL BE OVERWRITTEN.

{shaderPreset.MainShaderForScene(this)}
";
        }

        /// <summary>
        ///     Returns <c>SDF/&lt;scene name&gt; &lt;first 8 characters of the prefab GUID&gt;</c>, so scenes with the same name
        ///     get different shaders. A slash in the name would add a submenu to the shader menu, and ShaderLab strings can't
        ///     escape a quote, so slashes become dashes and double quotes become single quotes.
        /// </summary>
        internal static string GeneratedShaderName(string sceneName, string? prefabGuid) {
            var escaped = new string(sceneName.Select(c => c switch { '/' or '\\' => '-', '"' => '\'', '\n' or '\r' => ' ', _ => c }).ToArray()).Trim();
            return string.IsNullOrEmpty(prefabGuid) ? $"SDF/{escaped}" : $"SDF/{escaped} {prefabGuid!.Substring(0, Math.Min(8, prefabGuid.Length))}";
        }
        // ensure empty line at the bottom
        /* Used to register a controller under the scene */


        /// <summary>
        ///     Returns a sequence of transforms starting (and excluding) scene name down to the controller.
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public IEnumerable<Transform> GetControllerSceneAncestors(Controller controller)
            => controller.transform.AncestorsAndSelf().TakeWhile(t => t != transform);

        /// <summary>
        ///     Returns <c>SDF_&lt;id&gt;</c>, built from the controller's <see cref="Controller.Id" />, so renaming or moving
        ///     objects doesn't rename uniforms and functions.
        ///     <para>
        ///         In the editor, a controller in an open scene, e.g. in Prefab Mode, gets a new identifier if it has none or
        ///         if an earlier controller uses the same one, e.g. after duplicating an object. The new identifier is saved
        ///         with the prefab. The prefab import, prefab assets and prefab instances never change identifiers: they have
        ///         to agree with the saved file. There, a controller without a unique identifier gets one from its position in
        ///         the scene, which is the same for every load of the same file.
        ///     </para>
        /// </summary>
        string ControllerIdentifier(Controller controller, int index, ISet<string> usedIds) {
#if UNITY_EDITOR
            if ((string.IsNullOrEmpty(controller.Id) || usedIds.Contains(controller.Id)) && CanAssignIds(controller)) {
                do controller.AssignNewId();
                while (usedIds.Contains(controller.Id));
            }
#endif
            return !string.IsNullOrEmpty(controller.Id) && usedIds.Add(controller.Id) ? $"SDF_{controller.Id}" : $"SDF_x{index}";
        }

#if UNITY_EDITOR
        // objects of a prefab asset or of its import result aren't in a scene
        static bool CanAssignIds(Controller controller)
            => !Application.isPlaying && controller.gameObject.scene.IsValid() && !PrefabUtility.IsPartOfPrefabInstance(controller);
#endif



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



        // TODO: bind action callbacks with shaderId in the closure to update the material instead of dispatching with visitor
        class ShaderPropertyUpdatingVisitor
            :
                PropertyVisitor,
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

            public void Visit<TContainer>(in VisitContext<TContainer, Matrix4x4> context, ref TContainer container, ref Matrix4x4 value)
                => scene.propertyBlock!.SetMatrix(GetShaderId(container, context.Property), value);

            public void Visit<TContainer>(in VisitContext<TContainer, Vector2> context, ref TContainer container, ref Vector2 value)
                => scene.propertyBlock!.SetVector(GetShaderId(container, context.Property), value);

            public void Visit<TContainer>(
                in VisitContext<TContainer, Vector2Int> context,
                ref TContainer container,
                ref Vector2Int value
            )
                => scene.propertyBlock!.SetVector(GetShaderId(container, context.Property), (Vector2)value);

            public void Visit<TContainer>(in VisitContext<TContainer, Vector3> context, ref TContainer container, ref Vector3 value)
                => scene.propertyBlock!.SetVector(GetShaderId(container, context.Property), value);

            public void Visit<TContainer>(
                in VisitContext<TContainer, Vector3Int> context,
                ref TContainer container,
                ref Vector3Int value
            )
                => scene.propertyBlock!.SetVector(GetShaderId(container, context.Property), (Vector3)value);

            public void Visit<TContainer>(in VisitContext<TContainer, Vector4> context, ref TContainer container, ref Vector4 value)
                => scene.propertyBlock!.SetVector(GetShaderId(container, context.Property), value);

            public void Visit<TContainer>(in VisitContext<TContainer, bool> context, ref TContainer container, ref bool value)
                => scene.propertyBlock!.SetFloat(GetShaderId(container, context.Property), value ? 1 : 0);

            public void Visit<TContainer>(in VisitContext<TContainer, float> context, ref TContainer container, ref float value)
                => scene.propertyBlock!.SetFloat(GetShaderId(container, context.Property), value);

            public void Visit<TContainer>(in VisitContext<TContainer, int> context, ref TContainer container, ref int value)
                => scene.propertyBlock!.SetInteger(GetShaderId(container, context.Property), value);

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

#if UNITY_EDITOR

        /// <summary>
        ///     Updates the generated shader in memory from the current state of the scene, so structural changes show up in
        ///     Prefab Mode before the prefab is saved. Saving the prefab reimports it, which generates the same shader from
        ///     the saved file. Does nothing outside Prefab Mode: the shader belongs to the prefab, so an instance with a
        ///     different structure can't change it.
        /// </summary>
        /// <exception cref="ShaderGenerationException"></exception>
        internal void GenerateSceneAssets() {
            if (!IsEditingContextOpen(out var stage) || generatedShader == null)
                return;

            Profiler.BeginSample("shader text generation");
            var shaderSource = AssembleShaderSource(AssetDatabase.AssetPathToGUID(stage.assetPath));
            Profiler.EndSample();
            ShaderUtil.UpdateShaderAsset(generatedShader, shaderSource, false);
        }

        /// <returns>true if <paramref name="subAsset" /> is stored in the asset file, and not generated by the import</returns>
        internal static bool IsStoredInFile(UnityEngine.Object subAsset, string path)
            => AssetDatabase.TryGetGUIDAndLocalFileIdentifier(subAsset, out _, out long localId)
             && File.ReadAllText(path).Contains($" &{localId}\n");

        // Unity calls context menu methods without arguments, so they can't take a MenuCommand
        [ContextMenu("Regenerate shader")]
        void RegenerateShaderMenu() {
            if (!IsEditingContextOpen(out _)) {
                Debug.LogWarning("Open the SDF scene prefab in Prefab Mode to regenerate its shader.", this);
                return;
            }
            RevalidateScene();
            GenerateSceneAssets();
        }

        [ContextMenu("Remove sub-assets")]
        void RemoveSubAssets() {
            // in Prefab Mode the edited objects aren't the asset, so GetAssetPath returns an empty path
            var path = IsEditingContextOpen(out var prefabStage) ? prefabStage.assetPath : AssetDatabase.GetAssetPath(gameObject);
            if (string.IsNullOrEmpty(path)) {
                Debug.LogWarning("Open the SDF scene prefab in Prefab Mode, or select the prefab asset, to remove its sub-assets.", this);
                return;
            }

            var removed = false;
            // generated sub-assets aren't stored in the file; the next import would recreate them anyway
            foreach (var subAsset in AssetDatabase.LoadAllAssetRepresentationsAtPath(path).Where(a => IsStoredInFile(a, path))) {
                if (!EditorUtility.DisplayDialog(
                        "Remove sub-asset",
                        $"Delete the sub-asset {subAsset.name} ({subAsset.GetType().Name}) from {path}? Existing references to it will break!",
                        "Delete",
                        "Keep"
                    ))
                    continue;
                AssetDatabase.RemoveObjectFromAsset(subAsset);
                removed = true;
            }
            if (removed)
                AssetDatabase.SaveAssets();
        }

#endif
    }
}
