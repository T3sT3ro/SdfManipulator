using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using me.tooster.sdf.Editor.Controllers.SDF;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
namespace me.tooster.sdf.Editor.Controllers.Editors {
    [CustomEditor(typeof(SdfScene))]
    public class SdfSceneEditor : UnityEditor.Editor {
        public override VisualElement CreateInspectorGUI() {
            var sdfScene = (SdfScene)target;

            var root = new VisualElement();
            var editor = Resources.Load<VisualTreeAsset>("UI/sdf_scene_editor").Instantiate();
            root.Add(editor);

            var diagnostics = root.Q<VisualElement>("diagnostics");
            if (!sdfScene.controlledShader)
                diagnostics.Add(new HelpBox("Missing controlled shader asset.", HelpBoxMessageType.Error));
            if (!sdfScene.raymarchingShader)
                diagnostics.Add(new HelpBox("Raymarching shader asset required.", HelpBoxMessageType.Error));
            if (sdfScene.gameObject.TryGetComponent<Renderer>(out var r) && r.sharedMaterial != sdfScene.controlledMaterial)
                diagnostics.Add(new HelpBox("Material on SdfScene renderer is not the controlled material.", HelpBoxMessageType.Warning));
            if (!sdfScene.controlledMaterial)
                diagnostics.Add(new HelpBox("Controlled material asset required.", HelpBoxMessageType.Error));
            else if (sdfScene.controlledMaterial.shader != sdfScene.controlledShader) {
                diagnostics.Add(
                    new HelpBox(
                        "Controlled material's shader is different than controlled shader.",
                        HelpBoxMessageType.Warning
                    )
                );
            }

            var nonUniformScaled = sdfScene.GetComponentsInChildren<Controller>().Any(
                c => {
                    var s = c.transform.lossyScale;
                    return !(Mathf.Approximately(s.x, s.y) && Mathf.Approximately(s.y, s.z));
                }
            );

            if (nonUniformScaled) {
                diagnostics.Add(
                    new HelpBox(
                        "Some controllers end up with non-uniform scale — it may cause weird rendering artifacts",
                        HelpBoxMessageType.Warning
                    )
                );
            }


            root.Q<Button>("rebuild").RegisterCallback<ClickEvent>(e => RebuildShader());
            root.Q<Button>("open").RegisterCallback<ClickEvent>(e => OpenGeneratedShader());

            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            return root;
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        static void OnRecompile() { EditorApplication.delayCall += () => { }; }

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
             *   |- root (SdfCombineController)
             *      |- cone (SdfConeController)
             *      |- sphere (SdfSphereController)
             */
            static void CreateSdfSceneAsset(string path) {
                var scenePrefabRoot = new GameObject("new SDF scene");
                var sdfScene = scenePrefabRoot.AddComponent<SdfScene>();

                var domain = GameObject.CreatePrimitive(PrimitiveType.Cube);
                DestroyImmediate(domain.AddComponent<BoxCollider>());
                domain.name = "scene renderer";
                domain.transform.SetParent(scenePrefabRoot.transform);
                domain.transform.localScale = Vector3.one * 3;

                var sceneRoot = new GameObject("root");
                sceneRoot.transform.SetParent(scenePrefabRoot.transform);
                var rootController = sceneRoot.AddComponent<SdfCombineController>();
                rootController.Operation = SdfCombineController.CombinationOperation.SMOOTH_UNION;
                rootController.BlendFactor = 0.3f;
                rootController.LocalPosition = Vector3.down * 1.5f;

                var coneGameObject = new GameObject("cone");
                coneGameObject.transform.SetParent(sceneRoot.transform);
                var coneController = coneGameObject.AddComponent<SdfConeController>();
                coneController.LocalPosition = Vector3.up * 2f;

                var sphereGameObject = new GameObject("sphere");
                var sphereController = sphereGameObject.AddComponent<SdfSphereController>();
                sphereGameObject.transform.SetParent(sceneRoot.transform);
                sphereController.LocalPosition = Vector3.up * 1f;

                sdfScene.sdfSceneRoot = rootController;
                var shader = ShaderUtil.CreateShaderAsset("// empty shader\n", false);
                shader.name = "generated shader";
                var material = new Material(shader) { name = "generated material" };

                AssetDatabase.AddObjectToAsset(shader, path);
                AssetDatabase.AddObjectToAsset(material, path);

                var domainRenderer = domain.GetComponent<Renderer>();
                domainRenderer.sharedMaterial = material;

                sdfScene.controlledShader = shader;
                sdfScene.controlledMaterial = material;

                PrefabUtility.SaveAsPrefabAsset(scenePrefabRoot, path);
                DestroyImmediate(scenePrefabRoot);
                AssetDatabase.SaveAssetIfDirty(AssetDatabase.GUIDFromAssetPath(path));
            }
        }
    }
}
