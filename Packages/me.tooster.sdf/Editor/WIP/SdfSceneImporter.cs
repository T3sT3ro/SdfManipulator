using System;
using me.tooster.sdf.Editor.Controllers.SDF;
using me.tooster.sdf.Editor.Controllers.SDF.Primitives;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
namespace me.tooster.sdf.Editor {
    // based on https://github.com/Unity-Technologies/ShaderGraph/blob/master/com.unity.shadergraph/Editor/Importers/ShaderGraphImporter.cs
    /**
     * This importer creates a compound assets: main asset is the prefab, scene description. Sub assets include generated Shader (from scene prefab) and a default material
     */
    [ScriptedImporter(1, "sdf")]
    public class SdfSceneImporter : ScriptedImporter {
        public override void OnImportAsset(AssetImportContext ctx) {
            var oldShader = AssetDatabase.LoadAssetAtPath<Shader>(ctx.assetPath);
            if (oldShader != null)
                ShaderUtil.ClearShaderMessages(oldShader);

            var shaderText = getShaderText(ctx.assetPath);
            var shader = ShaderUtil.CreateShaderAsset(ctx, shaderText, false);

            var sdfScene = PrefabUtility.LoadPrefabContents(ctx.assetPath);
            if (sdfScene is not null) return;

            sdfScene = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sdfScene.AddComponent<SdfScene>();
            var material = new Material(shader);

            ctx.AddObjectToAsset("shader", shader);
            ctx.AddObjectToAsset("scene", sdfScene);
            ctx.SetMainObject(shader);
            ctx.AddObjectToAsset("material", material);
        }

        static string getShaderText(string path) {
            var scene = AssetDatabase.LoadAssetAtPath<SdfScene>(path);
            scene.RequiresRegeneration = true;
            throw new NotImplementedException("Empty sdf scene");
        }

        // similar to CreateShaderGraph
        // [MenuItem("Assets/Create/SDF/Sdf Scene Asset (new)")]
        // static void CreateSdfSceneAsset() {
        //     ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
        //         ScriptableObject.CreateInstance<CreateGraphAction>(),
        //         "Example SDF scene.sdf", null, null);
        // }

        static void CreateSdfSceneAsset(string path) {
            var scene = new GameObject("scene root");
            scene.AddComponent<SdfScene>();

            var defaultCube = new GameObject("cube");
            defaultCube.AddComponent<SdfBoxController>();

            var graphItem = ScriptableObject.CreateInstance<CreateGraphAction>();
            AssetDatabase.CreateAsset(scene, path);
        }



        class CreateGraphAction : EndNameEditAction {
            public override void Action(int instanceId, string pathName, string resourceFile) {
                CreateSdfSceneAsset(pathName);
                AssetDatabase.Refresh();
                var obj = AssetDatabase.LoadAssetAtPath<SdfScene>(pathName);
                Selection.activeObject = obj;
            }
        }
    }
}