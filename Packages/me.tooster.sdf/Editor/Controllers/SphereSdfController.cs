using System;
using System.Collections.Generic;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.Controllers.Data;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Editor.Controllers {
    [ShaderInclude("Packages/me.tooster.sdf/Editor/Resources/Includes/primitives.hlsl")]
    public class SphereSdfController : TransformController {
        readonly Property<float> radius = new Property<float>("radius", "Sphere radius", 1f);

        public override IEnumerable<Property> Properties {
            get { yield return radius; }
        }
        
        public Expression<hlsl> SdfEvalExpression(Expression<hlsl> pointExpression) => 
            AST.Hlsl.Extensions.FunctionCall("sdf::primitives::sphere",
                pointExpression, new Identifier { id = SdfScene.GetIdentifier(radius) });

        // TODO: add accelerator
        [MenuItem("GameObject/SDF/Primitives/Sphere", priority = -20)]
        public static void CreateSdfSphere() {
            var target = Selection.activeGameObject;
            var scene = target.GetComponentInParent<SdfScene>();
            if (scene == null)
                throw new Exception("Primitives must be added under Sdf Scene Controller");

            var sdf = new GameObject("sphere");
            var controller = sdf.AddComponent<SphereSdfController>();

            if (Selection.activeObject)
                sdf.transform.SetParent(target.transform);
        }
        
        [MenuItem("GameObject/SDF/Primitives/Sphere", priority = -20)]
        public static void CreateSdfBox() => Controller.TryCreateNode<SphereSdfController>("box");
    }
}
