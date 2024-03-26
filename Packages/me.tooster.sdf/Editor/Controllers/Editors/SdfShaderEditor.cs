using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
namespace me.tooster.sdf.Editor.Controllers.Editors {
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>For implementation reference, see <code>Editor/Drawing/MaterialEditor/ShaderGraphPropertyDrawers.cs</code></remarks>
    /// <remarks>As well as <a href="https://github.com/Unity-Technologies/UnityCsReference/blob/1b4b79be1f4bedfe18965946323fd565702597ac/Editor/Mono/Inspector/ShaderInspector.cs">ShaderInspector</a></remarks> 
    public class SdfShaderEditor : ShaderGUI {
        private bool    showErrors             = false;
        private Vector2 scrollErrorsPosition   = Vector2.zero;
        private bool    showWarnings           = false;
        private Vector2 scrollWarningsPosition = Vector2.zero;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties) {
            var material = (Material)materialEditor.target;
            var shader = material.shader;

            // DON'T DELETE BELOW - it calls internally FetchCachedMessages, without which the GetShadersMessages has no effect
            var shaderHasMessages =
                ShaderUtil.ShaderHasError(shader) || ShaderUtil.ShaderHasWarnings(shader); // << uncomment to see error window populate
            var messages = ShaderUtil.GetShaderMessages(shader).ToLookup(m => m.severity);

            ShowMessageFold(messages[ShaderCompilerMessageSeverity.Error].ToArray(), MessageType.Error, ref showErrors, "Errors",
                ref scrollErrorsPosition);
            ShowMessageFold(messages[ShaderCompilerMessageSeverity.Warning].ToArray(), MessageType.Warning, ref showWarnings, "Warnings",
                ref scrollWarningsPosition);

            base.OnGUI(materialEditor, properties);
        }

        private void ShowMessageFold(
            IReadOnlyCollection<ShaderMessage> messages,
            MessageType severity,
            ref bool foldEnabled,
            string foldTitle,
            ref Vector2 scrollPos
        ) {
            using (new EditorGUI.DisabledScope(messages.Count == 0)) {
                foldEnabled = EditorGUILayout.BeginFoldoutHeaderGroup(foldEnabled, $"{foldTitle} ({messages.Count})");
                if (foldEnabled)
                    using (new EditorGUILayout.ScrollViewScope(scrollPos)) {
                        foreach (var msg in messages) {
                            EditorGUILayout.PrefixLabel($"{msg.line}:");
                            EditorGUILayout.HelpBox(msg.message, severity);
                        }
                    }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
    }
}
