using System;
using System.Text.RegularExpressions;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
// @formatter off
    // TODO: use interned strings or static strings for token getters
    // Tokens
    public record LineFeedToken                         : Token<Hlsl> { public override string Text => "\n"; }
    public record OpenParenToken                        : Token<Hlsl> { public override string Text => "("; }
    public record CloseParenToken                       : Token<Hlsl> { public override string Text => ")"; }
    public record OpenBraceToken                        : Token<Hlsl> { public override string Text => "{"; }
    public record CloseBraceToken                       : Token<Hlsl> { public override string Text => "}"; }
    public record OpenBracketToken                      : Token<Hlsl> { public override string Text => "["; }
    public record CloseBracketToken                     : Token<Hlsl> { public override string Text => "]"; }
    
    public record TildeToken                            : Token<Hlsl> { public override string Text => "~"; }
    public record NotToken                              : Token<Hlsl> { public override string Text => "!"; }
    public record MinusToken                            : Token<Hlsl> { public override string Text => "-"; }
    public record PlusToken                             : Token<Hlsl> { public override string Text => "+"; }
    public record PercentToken                          : Token<Hlsl> { public override string Text => "%"; }
    public record CaretToken                            : Token<Hlsl> { public override string Text => "^"; }
    public record AmpersandToken                        : Token<Hlsl> { public override string Text => "&"; }
    public record AsteriskToken                         : Token<Hlsl> { public override string Text => "*"; }
    
    public record BarToken                              : Token<Hlsl> { public override string Text => "|"; }
    public record ColonToken                            : Token<Hlsl> { public override string Text => ":"; }
    public record SemicolonToken                        : Token<Hlsl> { public override string Text => ";"; }
    public record LessThanToken                         : Token<Hlsl> { public override string Text => "<"; }
    public record GreaterThanToken                      : Token<Hlsl> { public override string Text => ">"; }
    public record CommaToken                            : Token<Hlsl> { public override string Text => ","; }
    public record DotToken                              : Token<Hlsl> { public override string Text => "."; }
    public record QuestionToken                         : Token<Hlsl> { public override string Text => "?"; }
    public record HashToken                             : Token<Hlsl> { public override string Text => "#"; }
    public record SlashToken                            : Token<Hlsl> { public override string Text => "/"; }
    
    
    public record BarBarToken                           : Token<Hlsl> { public override string Text => "||"; }
    public record AmpersandAmpersandToken               : Token<Hlsl> { public override string Text => "&&"; }
    public abstract record AffixOperatorToken           : Token<Hlsl>;
    public record ColonColonToken                       : Token<Hlsl> { public override string Text => "::"; }
    public record ExclamationEqualsToken                : Token<Hlsl> { public override string Text => "!="; }
    public record EqualsEqualsToken                     : Token<Hlsl> { public override string Text => "=="; }
    public record LessThanEqualsToken                   : Token<Hlsl> { public override string Text => "<="; }
    public record LessThanLessThanToken                 : Token<Hlsl> { public override string Text => "<<"; }
    public record GreaterThanEqualsToken                : Token<Hlsl> { public override string Text => ">="; }
    public record GreaterThanGreaterThanToken           : Token<Hlsl> { public override string Text => ">>"; }
    
    public record MinusMinusToken                       : AffixOperatorToken { public override string Text => "--"; }
    public record PlusPlusToken                         : AffixOperatorToken { public override string Text => "++"; }
    
    public abstract record AssignmentToken : Token<Hlsl>;
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
    public record LinearKeyword             : Token<Hlsl> { public override string Text => "linear"; }
    public record CentroidKeyword           : Token<Hlsl> { public override string Text => "centroid"; }
    public record NoperspectiveKeyword      : Token<Hlsl> { public override string Text => "noperspective"; }
    public record SampleKeyword             : Token<Hlsl> { public override string Text => "sample"; }
    public record NointerpolationKeyword    : Token<Hlsl> { public override string Text => "nointerpolation"; }

    public record ConstKeyword              : Token<Hlsl> { public override string Text => "const"; }
    public record RowMajorKeyword           : Token<Hlsl> { public override string Text => "row_major"; }
    public record ColumnMajorKeyword        : Token<Hlsl> { public override string Text => "column_major"; }
    
    public record ExternKeyword             : Token<Hlsl> { public override string Text => "extern"; }
    public record PreciseKeyword            : Token<Hlsl> { public override string Text => "precise"; }
    public record SharedKeyword             : Token<Hlsl> { public override string Text => "shared"; }
    public record GroupsharedKeyword        : Token<Hlsl> { public override string Text => "groupshared"; }
    public record StaticKeyword             : Token<Hlsl> { public override string Text => "static"; }
    public record UniformKeyword            : Token<Hlsl> { public override string Text => "uniform"; }
    
    public abstract record PredefinedTypeToken : Token<Hlsl>;
    public abstract record ScalarTypeToken : PredefinedTypeToken;
    public record VoidKeyword               : PredefinedTypeToken { public override string Text => "void"; }
    public record BoolKeyword               : ScalarTypeToken { public override string Text => "bool"; }
    public record HalfKeyword               : ScalarTypeToken { public override string Text => "half"; }
    public record IntKeyword                : ScalarTypeToken { public override string Text => "int"; }
    public record UintKeyword               : ScalarTypeToken { public override string Text => "uint"; }
    public record FloatKeyword              : ScalarTypeToken { public override string Text => "float"; }
    public record FixedKeyword              : ScalarTypeToken { public override string Text => "fixed"; }
    public record DoubleKeyword             : ScalarTypeToken { public override string Text => "double"; }
    public record StructKeyword             : Token<Hlsl> { public override string Text => "struct"; }

    public record Texture1DKeyword          : Token<Hlsl> { public override string Text => "Texture1D"; }
    public record Texture1DArrayKeyword     : Token<Hlsl> { public override string Text => "Texture1DArray"; }
    public record Texture2DKeyword          : Token<Hlsl> { public override string Text => "Texture2D"; }
    public record Texture2DArrayKeyword     : Token<Hlsl> { public override string Text => "Texture2DArray"; }
    public record Texture3DKeyword          : Token<Hlsl> { public override string Text => "Texture3D"; }
    public record TextureCubeKeyword        : Token<Hlsl> { public override string Text => "TextureCube"; }

    public record FalseKeyword              : Token<Hlsl> { public override string Text => "false"; }
    public record TrueKeyword               : Token<Hlsl> { public override string Text => "true"; }

    public record SwitchKeyword             : Token<Hlsl> { public override string Text => "switch"; }
    public record CaseKeyword               : Token<Hlsl> { public override string Text => "case"; }
    public record DefaultKeyword            : Token<Hlsl> { public override string Text => "default"; }
    public record BreakKeyword              : Token<Hlsl> { public override string Text => "break"; }
    public record ContinueKeyword           : Token<Hlsl> { public override string Text => "continue"; }
    public record ReturnKeyword             : Token<Hlsl> { public override string Text => "return"; }
    public record DiscardKeyword            : Token<Hlsl> { public override string Text => "discard"; }
    public record IfKeyword                 : Token<Hlsl> { public override string Text => "if"; }
    public record DoKeyword                 : Token<Hlsl> { public override string Text => "do"; }
    public record ElseKeyword               : Token<Hlsl> { public override string Text => "else"; }
    public record WhileKeyword              : Token<Hlsl> { public override string Text => "while"; }
    public record ForKeyword                : Token<Hlsl> { public override string Text => "for"; }

    public record TypedefKeyword            : Token<Hlsl> { public override string Text => "typedef"; }
    public record DefineKeyword             : Token<Hlsl> { public override string Text => "define"; }
    public record UndefKeyword              : Token<Hlsl> { public override string Text => "undef"; }
    public record IfdefKeyword              : Token<Hlsl> { public override string Text => "ifdef"; }
    public record IfndefKeyword             : Token<Hlsl> { public override string Text => "ifndef"; }
    public record EndIfKeyword              : Token<Hlsl> { public override string Text => "endif"; }
    public record ElifKeyword               : Token<Hlsl> { public override string Text => "elif"; }
    public record PragmaKeyword             : Token<Hlsl> { public override string Text => "pragma"; }

    public record ExportKeyword             : Token<Hlsl> { public override string Text => "export"; }
    public record LineKeyword               : Token<Hlsl> { public override string Text => "line"; }
    public record ErrorKeyword              : Token<Hlsl> { public override string Text => "error"; }
    public abstract record IncludePreprocessorKeyword : Token <Hlsl>;
    public record IncludeKeyword            : IncludePreprocessorKeyword { public override string Text => "include"; }
    public record IncludeWithPragmasKeyword : IncludePreprocessorKeyword { public override string Text => "include_with_pragmas"; }

    public record InKeyword                 : Token<Hlsl> { public override string Text => "in"; }
    public record InoutKeyword              : Token<Hlsl> { public override string Text => "inout"; }
    public record OutKeyword                : Token<Hlsl> { public override string Text => "out"; }
    
    // semantics
    
    public abstract record SemanticToken : Token<Hlsl>;
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

    // @formatter on

    // {SEMANTIC_NAME}[{n}] e.g. : TEXCOORD1 or POSITION
    public abstract record IndexedSemantic : SemanticToken {
        protected abstract string Name { get; }
        public             uint?  n    { get; init; } // optional for both variants, e.g. PSIZE and PSIZE0
        public override    string Text => string.Intern($"{Name}{n?.ToString() ?? string.Empty}");
    }

    public record MatrixToken : PredefinedTypeToken {
        public          ScalarTypeToken type { get; init; } = new FloatKeyword();
        public          uint            rows { get; init; } = 4;
        public          uint            cols { get; init; } = 4;
        public override string          Text => string.Intern($"{type.Text}{rows.ToString()}x{cols.ToString()}");
    }

    public record VectorToken : PredefinedTypeToken {
        public          ScalarTypeToken type  { get; init; } = new FloatKeyword();
        public          uint            arity { get; init; } = 4;
        public override string          Text  => string.Intern($"{type.Text}{arity.ToString()}");
    }

    public record IdentifierToken : ValidatedToken<Hlsl> {
        private static readonly Regex pattern = new(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
        protected override      Regex Pattern => pattern;

        public static implicit operator IdentifierToken(string name) => new() { ValidatedText = name };
    }

    /// <a href="https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-appendix-grammar">grammar</a>
    public abstract record Literal : ValidatedToken<Hlsl>;

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

    public record QuotedStringLiteral : Literal {
        private static readonly Regex pattern = new(@"^""[^""\n\r]*""$");
        protected override      Regex Pattern => pattern;

        public static implicit operator QuotedStringLiteral(string content) =>
            new() { ValidatedText = $"\"{content}\"" };
    }

    // used in preprocessor, can be multiline if there is a "\\n" sequence
    public record TokenString : ValidatedToken<Hlsl> {
        private static readonly Regex pattern = new(@"^(?:.|\\\n)*$");
        protected override      Regex Pattern => pattern;

        public TokenString() { TextUnsafe = ""; }
    }
}
