using UnityEditor;
namespace me.tooster.sdf.Editor.Controllers.Editors {
    /// <summary>
    /// Temporarily this class is empty and only invokes base editor, but in the future it could be improved.
    /// In the future, where UIToolkit supports the materia editor :| 
    /// </summary>
    /// <remarks>For implementation reference, see <code>Editor/Drawing/MaterialEditor/ShaderGraphPropertyDrawers.cs</code></remarks>
    /// <remarks>As well as <a href="https://github.com/Unity-Technologies/UnityCsReference/blob/1b4b79be1f4bedfe18965946323fd565702597ac/Editor/Mono/Inspector/ShaderInspector.cs">ShaderInspector</a></remarks> 
    public class SdfShaderEditor : ShaderGUI {
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties) { base.OnGUI(materialEditor, properties); }
    }
}
