using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Shaderlab;
using me.tooster.sdf.AST.Shaderlab.Syntax;
using me.tooster.sdf.AST.Shaderlab.Syntax.Commands;
using me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific;
using me.tooster.sdf.AST.Shaderlab.Syntax.SubShaderSpecific;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using me.tooster.sdf.Editor.Controllers.SDF;
using me.tooster.sdf.Editor.Util;
using Unity.Properties;
using UnityEngine;
using PropertySyntax = me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific.Property;
using Shader = me.tooster.sdf.AST.Shaderlab.Syntax.Shader;

namespace me.tooster.sdf.Editor.Controllers.ShaderPartials {
    public partial class RaymarchingShader {
        [SerializeField] bool transparent = false;

        public string MainShader(SdfScene scene) {
            if (scene.sdfSceneRoot == null) return "// empty shader";
            var unformatedSource = shader(scene);
            var formattedSource = ShaderlabFormatter.Format(unformatedSource);
            return formattedSource?.ToString() ?? "// empty shader";
        }


        #region shaderlab

        Shader shader(SdfScene scene)
            => new()
            {
                name = scene.name + " (generated)",
                materialProperties = MaterialProperties(scene),
                shaderStatements = new ShaderStatement[]
                {
                    new Fallback { name = "Sdf/Fallback" },
                    new CustomEditor { editor = "me.tooster.sdf.Editor.Controllers.Editors.SdfShaderEditor" },
                    SubShader(scene),
                },
            };

        /// <summary>
        ///  generates a material properties block from collected properties
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">property type is not supported</exception>
        MaterialProperties MaterialProperties(SdfScene scene) {
            return new MaterialProperties
            {
                properties = GlobalShaderProperties
                    .Concat(
                        scene.sceneData.Properties
                            .Where(p => PropertyContainer.GetProperty(p.controller, p.path).IsPropertyShaderlabCompatible())
                            .GroupBy(pd => pd.controller)
                            .SelectMany(
                                group => group.Select(pd => generatePropertySyntax(scene, pd))
                                    .Select(
                                        (ps, i) => i > 0
                                            ? ps
                                            : ps with
                                            {
                                                attributes = SyntaxUnityExtensions.headerAttribute(
                                                    scene.GetControllerSceneAncestors(group.Key).Reverse().Select(c => c.name)
                                                        .JoinToString("/")
                                                ),
                                            }
                                    )
                            )
                    )
                    .ToList(),
            };
        }

        static IEnumerable<PropertySyntax> GlobalShaderProperties {
            get {
                return new[]
                {
                    // [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Int) = 0
                    new PropertySyntax
                    {
                        id = "_Cull", displayName = "Cull", propertyType = new IntKeyword(),
                        initializer = new PropertySyntax.Number<IntLiteral> { numberLiteral = (int)UnityEngine.Rendering.CullMode.Back },
                        attributes = new[]
                        {
                            SyntaxUnityExtensions.headerAttribute("global raymarching properties"),
                            SyntaxUnityExtensions.spaceAttribute(),
                            SyntaxUnityExtensions.enumAttribute<UnityEngine.Rendering.CullMode>(),
                        },
                    },
                    // [Toggle][KeyEnum(Off, On)] _ZWrite ("ZWrite", Float) = 0
                    new PropertySyntax
                    {
                        id = "_ZWrite", displayName = "ZWrite", propertyType = new FloatKeyword(),
                        initializer = new PropertySyntax.Number<IntLiteral> { numberLiteral = 1 },
                        attributes = new[]
                        {
                            SyntaxUnityExtensions.tooltipAttribute(
                                "Enable for correct blending with other geometry and backface rendering"
                            ),
                            SyntaxUnityExtensions.toggleAttribute(),
                            SyntaxUnityExtensions.keyEnumAttribute("Off", "On"),
                        },
                    },
                    // [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Int) = 1
                    new PropertySyntax
                    {
                        id = "_ZTest", displayName = "ZTest", propertyType = new IntKeyword(),
                        initializer = new PropertySyntax.Number<IntLiteral>
                            { numberLiteral = (int)UnityEngine.Rendering.CompareFunction.LessEqual },
                        attributes = SyntaxUnityExtensions.enumAttribute<UnityEngine.Rendering.CompareFunction>(),
                    },
                };
            }
        }

        PropertySyntax generatePropertySyntax(SdfScene scene, SdfScene.PropertyData pd) {
            // FIXME: avoid repeated property reads in upper level
            var t = PropertyContainer.GetProperty(pd.controller, pd.path).DeclaredValueType();

            var shaderlabType = t.shaderlabTypeKeyword();
            PropertySyntax.Initializer shaderlabInitializer = t switch
            {
                not null when t == typeof(int)   => new PropertySyntax.Number<IntLiteral> { numberLiteral = 0 },
                not null when t == typeof(float) => new PropertySyntax.Number<FloatLiteral> { numberLiteral = 0 },
                not null when t == typeof(Vector2) || t == typeof(Vector3) || t == typeof(Vector4)
                 || t == typeof(Vector2Int) || t == typeof(Vector3Int) => new PropertySyntax.Vector
                        { arguments = Vector4.zero.VectorArgumentList() },
                _ => throw new ShaderGenerationException($"Can't generate initializer for shaderlab property {pd}"),
            };


            return new PropertySyntax
                {
                    propertyType = shaderlabType,
                    initializer = shaderlabInitializer,
                } with
                {
                    id = pd.identifier,
                    displayName = PropertyContainer.GetProperty(pd.controller, pd.path)
                        .GetAttribute<ShaderPropertyAttribute>().Description,
                };
        }

        TagsBlock TagsBlock => new()
        {
            tags = new[]
            {
                new Tag { key = "RenderType", value = transparent ? "Transparency" : "Opaque" },
                new Tag { key = "Queue", value = transparent ? "Transparent+1" : "Geometry+1" },
                new Tag { key = "IgnoreProjector", value = "True" },
                new Tag { key = "LightMode", value = "ForwardBase" },
            },
        };

        IEnumerable<SubShaderOrPassStatement> Commands => new List<Command>
        { // TODO: use properties on raymarcher node to set these possibly? 
            new ZTest { operation = new CalculatedArgument { id = "_ZTest" } },
            new Cull { state = new CalculatedArgument { id = "_Cull" } },
            new ZWrite { state = new CalculatedArgument { id = "_ZWrite" } },
            transparent
                ? new Blend
                {
                    blendArguments = new Blend.SrcDstArguments
                    {
                        srcFactorArg = new SrcAlphaKeyword(),
                        dstFactorArg = new OneMinusSrcAlphaKeyword(),
                    },
                }
                : null,
        }.FilterNotNull();

        SubShader SubShader(SdfScene scene)
            => new()
            {
                statements = new SyntaxList<shaderlab, SubShaderOrPassStatement>(TagsBlock)
                    .Splice(1, 0, Commands)
                    .Append<SubShaderOrPassStatement>(Pass(scene))
                    .ToList(),
            };

        Pass Pass(SdfScene scene)
            => new()
            {
                statements = new PassStatement[]
                {
                    new HlslProgram { hlsl = new InjectedLanguage<shaderlab, hlsl>(HlslTree(scene)) },
                },
            };

        #endregion
    }
}
