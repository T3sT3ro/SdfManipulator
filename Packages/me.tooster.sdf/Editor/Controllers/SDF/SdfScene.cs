#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.Controllers.ShaderPartials;
using me.tooster.sdf.Editor.Util;
using Unity.Properties;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;
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

        public RaymarchingShader raymarchingShader;
        public Shader?           controlledShader;
        public Material?         controlledMaterial; // similarity: https://docs.unity3d.com/ScriptReference/HideFlags.html

        public SdfController? sdfSceneRoot;

        readonly ShaderPropertyCollector       shaderPropertyCollector = new();
        readonly ShaderPropertyUpdatingVisitor shaderPropertyUpdatingVisitor;

        public SdfScene() => shaderPropertyUpdatingVisitor = new ShaderPropertyUpdatingVisitor(this);

        public SceneData sceneData { get; private set; }
        public bool      IsDirty   { get; set; }

        void Awake() {
            PrefabStage.prefabSaved += OnPrefabSaved;
            IsDirty = true;
        }

        void LateUpdate() {
            var requiresForceUpdate = IsDirty;
            if (IsDirty) {
                IsDirty = false;
                RevalidateScene();
                GenerateSceneAssets();
            }
            UpdateShaderUniforms(forceUpdateAll: requiresForceUpdate);
        }


        // TODO: use more event-driven architecture where creation, move, rename and deletion of individual controllers is detected. 
        void OnValidate() {
            raymarchingShader = RaymarchingShader.instance;
            if (controlledMaterial == null || controlledShader == null)
                IsDirty = true;
            RevalidateScene(); // fixme: Revalidate triggered without regenerate will cause difference between shader content and scene data
            UpdateShaderUniforms(forceUpdateAll: true);
        }

        static void OnPrefabSaved(GameObject go) {
            var scene = go.GetComponent<SdfScene>();
            scene.IsDirty = true;
        }

        internal void RevalidateScene() {
            if (this == null) return; // needed for Unity reasons 
            raymarchingShader = RaymarchingShader.instance;

            if (sdfSceneRoot == null) sdfSceneRoot = GetComponentInChildren<SdfController>();

            var controllerData = new Dictionary<Controller, ControllerData>();

            foreach (var controller in GetComponentsInChildren<Controller>()) {
                controller.StructureChanged -= HandleStructuralChange; // prevent same callback registering twice
                controller.StructureChanged += HandleStructuralChange;

                controller.PropertyChanged -= HandlePropertyChange;
                controller.PropertyChanged += HandlePropertyChange;

                var controllerId = GenerateControllerIdentifier(controller);

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
            if (!controlledMaterial)
                throw new Exception("SdfScene is missing controlled material to update a property!");

            var controller = pd.controller;
            PropertyContainer.Accept(shaderPropertyUpdatingVisitor, ref controller, pd.path);
        }

        void HandleStructuralChange(Controller source) {
            IsDirty = true;
            // RefreshSceneData();
            // GenerateSceneAssets();
            // UpdateShaderUniforms();
        }

        void HandlePropertyChange(object sender, PropertyChangedEventArgs eventArgs) {
            QueuePropertyUpdate(sceneData.controllers[(Controller)sender].properties[new PropertyPath(eventArgs.PropertyName)]);
        }

        internal void UpdateShaderUniforms(bool forceUpdateAll = false) {
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
            string shaderSource;
            // try { // TURNED OF DUE TO DEBUGGER NOT STEPPING INTO THE STACK TRACE: https://youtrack.jetbrains.com/issue/RIDER-18382/Rethrown-exceptions-dont-point-to-the-correct-stack-location
            shaderSource = AssembleShaderSource();
            Debug.LogFormat("Shader code:\n---\n{0}\n---", shaderSource); // TODO: remove after debugging is done
            /*} catch (Exception e) {
                throw new ShaderGenerationException("Shader generation error", e);
            }*/

            if (controlledShader == null)
                throw new ShaderGenerationException("Missing attached shader asset as a target for generation!");

            controlledShader.name = "(generated) main shader";
            ShaderUtil.UpdateShaderAsset(controlledShader, shaderSource);

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

{raymarchingShader.MainShader(this)}
"; // ensure empty line at the bottom

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
        string GenerateControllerIdentifier(Controller controller) {
            var pathFromParent = GetControllerSceneAncestors(controller).Reverse().ToArray();
            var indexedPath = pathFromParent.Select(t => t.GetSiblingIndex().ToString("X")).JoinToString("_");
            return
                @$"SDF_{indexedPath}__{
                    pathFromParent
                        .Select(t => $"{t.name}")
                        .Select(API.Extensions.sanitizeToIdentifierString)
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
                identifierLookup = propertiesData.ToDictionary(pd => pd.identifier);
                pathLookup = propertiesData.ToDictionary(pd => pd.path);
                propertyLookup = propertiesData.ToDictionary(pd => pd.property);
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
