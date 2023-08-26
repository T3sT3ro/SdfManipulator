using System;
using System.Text.RegularExpressions;

namespace AST.Hlsl.Syntax {
// @formatter OFF
    // TODO: use interned strings or static strings for token getters
    // Tokens
    public record LineFeedToken                         : HlslToken { public override string Text => "\n"; }
    public record OpenParenToken                        : HlslToken { public override string Text => "("; }
    public record CloseParenToken                       : HlslToken { public override string Text => ")"; }
    public record OpenBraceToken                        : HlslToken { public override string Text => "{"; }
    public record CloseBraceToken                       : HlslToken { public override string Text => "}"; }
    public record OpenBracketToken                      : HlslToken { public override string Text => "["; }
    public record CloseBracketToken                     : HlslToken { public override string Text => "]"; }
    
    public record TildeToken                            : HlslToken { public override string Text => "~"; }
    public record NotToken                              : HlslToken { public override string Text => "!"; }
    public record MinusToken                            : HlslToken { public override string Text => "-"; }
    public record PlusToken                             : HlslToken { public override string Text => "+"; }
    public record PercentToken                          : HlslToken { public override string Text => "%"; }
    public record CaretToken                            : HlslToken { public override string Text => "^"; }
    public record AmpersandToken                        : HlslToken { public override string Text => "&"; }
    public record AsteriskToken                         : HlslToken { public override string Text => "*"; }
    
    public record BarToken                              : HlslToken { public override string Text => "|"; }
    public record ColonToken                            : HlslToken { public override string Text => ":"; }
    public record SemiToken                             : HlslToken { public override string Text => ";"; }
    public record LessThanToken                         : HlslToken { public override string Text => "<"; }
    public record GreaterThanToken                      : HlslToken { public override string Text => ">"; }
    public record CommaToken                            : HlslToken { public override string Text => ","; }
    public record DotToken                              : HlslToken { public override string Text => "."; }
    public record QuestionToken                         : HlslToken { public override string Text => "?"; }
    public record HashToken                             : HlslToken { public override string Text => "#"; }
    public record SlashToken                            : HlslToken { public override string Text => "/"; }
    
    
    public record BarBarToken                           : HlslToken { public override string Text => "||"; }
    public record AmpersandAmpersandToken               : HlslToken { public override string Text => "&&"; }
    public abstract record AffixOperatorToken           : HlslToken;
    public record ColonColonToken                       : HlslToken { public override string Text => "::"; }
    public record ExclamationEqualsToken                : HlslToken { public override string Text => "!="; }
    public record EqualsEqualsToken                     : HlslToken { public override string Text => "=="; }
    public record LessThanEqualsToken                   : HlslToken { public override string Text => "<="; }
    public record LessThanLessThanToken                 : HlslToken { public override string Text => "<<"; }
    public record GreaterThanEqualsToken                : HlslToken { public override string Text => ">="; }
    public record GreaterThanGreaterThanToken           : HlslToken { public override string Text => ">>"; }
    
    public record MinusMinusToken                       : AffixOperatorToken { public override string Text => "--"; }
    public record PlusPlusToken                         : AffixOperatorToken { public override string Text => "++"; }
    
    public abstract record AssignmentToken : HlslToken;
    public record EqualsToken                           : AssignmentToken { public override string Text => "="; }
    public record LessThanLessThanEqualsToken           : AssignmentToken { public override string Text => "<<="; }
    public record GreaterThanGreaterThanEqualsToken     : AssignmentToken { public override string Text => ">>="; }
    public record SlashEqualsToken                      : AssignmentToken { public override string Text => "/="; }
    public record AsteriskEqualsToken                   : AssignmentToken { public override string Text => "*="; }
    public record BarEqualsToken                        : AssignmentToken { public override string Text => "|="; }
    public record AmpersandEqualsToken                  : AssignmentToken { public override string Text => "&="; }
    public record PlusEqualsToken                       : AssignmentToken { public override string Text => "+="; }
    public record MinusEqualsToken                      : AssignmentToken { public override string Text => "-="; }
    public record CaretEqualsToken                      : AssignmentToken { public override string Text => "^="; }
    public record PercentEqualsToken                    : AssignmentToken { public override string Text => "%="; }
    // Keywords
    public record LinearKeyword             : HlslToken { public override string Text => "linear"; }
    public record CentroidKeyword           : HlslToken { public override string Text => "centroid"; }
    public record NoperspectiveKeyword      : HlslToken { public override string Text => "noperspective"; }
    public record SampleKeyword             : HlslToken { public override string Text => "sample"; }
    public record NointerpolationKeyword    : HlslToken { public override string Text => "nointerpolation"; }

    public record ConstKeyword              : HlslToken { public override string Text => "const"; }
    public record RowMajorKeyword           : HlslToken { public override string Text => "row_major"; }
    public record ColumnMajorKeyword        : HlslToken { public override string Text => "column_major"; }
    
    public record ExternKeyword             : HlslToken { public override string Text => "extern"; }
    public record PreciseKeyword            : HlslToken { public override string Text => "precise"; }
    public record SharedKeyword             : HlslToken { public override string Text => "shared"; }
    public record GroupsharedKeyword        : HlslToken { public override string Text => "groupshared"; }
    public record StaticKeyword             : HlslToken { public override string Text => "static"; }
    public record UniformKeyword            : HlslToken { public override string Text => "uniform"; }
    
    public abstract record PredefinedTypeToken : HlslToken;
    public abstract record ScalarTypeToken : PredefinedTypeToken;
    public record VoidKeyword               : PredefinedTypeToken { public override string Text => "void"; }
    public record BoolKeyword               : ScalarTypeToken { public override string Text => "bool"; }
    public record HalfKeyword               : ScalarTypeToken { public override string Text => "half"; }
    public record IntKeyword                : ScalarTypeToken { public override string Text => "int"; }
    public record UintKeyword               : ScalarTypeToken { public override string Text => "uint"; }
    public record FloatKeyword              : ScalarTypeToken { public override string Text => "float"; }
    public record FixedKeyword              : ScalarTypeToken { public override string Text => "fixed"; }
    public record DoubleKeyword             : ScalarTypeToken { public override string Text => "double"; }
    public record StructKeyword             : HlslToken { public override string Text => "struct"; }

    public record Texture1DKeyword          : HlslToken { public override string Text => "Texture1D"; }
    public record Texture1DArrayKeyword     : HlslToken { public override string Text => "Texture1DArray"; }
    public record Texture2DKeyword          : HlslToken { public override string Text => "Texture2D"; }
    public record Texture2DArrayKeyword     : HlslToken { public override string Text => "Texture2DArray"; }
    public record Texture3DKeyword          : HlslToken { public override string Text => "Texture3D"; }
    public record TextureCubeKeyword        : HlslToken { public override string Text => "TextureCube"; }

    public record FalseKeyword              : HlslToken { public override string Text => "false"; }
    public record TrueKeyword               : HlslToken { public override string Text => "true"; }

    public record SwitchKeyword             : HlslToken { public override string Text => "switch"; }
    public record CaseKeyword               : HlslToken { public override string Text => "case"; }
    public record DefaultKeyword            : HlslToken { public override string Text => "default"; }
    public record BreakKeyword              : HlslToken { public override string Text => "break"; }
    public record ContinueKeyword           : HlslToken { public override string Text => "continue"; }
    public record ReturnKeyword             : HlslToken { public override string Text => "return"; }
    public record DiscardKeyword            : HlslToken { public override string Text => "discard"; }
    public record IfKeyword                 : HlslToken { public override string Text => "if"; }
    public record DoKeyword                 : HlslToken { public override string Text => "do"; }
    public record ElseKeyword               : HlslToken { public override string Text => "else"; }
    public record WhileKeyword              : HlslToken { public override string Text => "while"; }
    public record ForKeyword                : HlslToken { public override string Text => "for"; }

    public record TypedefKeyword            : HlslToken { public override string Text => "typedef"; }
    public record DefineKeyword             : HlslToken { public override string Text => "define"; }
    public record UndefKeyword              : HlslToken { public override string Text => "undef"; }
    public record IfdefKeyword              : HlslToken { public override string Text => "ifdef"; }
    public record IfndefKeyword             : HlslToken { public override string Text => "ifndef"; }
    public record EndIfKeyword              : HlslToken { public override string Text => "endif"; }
    public record ElifKeyword               : HlslToken { public override string Text => "elif"; }
    public record PragmaKeyword             : HlslToken { public override string Text => "pragma"; }

    public record ExportKeyword             : HlslToken { public override string Text => "export"; }
    public record IncludeKeyword            : HlslToken { public override string Text => "include"; }
    public record LineKeyword               : HlslToken { public override string Text => "line"; }

    public record InKeyword                 : HlslToken { public override string Text => "in"; }
    public record InoutKeyword              : HlslToken { public override string Text => "inout"; }
    public record OutKeyword                : HlslToken { public override string Text => "out"; }
    
    // semantics
    
    public abstract record SemanticToken : HlslToken;
    //   legacy
    public record PositiontSemantic         : SemanticToken { public override string Text => "POSITIONT";}
    public record FogSemantic               : SemanticToken { public override string Text => "FOG";}
    public record BinormalSemantic          : IndexedSemantic { protected override string Name => "BINORMAL";}
    public record BlendindicesSemantic      : IndexedSemantic { protected override string Name => "BLENDINDICES";}
    public record BlendweightSemantic       : IndexedSemantic { protected override string Name => "BLENDWEIGHT";}
    public record ColorSemantic             : IndexedSemantic { protected override string Name => "COLOR";}
    public record NormalSemantic            : IndexedSemantic { protected override string Name => "NORMAL";}
    public record PositionSemantic          : IndexedSemantic { protected override string Name => "POSITION";}
    public record PsizeSemantic             : IndexedSemantic { protected override string Name => "PSIZE";}
    public record TangentSemantic           : IndexedSemantic { protected override string Name => "TANGENT";}
    public record TexcoordSemantic          : IndexedSemantic { protected override string Name => "TEXCOORD";}
    public record TessfactorSemantic        : IndexedSemantic { protected override string Name => "TESSFACTOR";}
    public record VfaceSemantic             : IndexedSemantic { protected override string Name => "VFACE";}
    public record VposSemantic              : IndexedSemantic { protected override string Name => "VPOS";}
    public record DepthSemantic             : IndexedSemantic { protected override string Name => "DEPTH";}
    
    //   System Value Semantics
    public record SvClipDistanceSemantic    : IndexedSemantic { protected override string Name => "SV_ClipDistanceSemantic"; }
    public record SvCullDistanceSemantic    : IndexedSemantic { protected override string Name => "SV_CullDistanceSemantic"; }
    // where 0 <= n <= 7
    public record SvTargetSemantic          : IndexedSemantic { protected override string Name => "SV_TargetSemantic"; } 
    
    public record SvCoverageSemantic               : SemanticToken { public override string Text => "SV_CoverageSemantic"; } 
    public record SvDepthSemantic                  : SemanticToken { public override string Text => "SV_DepthSemantic"; } 
    public record SvDepthGreaterEqualSemantic      : SemanticToken { public override string Text => "SV_DepthGreaterEqualSemantic"; } 
    public record SvDepthLessEqualSemantic         : SemanticToken { public override string Text => "SV_DepthLessEqualSemantic"; } 
    public record SvDispatchThreadIDSemantic       : SemanticToken { public override string Text => "SV_DispatchThreadIDSemantic"; } 
    public record SvDomainLocationSemantic         : SemanticToken { public override string Text => "SV_DomainLocationSemantic"; } 
    public record SvGroupIDSemantic                : SemanticToken { public override string Text => "SV_GroupIDSemantic"; } 
    public record SvGroupIndexSemantic             : SemanticToken { public override string Text => "SV_GroupIndexSemantic"; } 
    public record SvGroupThreadIDSemantic          : SemanticToken { public override string Text => "SV_GroupThreadIDSemantic"; } 
    public record SvGSInstanceIDSemantic           : SemanticToken { public override string Text => "SV_GSInstanceIDSemantic"; } 
    public record SvInnerCoverageSemantic          : SemanticToken { public override string Text => "SV_InnerCoverageSemantic"; } 
    public record SvInsideTessFactorSemantic       : SemanticToken { public override string Text => "SV_InsideTessFactorSemantic"; } 
    public record SvInstanceIDSemantic             : SemanticToken { public override string Text => "SV_InstanceIDSemantic"; } 
    public record SvIsFrontFaceSemantic            : SemanticToken { public override string Text => "SV_IsFrontFaceSemantic"; } 
    public record SvOutputControlPointIDSemantic   : SemanticToken { public override string Text => "SV_OutputControlPointIDSemantic"; } 
    public record SvPositionSemantic               : SemanticToken { public override string Text => "SV_PositionSemantic"; } 
    public record SvPrimitiveIDSemantic            : SemanticToken { public override string Text => "SV_PrimitiveIDSemantic"; } 
    public record SvRenderTargetArrayIndexSemantic : SemanticToken { public override string Text => "SV_RenderTargetArrayIndexSemantic"; } 
    public record SvSampleIndexSemantic            : SemanticToken { public override string Text => "SV_SampleIndexSemantic"; } 
    public record SvStencilRefSemantic             : SemanticToken { public override string Text => "SV_StencilRefSemantic"; } 
    public record SvTessFactorSemantic             : SemanticToken { public override string Text => "SV_TessFactorSemantic"; } 
    public record SvVertexIDSemantic               : SemanticToken { public override string Text => "SV_VertexIDSemantic"; } 
    public record SvViewportArrayIndexSemantic     : SemanticToken { public override string Text => "SV_ViewportArrayIndexSemantic"; } 
    public record SvShadingRateSemantic            : SemanticToken { public override string Text => "SV_ShadingRateSemantic"; } 

    // @formatter ON

    // {SEMANTIC_NAME}[{n}] e.g. : TEXCOORD1 or POSITION
    public abstract record IndexedSemantic : SemanticToken {
        protected abstract string Name { get; }
        public             uint?  n    { get; init; } // optional for both variants, e.g. PSIZE and PSIZE0
        public override    string Text => String.Intern($"{Name}{n?.ToString() ?? String.Empty}");
    }

    public record MatrixToken : PredefinedTypeToken {
        public          ScalarTypeToken type { get; init; }
        public          uint            rows { get; init; }
        public          uint            cols { get; init; }
        public override string          Text => String.Intern($"{type.Text}{rows.ToString()}x{cols.ToString()}");
    }

    public record VectorToken : PredefinedTypeToken {
        public          ScalarTypeToken type  { get; init; }
        public          uint            arity { get; init; }
        public override string          Text  => String.Intern($"{type.Text}{arity.ToString()}");
    }

    public abstract record ValidatedHlslToken : HlslToken {
        private readonly string validatedText;

        public override string Text => validatedText;

        protected abstract Regex Pattern { get; }

        /// <summary>Creates new token by validating the text.</summary>
        /// <exception cref="ArgumentException">if text doesn't match pattern</exception>
        public virtual string ValidatedText {
            init {
                if (!Pattern.IsMatch(value))
                    throw new ArgumentException($"Token text: {value} doesn't match pattern: {Pattern}");

                validatedText = value;
            }
        }

        protected string TextUnsafe {
            init => validatedText = value;
        }
    }

    public record WhitespaceToken : ValidatedHlslToken {
        private static readonly Regex pattern = new(@"\s+");
        protected override      Regex Pattern => pattern;
    }

    public record IdentifierToken : ValidatedHlslToken {
        private static readonly Regex pattern = new(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
        protected override      Regex Pattern => pattern;

        public static implicit operator IdentifierToken(string name) => new() { ValidatedText = name };
    }

    /// <a href="https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-appendix-grammar">grammar</a>
    public abstract record Literal : ValidatedHlslToken;

    public record FloatLiteral : Literal {
        private static readonly Regex pattern =
            new(@"^((\d*\.\d+|\d+\.\d*)([eE][+-]?\d+)?|\d+([eE][+-]?\d+))[hHfFlL]?$");

        protected override Regex Pattern => pattern;

        public static implicit operator FloatLiteral(float value) => new() { TextUnsafe = value.ToString("F") };
    }

    public record IntLiteral : Literal {
        private static readonly Regex pattern = new(@"^(\d+|0\d+|0x\d+)[uUlL]?$");
        protected override      Regex Pattern => pattern;

        public static implicit operator IntLiteral(int value) => new() { TextUnsafe = value.ToString("D") };
    }

    public record BooleanLiteral : Literal {
        private static readonly Regex pattern = new(@"^(true|false)$");
        protected override      Regex Pattern => pattern;

        public static implicit operator BooleanLiteral(bool value) =>
            new() { TextUnsafe = value ? bool.TrueString : bool.FalseString };
    }
}
