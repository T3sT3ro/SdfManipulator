using System.Collections.Generic;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using me.tooster.sdf.Editor.API;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Editor.Controllers {
    [ShaderInclude("Packages/me.tooster.sdf/Editor/Resources/Includes/primitives.hlsl")]
    public class BoxSdfController : TransformController {
        readonly Property<Vector3> boxSize = new Property<Vector3>("size", "Box size", Vector3.one / 4);

        public override IEnumerable<Property> Properties {
            get { yield return boxSize; }
        }

        [MenuItem("GameObject/SDF/Box", priority = -20)]
        public static void CreateSdfBox() => Controller.TryCreateNode<BoxSdfController>("box");
    }
}
