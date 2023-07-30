#nullable enable
using System.Collections.Generic;
using AST.Hlsl;
using AST.Hlsl.Syntax;
using AST.Hlsl.Syntax.Statements;

namespace AST.Shaderlab {
    /// ahttps://docs.unity3d.com/Manual/SL-Shader.html
    public record Shader {
        public IdentifierName          id                 { get; set; }
        public MaterialProperties? materialProperties { get; set; }
        public HlslInclude[]?      hlslIncludes       { get; set; }
        public SubShader[]         subShaders         { get; set; }

        /// <ahref="https://docs.unity3d.com/Manual/SL-Fallback.html">Fallback</a>
        public IdentifierName? fallback { get; set; }

        /// <a href="https://docs.unity3d.com/Manual/SL-CustomEditor.html">CustomEditor</a>
        public CustomEditor? customEditor { get; set; }

        public CustomEditorForRenderPipeline[]? customEditorsForRenderPipelines { get; set; }

        /// <a href="https://docs.unity3d.com/Manual/SL-SubShader.html">SubShader</a>
        public record SubShader {
            /// <a href="https://docs.unity3d.com/Manual/SL-ShaderLOD.html">LOD</a>
            public int? lod { get; set; }

            public Tag[]?         tags         { get; set; }
            public HlslInclude[]? hlslIncludes { get; set; }
            public Pass[]         passes       { get; set; }

            public interface Command : Commands.Command { }

            public interface Tag : Shaderlab.Tag { }

            /// <a href="https://docs.unity3d.com/Manual/SL-Pass.html">Pass</a> 
            public record Pass {
                public IdentifierName?    name         { get; set; }
                public Tag[]?         tags         { get; set; }
                public HlslInclude[]? hlslIncludes { get; set; }
                public HlslProgram?   hlslProgam   { get; set; }

                public interface Command : Commands.Command { }

                public interface Tag : Shaderlab.Tag { }
            }
        }

        public record HlslInclude : Statement {
            public override IReadOnlyList<HlslSyntax>        ChildNodes          { get; }
            public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens { get; }
        }

        public record HlslProgram : Statement {
            public override IReadOnlyList<HlslSyntax>        ChildNodes          { get; }
            public override IReadOnlyList<HlslSyntaxOrToken> ChildNodesAndTokens { get; }
        }


        public record CustomEditor {
            public IdentifierName id { get; set; }
        }

        public record CustomEditorForRenderPipeline {
            public IdentifierName id       { get; set; }
            public IdentifierName pipeline { get; set; }
        }
    }

    /// <a href="https://docs.unity3d.com/Manual/SL-Properties.html">Properties</a>
    public record MaterialProperties {
        public Property[] properties { get; set; }

        public record Property {
            public PropertyAttribute? attribute   { get; set; }
            public IdentifierName         id          { get; set; }
            public string             displayName { get; set; }
            public Type               type        { get; set; }
            public Initializer        initializer { get; set; }


            public record PropertyAttribute {
                public enum Kind {
                    HEADER, SPACE, TOOLTIP,
                    TOGGLE, KEYWORD, KEY_ENUM, ENUM,
                    GAMMA, HDR, HIDE_IN_INSPECTOR, MAIN_TEXTURE, MAIN_COLOR, NO_SCALE_OFFSET, NORMAL,
                    PER_RENDERER_DATA,
                }

                public Kind      kind   { get; set; }
                public string[]? values { get; set; }
            }

            public interface Initializer {
                public record Integer : Initializer {
                    public int value { get; set; }
                }

                public record Float : Initializer {
                    public float value { get; set; }
                }

                public record Vector : Initializer {
                    public float x { get; set; }
                    public float y { get; set; }
                    public float z { get; set; }
                    public float w { get; set; }
                }

                public record Color : Initializer {
                    public float r { get; set; }
                    public float g { get; set; }
                    public float b { get; set; }
                    public float a { get; set; }
                }

                public record Texture : Initializer {
                    public IdentifierName id { get; set; }
                }

                public record CubeTexture : Initializer {
                    public IdentifierName id { get; set; }
                }

                public record Range : Initializer {
                    public float min { get; set; }
                    public float max { get; set; }
                }

                public record IntRange : Initializer {
                    public int min { get; set; }
                    public int max { get; set; }
                }
            }
        }
    }
}
