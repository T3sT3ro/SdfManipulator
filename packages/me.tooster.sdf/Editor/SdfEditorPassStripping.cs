#nullable enable
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;

namespace me.tooster.sdf.Editor {
    /// <summary>
    ///     Leaves the passes that only the editor draws out of player builds: the selection outline and picking of the
    ///     scene view, and the object picking and focus views of SDF scenes. A player never draws them, and each of them
    ///     would add compiled programs with another copy of the scene functions.
    /// </summary>
    class SdfEditorPassStripping : IPreprocessShaders {
        // the names of the passes, see BuiltInGenerator-Shaderlab.cs
        static readonly HashSet<string> editorOnlyPasses = new() { "SceneSelectionPass", "ScenePickingPass", "SdfObjectPicking", "SdfFocus" };

        internal static IReadOnlyCollection<string> EditorOnlyPasses => editorOnlyPasses;

        public int callbackOrder => 0;

        /// Whether the pass <paramref name="passName" /> of the shader <paramref name="shaderName" /> is an editor-only pass of
        /// a generated SDF shader
        internal static bool IsEditorOnly(string shaderName, string passName)
            => shaderName.StartsWith("SDF/") && editorOnlyPasses.Contains(passName);

        public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data) {
            if (IsEditorOnly(shader.name, snippet.passName))
                data.Clear();
        }
    }
}
