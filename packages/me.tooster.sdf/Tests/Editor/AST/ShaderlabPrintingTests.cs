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
using NUnit.Framework;
using static me.tooster.sdf.AST.Hlsl.Extensions;
using Attribute = me.tooster.sdf.AST.Shaderlab.Syntax.ShaderSpecific.Attribute;
using FloatLiteral = me.tooster.sdf.AST.Shaderlab.Syntax.FloatLiteral;
using HlslFloat = me.tooster.sdf.AST.Hlsl.Syntax.FloatKeyword;
using IntLiteral = me.tooster.sdf.AST.Shaderlab.Syntax.IntLiteral;
using QuotedStringLiteral = me.tooster.sdf.AST.Shaderlab.Syntax.QuotedStringLiteral;

namespace me.tooster.sdf.Tests.Editor.AST {
    /// <summary>
    ///     Every ShaderLab syntax node prints valid ShaderLab after formatting. <see cref="EveryNodeTypeHasAPrintingTest" />
    ///     checks that the cases cover every node type.
    /// </summary>
    public class ShaderlabPrintingTests {
        static QuotedStringLiteral Quoted(string text) => text;

        static InjectedLanguage<shaderlab, hlsl> Hlsl(string variable)
            => new(new Tree<hlsl>(Var((me.tooster.sdf.AST.Hlsl.Syntax.Type)new HlslFloat(), variable)));

        static Property RangeProperty
            => new()
            {
                attributes = new Attribute { id = "Header", arguments = (ArgumentList<Attribute.Value>)(Attribute.Value)"Values" },
                id = "_Value",
                displayName = Quoted("Value"),
                propertyType = new Property.RangeType { min = (FloatLiteral)0f, max = (FloatLiteral)1f },
                initializer = new Property.Number<FloatLiteral> { numberLiteral = 0.5f },
            };

        static Property IntegerProperty
            => new()
            {
                id = "_Count", displayName = Quoted("Count"), propertyType = new IntegerKeyword(),
                initializer = new Property.Number<IntLiteral> { numberLiteral = 2 },
            };

        static Property ColorProperty
            => new()
            {
                id = "_Color", displayName = Quoted("Color"), propertyType = new ColorKeyword(),
                initializer = new Property.Vector
                {
                    arguments = new LiteralExpression[] { 1f, 0.5f, 0f, 1f },
                },
            };

        static Property TextureProperty
            => new()
            {
                id = "_MainTex", displayName = Quoted("Texture"), propertyType = new Texture2DKeyword(),
                initializer = new Property.Texture { textureName = Quoted("white") },
            };

        static TagsBlock Tags => new() { tags = new Tag { key = Quoted("Queue"), value = Quoted("Geometry") } };

        static Pass Pass
            => new()
            {
                statements = new PassStatement[]
                {
                    new Pass.Name { name = Quoted("Main") },
                    new HlslProgram { hlsl = Hlsl("y") },
                },
            };

        static IEnumerable<(string name, Tree<shaderlab>.Node node, string expected)> Cases() {
            yield return ("Identifier", (Identifier)"Foo", "Foo");

            // commands
            yield return ("Cull", new Cull { state = new BackKeyword() }, "Cull Back");
            yield return ("ZWrite", new ZWrite { state = new OffKeyword() }, "ZWrite Off");
            yield return ("ZTest", new ZTest { operation = new LessEqualKeyword() }, "ZTest LessEqual");
            yield return ("ZClip with a calculated argument", new ZClip { enabled = new CalculatedArgument { id = "_ZClip" } }, "ZClip [_ZClip]");
            yield return ("BlendOp", new BlendOp { operationArg = new AddKeyword() }, "BlendOp Add");
            yield return ("Blend.StateArgument", new Blend { blendArguments = new Blend.StateArgument() }, "Blend Off");
            yield return ("Blend.SrcDstArguments", new Blend { blendArguments = new Blend.SrcDstArguments { dstFactorArg = new OneMinusSrcAlphaKeyword() } },
                "Blend SrcAlpha OneMinusSrcAlpha");
            yield return ("Blend.RgbaFactorArguments", new Blend
                {
                    blendArguments = new Blend.RgbaFactorArguments
                    {
                        srcRgbFactor = new OneKeyword(), dstRgbFactor = new ZeroKeyword(),
                        srcAlphaFactor = new SrcAlphaKeyword(), dstAlphaFactor = new DstAlphaKeyword(),
                    },
                },
                "Blend One Zero, SrcAlpha DstAlpha");

            // shader and subshader statements
            yield return ("Lod", new Lod { level = 100 }, "LOD 100");
            yield return ("Fallback", new Fallback { name = Quoted("Diffuse") }, "Fallback \"Diffuse\"");
            yield return ("CustomEditor", new CustomEditor { editor = Quoted("TestEditor") }, "CustomEditor \"TestEditor\"");
            yield return ("CustomEditorForRP", new CustomEditorForRP { editor = Quoted("TestEditor"), pipeline = Quoted("UniversalRenderPipelineAsset") },
                "CustomEditorForRenderPipeline \"TestEditor\" \"UniversalRenderPipelineAsset\"");
            yield return ("TagsBlock", Tags, "Tags {\n    \"Queue\" = \"Geometry\"\n}");

            // properties
            yield return ("Property with a range", RangeProperty, "[Header (Values)]\n_Value (\"Value\", Range (0.0, 1.0)) = 0.5");
            yield return ("Property.Number", IntegerProperty, "_Count (\"Count\", Integer) = 2");
            yield return ("Property.Vector", ColorProperty, "_Color (\"Color\", Color) = (1.0, 0.5, 0.0, 1.0)");
            yield return ("Property.Texture", TextureProperty, "_MainTex (\"Texture\", 2D) = \"white\" {}");

            // blocks
            yield return ("HlslInclude", new HlslInclude { hlsl = Hlsl("x") }, "HLSLINCLUDE\nfloat x;\nENDHLSL");
            yield return ("Pass", Pass, "Pass {\n    Name \"Main\"\n    HLSLPROGRAM\n    float y;\n    ENDHLSL\n}");
            yield return ("Shader", new Shader
                {
                    name = Quoted("Test"),
                    materialProperties = new MaterialProperties { properties = new[] { RangeProperty, IntegerProperty, ColorProperty, TextureProperty } },
                    shaderStatements = new ShaderStatement[]
                    {
                        new CustomEditor { editor = Quoted("TestEditor") },
                        new Fallback { name = Quoted("Diffuse") },
                        new SubShader
                        {
                            statements = new SubShaderOrPassStatement[]
                            {
                                Tags,
                                new Lod { level = 100 },
                                new Cull { state = new BackKeyword() },
                                new ZWrite { state = new CalculatedArgument { id = "_ZWrite" } },
                                new HlslInclude { hlsl = Hlsl("x") },
                                Pass,
                            },
                        },
                    },
                },
                string.Join("\n",
                    "Shader \"Test\" {",
                    "    Properties {",
                    "        [Header (Values)]",
                    "        _Value (\"Value\", Range (0.0, 1.0)) = 0.5",
                    "        _Count (\"Count\", Integer) = 2",
                    "        _Color (\"Color\", Color) = (1.0, 0.5, 0.0, 1.0)",
                    "        _MainTex (\"Texture\", 2D) = \"white\" {}",
                    "    }",
                    "    CustomEditor \"TestEditor\"",
                    "    Fallback \"Diffuse\"",
                    "    SubShader {",
                    "        Tags {",
                    "            \"Queue\" = \"Geometry\"",
                    "        }",
                    "        LOD 100",
                    "        Cull Back",
                    "        ZWrite [_ZWrite]",
                    "        HLSLINCLUDE",
                    "        float x;",
                    "        ENDHLSL",
                    "        Pass {",
                    "            Name \"Main\"",
                    "            HLSLPROGRAM",
                    "            float y;",
                    "            ENDHLSL",
                    "        }",
                    "    }",
                    "}"
                ));
        }

        static IEnumerable<TestCaseData> PrintingCases() => Cases().Select(c => new TestCaseData(c.node, c.expected).SetName($"Prints {c.name}"));

        /// All printing cases, for tests of the visitors.
        internal static IEnumerable<(string name, Tree<shaderlab>.Node tree)> Trees => Cases().Select(c => (c.name, c.node));

        [TestCaseSource(nameof(PrintingCases))]
        public static void Prints(Tree<shaderlab>.Node node, string expected) => Assert.AreEqual(expected, ShaderlabFormatter.Format(node)!.ToString());

        [Test]
        public static void EveryNodeTypeHasAPrintingTest() {
            var printed = Trees.SelectMany(c => SyntaxTypes.Of(c.tree)).ToHashSet();

            var missing = SyntaxTypes.All<shaderlab>("me.tooster.sdf.AST.Shaderlab").Where(t => !printed.Contains(t)).Select(t => t.FullName).ToList();

            Assert.IsEmpty(missing, "Add a printing test case for these node types.");
        }

        [TestCase(true, "true")]
        [TestCase(false, "false")]
        public static void BooleanLiteralPrintsLowercase(bool value, string expected) => Assert.AreEqual(expected, ((BooleanLiteral)value).Text);

        [TestCase(0.5f, "0.5")]
        [TestCase(1f, "1.0")]
        [TestCase(1e-7f, "0.0000001")]
        public static void FloatLiteralPrintsADecimalWithoutExponent(float value, string expected) => Assert.AreEqual(expected, ((FloatLiteral)value).Text);
    }
}
