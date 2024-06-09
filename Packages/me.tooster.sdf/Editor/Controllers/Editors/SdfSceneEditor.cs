using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using me.tooster.sdf.Editor.Controllers.Generators;
using me.tooster.sdf.Editor.Controllers.SDF;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using SdfConeController = me.tooster.sdf.Editor.Controllers.SDF.Primitives.SdfConeController;
using SdfSphereController = me.tooster.sdf.Editor.Controllers.SDF.Primitives.SdfSphereController;
namespace me.tooster.sdf.Editor.Controllers.Editors {
    [CustomEditor(typeof(SdfScene))]
    public class SdfSceneEditor : UnityEditor.Editor {
        public override VisualElement CreateInspectorGUI() {
            var sdfScene = (SdfScene)target;


            var root = new VisualElement();
            var editor = Resources.Load<VisualTreeAsset>("UI/sdf_scene_editor").Instantiate();
            root.Add(editor);
            var generatorDropdown = editor.Q<DropdownField>("generatorId");
            generatorDropdown.choices = RaymarchingShaderGenerator.allGenerators.Keys.ToList();
            generatorDropdown.value = sdfScene.raymarchingShaderGenerator;
            var generatorProperty = serializedObject.FindProperty(nameof(sdfScene.raymarchingShaderGenerator));
            generatorDropdown.RegisterValueChangedCallback(
                evt => {
                    generatorProperty.stringValue = evt.newValue;
                    generatorProperty.serializedObject.ApplyModifiedProperties();
                    sdfScene.RequiresRegeneration = true;
                }
            );

            var diagnostics = root.Q<ListView>("diagnostics");

            diagnostics.itemsSource = sdfScene.diagnostics;
            diagnostics.makeItem = () => new HelpBox("test diagnostic", HelpBoxMessageType.Info);
            diagnostics.bindItem = (element, i) => {
                var helpbox = (HelpBox)element;
                helpbox.text = sdfScene.diagnostics[i].message;
                helpbox.messageType = sdfScene.diagnostics[i].severity switch
                {
                    SdfScene.Diagnostic.Severity.ERROR => HelpBoxMessageType.Error,
                    SdfScene.Diagnostic.Severity.WARN  => HelpBoxMessageType.Warning,
                    _                                  => HelpBoxMessageType.Info,
                };
                // rescale if content changes
                helpbox.style.width = new StyleLength(StyleKeyword.Auto);
                helpbox.style.height = new StyleLength(StyleKeyword.Auto);
            };

            root.Q<Button>("rebuild").RegisterCallback<ClickEvent>(e => RebuildShader());
            root.Q<Button>("open").RegisterCallback<ClickEvent>(e => OpenGeneratedShader());


            // InspectorElement.FillDefaultInspector(root, serializedObject, this);
            return root;
        }

        /*
        [UnityEditor.Callbacks.DidReloadScripts]
        static void OnRecompile() { EditorApplication.delayCall += () => { }; }
        */

        void RebuildShader() {
            var sdfScene = (SdfScene)target;

            sdfScene.RevalidateScene();
            sdfScene.GenerateSceneAssets();
            sdfScene.UpdateShaderUniforms(true);
        }

        void OpenGeneratedShader() {
            var sdfScene = (SdfScene)target;

            var assetName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(sdfScene.controlledShader));
            var shaderText = sdfScene.AssembleShaderSource();
            var path = $"Temp/GeneratedSdfScene-{assetName.Replace(" ", "")}.shader";
            WriteToFile(path, shaderText);
            OpenFile(path);
        }

        // From ShaderGraph GraphUtil.cs
        static void WriteToFile(string path, string content) {
            try {
                File.WriteAllText(path, content);
            } catch (Exception e) {
                Debug.LogError(e);
            }
        }

        // From ShaderGraph GraphUtil.cs
        static void OpenFile(string path) {
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
                p.Exited += (obj, args) => {
                    if (p.ExitCode != 0)
                        Debug.LogWarningFormat("Unable to open {0}: Check external editor in preferences", filePath);
                };
                p.Start();
            }
        }

        [MenuItem("GameObject/SDF/Scene")]
        static void CreateSdfScene() { // TODO: add shortcut accelerators to this and nodes (when sdf editing is enabled)
            var scene = GameObject.CreatePrimitive(PrimitiveType.Cube);
            scene.name = "SDF Scene";
            scene.AddComponent<SdfScene>();
        }

        [MenuItem("Assets/Create/SDF/Scene Asset")]
        static void CreateSdfSceneAsset() {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                CreateInstance<CreateSceneAction>(),
                "New SDF scene.prefab",
                null,
                null,
                true
            );
        }



        // based on ShaderGraph's `CreateShaderGraph`
        class CreateSceneAction : EndNameEditAction {
            public override void Action(int instanceId, string pathName, string resourceFile) {
                CreateSdfSceneAsset(pathName);
                AssetDatabase.Refresh();
            }

            /**
             * Creates new scene prefab with sub-asset material and shader and the prefab has the following structure:
             * - new Sdf scene (SdfScene)
             *   |- scene renderer (MeshRenderer + MeshFilter(Cube))
             *   |- root (SdfCombineController) - allows controling the origin of the domain
             *      |- cone (SdfConeController)
             *      |- sphere (SdfSphereController)
             */
            static void CreateSdfSceneAsset(string path) {
                var scenePrefabRoot = new GameObject("new SDF scene");
                var sdfScene = scenePrefabRoot.AddComponent<SdfScene>();

                var domain = GameObject.CreatePrimitive(PrimitiveType.Cube);
                DestroyImmediate(domain.GetComponent<BoxCollider>());
                domain.name = "scene renderer";
                domain.transform.SetParent(scenePrefabRoot.transform);
                domain.transform.localScale = Vector3.one * 3;

                var sceneRoot = new GameObject("root");
                sceneRoot.transform.SetParent(scenePrefabRoot.transform);
                sceneRoot.transform.localPosition = Vector3.down * 1.5f;
                var rootController = sceneRoot.AddComponent<SdfCombineController>();
                rootController.Operation = SdfCombineController.CombinationOperation.SMOOTH_UNION;
                rootController.BlendFactor = 0.3f;

                var coneGameObject = new GameObject("cone");
                coneGameObject.transform.SetParent(sceneRoot.transform);
                var coneController = coneGameObject.AddComponent<SdfConeController>();
                coneController.transform.localPosition = Vector3.up * 2f;

                var sphereGameObject = new GameObject("sphere");
                var sphereController = sphereGameObject.AddComponent<SdfSphereController>();
                sphereGameObject.transform.SetParent(sceneRoot.transform);
                sphereController.transform.localPosition = Vector3.up * 1f;

                sdfScene.sdfSceneRoot = rootController;
                var shader = ShaderUtil.CreateShaderAsset("// empty shader\n", false);
                shader.name = "generated shader";
                var material = new Material(shader) { name = "generated material" };

                var domainRenderer = domain.GetComponent<Renderer>();
                domainRenderer.sharedMaterial = material;

                sdfScene.controlledShader = shader;
                sdfScene.controlledMaterial = material;

                PrefabUtility.SaveAsPrefabAsset(scenePrefabRoot, path);
                AssetDatabase.AddObjectToAsset(shader, path);
                AssetDatabase.AddObjectToAsset(material, path);

                AssetDatabase.SaveAssetIfDirty(AssetDatabase.GUIDFromAssetPath(path));
                DestroyImmediate(scenePrefabRoot);
            }
        }
    }
}
