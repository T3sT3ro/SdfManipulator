using System;
using System.Text.RegularExpressions;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax {
// @formatter off
    // TODO: use interned strings or static strings for token getters
    // Tokens
    [TokenNode("\n")] public partial record LineFeedToken;
    [TokenNode("(")] public partial record OpenParenToken;
    [TokenNode(")")] public partial record CloseParenToken;
    [TokenNode("{")] public partial record OpenBraceToken;
    [TokenNode("}")] public partial record CloseBraceToken;
    [TokenNode("[")] public partial record OpenBracketToken;
    [TokenNode("]")] public partial record CloseBracketToken;
    
    [TokenNode("~")] public partial record TildeToken;
    [TokenNode("!")] public partial record NotToken;
    [TokenNode("-")] public partial record MinusToken;
    [TokenNode("+")] public partial record PlusToken;
    [TokenNode("%")] public partial record PercentToken;
    [TokenNode("^")] public partial record CaretToken;
    [TokenNode("&")] public partial record AmpersandToken;
    [TokenNode("*")] public partial record AsteriskToken;
    
    [TokenNode("|")] public partial record BarToken;
    [TokenNode(":")] public partial record ColonToken;
    [TokenNode(";")] public partial record SemicolonToken;
    [TokenNode("<")] public partial record LessThanToken;
    [TokenNode(">")] public partial record GreaterThanToken;
    [TokenNode(",")] public partial record CommaToken;
    [TokenNode(".")] public partial record DotToken;
    [TokenNode("?")] public partial record QuestionToken;
    [TokenNode("#")] public partial record HashToken;
    [TokenNode("/")] public partial record SlashToken;
    
    
    [TokenNode("||")] public partial record BarBarToken;
    [TokenNode("&&")] public partial record AmpersandAmpersandToken;
    
    public abstract partial record AffixOperatorToken : Token<hlsl>;
    [TokenNode("::")] public partial record ColonColonToken;
    [TokenNode("!=")] public partial record ExclamationEqualsToken;
    [TokenNode("==")] public partial record EqualsEqualsToken;
    [TokenNode("<=")] public partial record LessThanEqualsToken;
    [TokenNode("<<")] public partial record LessThanLessThanToken;
    [TokenNode(">=")] public partial record GreaterThanEqualsToken;
    [TokenNode(">>")] public partial record GreaterThanGreaterThanToken;
    
    [TokenNode("--")] public partial record MinusMinusToken : AffixOperatorToken;
    [TokenNode("++")] public partial record PlusPlusToken : AffixOperatorToken;
    
    public abstract partial record AssignmentToken : Token<hlsl>;
    [TokenNode("=")] public partial record EqualsToken : AssignmentToken;
    [TokenNode("<<=")] public partial record LessThanLessThanEqualsToken : AssignmentToken;
    [TokenNode(">>=")] public partial record GreaterThanGreaterThanEqualsToken : AssignmentToken;
    [TokenNode("/=")] public partial record SlashEqualsToken : AssignmentToken;
    [TokenNode("*=")] public partial record AsteriskEqualsToken : AssignmentToken;
    [TokenNode("|=")] public partial record BarEqualsToken : AssignmentToken;
    [TokenNode("&=")] public partial record AmpersandEqualsToken : AssignmentToken;
    [TokenNode("+=")] public partial record PlusEqualsToken : AssignmentToken;
    [TokenNode("-=")] public partial record MinusEqualsToken : AssignmentToken;
    [TokenNode("^=")] public partial record CaretEqualsToken : AssignmentToken;
    [TokenNode("%=")] public partial record PercentEqualsToken : AssignmentToken;
    // Keywords
    [TokenNode("linear")] public partial record LinearKeyword;
    [TokenNode("centroid")] public partial record CentroidKeyword;
    [TokenNode("noperspective")] public partial record NoperspectiveKeyword;
    [TokenNode("sample")] public partial record SampleKeyword;
    [TokenNode("nointerpolation")] public partial record NointerpolationKeyword;

    [TokenNode("const")] public partial record ConstKeyword;
    [TokenNode("row_major")] public partial record RowMajorKeyword;
    [TokenNode("column_major")] public partial record ColumnMajorKeyword;
    
    [TokenNode("extern")] public partial record ExternKeyword;
    [TokenNode("precise")] public partial record PreciseKeyword;
    [TokenNode("shared")] public partial record SharedKeyword;
    [TokenNode("groupshared")] public partial record GroupsharedKeyword;
    [TokenNode("static")] public partial record StaticKeyword;
    [TokenNode("uniform")] public partial record UniformKeyword;
    
    public abstract record PredefinedTypeToken : Token<hlsl>;
    public abstract record ScalarTypeToken : PredefinedTypeToken;
    [TokenNode("void")] public partial record VoidKeyword : PredefinedTypeToken;
    [TokenNode("bool")] public partial record BoolKeyword : ScalarTypeToken;
    [TokenNode("half")] public partial record HalfKeyword : ScalarTypeToken;
    [TokenNode("int")] public partial record IntKeyword : ScalarTypeToken;
    [TokenNode("uint")] public partial record UintKeyword : ScalarTypeToken;
    [TokenNode("float")] public partial record FloatKeyword : ScalarTypeToken;
    [TokenNode("fixed")] public partial record FixedKeyword : ScalarTypeToken;
    [TokenNode("double")] public partial record DoubleKeyword : ScalarTypeToken;
    [TokenNode("struct")] public partial record StructKeyword;

    [TokenNode("Texture1D")] public partial record Texture1DKeyword;
    [TokenNode("Texture1DArray")] public partial record Texture1DArrayKeyword;
    [TokenNode("Texture2D")] public partial record Texture2DKeyword;
    [TokenNode("Texture2DArray")] public partial record Texture2DArrayKeyword;
    [TokenNode("Texture3D")] public partial record Texture3DKeyword;
    [TokenNode("TextureCube")] public partial record TextureCubeKeyword;

    [TokenNode("false")] public partial record FalseKeyword;
    [TokenNode("true")] public partial record TrueKeyword;

    [TokenNode("switch")] public partial record SwitchKeyword;
    [TokenNode("case")] public partial record CaseKeyword;
    [TokenNode("default")] public partial record DefaultKeyword;
    [TokenNode("break")] public partial record BreakKeyword;
    [TokenNode("continue")] public partial record ContinueKeyword;
    [TokenNode("return")] public partial record ReturnKeyword;
    [TokenNode("discard")] public partial record DiscardKeyword;
    [TokenNode("if")] public partial record IfKeyword;
    [TokenNode("do")] public partial record DoKeyword;
    [TokenNode("else")] public partial record ElseKeyword;
    [TokenNode("while")] public partial record WhileKeyword;
    [TokenNode("for")] public partial record ForKeyword;

    [TokenNode("typedef")] public partial record TypedefKeyword;
    [TokenNode("define")] public partial record DefineKeyword;
    [TokenNode("undef")] public partial record UndefKeyword;
    [TokenNode("ifdef")] public partial record IfdefKeyword;
    [TokenNode("ifndef")] public partial record IfndefKeyword;
    [TokenNode("endif")] public partial record EndIfKeyword;
    [TokenNode("elif")] public partial record ElifKeyword;
    [TokenNode("pragma")] public partial record PragmaKeyword;

    [TokenNode("export")] public partial record ExportKeyword;
    [TokenNode("line")] public partial record LineKeyword;
    [TokenNode("error")] public partial record ErrorKeyword;
    
    public abstract record IncludePreprocessorKeyword : Token<hlsl>;
    [TokenNode("include")] public partial record IncludeKeyword : IncludePreprocessorKeyword;
    [TokenNode("include_with_pragmas")] public partial record IncludeWithPragmasKeyword : IncludePreprocessorKeyword;

    [TokenNode("in")] public partial record InKeyword;
    [TokenNode("inout")] public partial record InoutKeyword;
    [TokenNode("out")] public partial record OutKeyword;
    
    // semantics
    
    public abstract record SemanticToken : Token<hlsl>;
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
    
    [TokenNode("SV_CoverageSemantic")] public partial record SvCoverageSemantic : SemanticToken;
    [TokenNode("SV_DepthSemantic")] public partial record SvDepthSemantic : SemanticToken;
    [TokenNode("SV_DepthGreaterEqualSemantic")] public partial record SvDepthGreaterEqualSemantic : SemanticToken;
    [TokenNode("SV_DepthLessEqualSemantic")] public partial record SvDepthLessEqualSemantic : SemanticToken;
    [TokenNode("SV_DispatchThreadIDSemantic")] public partial record SvDispatchThreadIDSemantic : SemanticToken;
    [TokenNode("SV_DomainLocationSemantic")] public partial record SvDomainLocationSemantic : SemanticToken;
    [TokenNode("SV_GroupIDSemantic")] public partial record SvGroupIDSemantic : SemanticToken;
    [TokenNode("SV_GroupIndexSemantic")] public partial record SvGroupIndexSemantic : SemanticToken;
    [TokenNode("SV_GroupThreadIDSemantic")] public partial record SvGroupThreadIDSemantic : SemanticToken;
    [TokenNode("SV_GSInstanceIDSemantic")] public partial record SvGSInstanceIDSemantic : SemanticToken;
    [TokenNode("SV_InnerCoverageSemantic")] public partial record SvInnerCoverageSemantic : SemanticToken;
    [TokenNode("SV_InsideTessFactorSemantic")] public partial record SvInsideTessFactorSemantic : SemanticToken;
    [TokenNode("SV_InstanceIDSemantic")] public partial record SvInstanceIDSemantic : SemanticToken;
    [TokenNode("SV_IsFrontFaceSemantic")] public partial record SvIsFrontFaceSemantic : SemanticToken;
    [TokenNode("SV_OutputControlPointIDSemantic")] public partial record SvOutputControlPointIDSemantic : SemanticToken;
    [TokenNode("SV_PositionSemantic")] public partial record SvPositionSemantic : SemanticToken;
    [TokenNode("SV_PrimitiveIDSemantic")] public partial record SvPrimitiveIDSemantic : SemanticToken;
    [TokenNode("SV_RenderTargetArrayIndexSemantic")] public partial record SvRenderTargetArrayIndexSemantic : SemanticToken;
    [TokenNode("SV_SampleIndexSemantic")] public partial record SvSampleIndexSemantic : SemanticToken;
    [TokenNode("SV_StencilRefSemantic")] public partial record SvStencilRefSemantic : SemanticToken;
    [TokenNode("SV_TessFactorSemantic")] public partial record SvTessFactorSemantic : SemanticToken;
    [TokenNode("SV_VertexIDSemantic")] public partial record SvVertexIDSemantic : SemanticToken;
    [TokenNode("SV_ViewportArrayIndexSemantic")] public partial record SvViewportArrayIndexSemantic : SemanticToken;
    [TokenNode("SV_ShadingRateSemantic")] public partial record SvShadingRateSemantic : SemanticToken;

    // @formatter on

    // {SEMANTIC_NAME}[{n}] e.g. : TEXCOORD1 or POSITION
    public abstract record IndexedSemantic : SemanticToken {
        protected abstract string Name { get; }
        public             uint?  n    { get; init; } // optional for both variants, e.g. PSIZE and PSIZE0
        public override    string Text => string.Intern($"{Name}{n?.ToString() ?? string.Empty}");
    }

    public partial record MatrixToken : PredefinedTypeToken {
        public          ScalarTypeToken type { get; init; } = new FloatKeyword();
        public          uint            rows { get; init; } = 4;
        public          uint            cols { get; init; } = 4;
        public override string          Text => string.Intern($"{type.Text}{rows.ToString()}x{cols.ToString()}");
    }

    public partial record VectorToken : PredefinedTypeToken {
        public          ScalarTypeToken type  { get; init; } = new FloatKeyword();
        public          uint            arity { get; init; } = 4;
        public override string          Text  => string.Intern($"{type.Text}{arity.ToString()}");
    }

    public partial record IdentifierToken : ValidatedToken<hlsl> {
        private static readonly Regex pattern = new(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
        protected override      Regex Pattern => pattern;

        public static implicit operator IdentifierToken(string name) => new() { ValidatedText = name };
    }

    /// <a href="https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-appendix-grammar">grammar</a>
    public abstract record Literal : ValidatedToken<hlsl>;

    public partial record FloatLiteral : Literal {
        private static readonly Regex pattern =
            new(@"^((\d*\.\d+|\d+\.\d*)([eE][+-]?\d+)?|\d+([eE][+-]?\d+))[hHfFlL]?$");

        protected override Regex Pattern => pattern;

        public static implicit operator FloatLiteral(float value) => new() { TextUnsafe = value.ToString("F") };
    }

    public partial record IntLiteral : Literal {
        private static readonly Regex pattern = new(@"^(\d+|0\d+|0x\d+)[uUlL]?$");
        protected override      Regex Pattern => pattern;

        public static implicit operator IntLiteral(int value) => new() { TextUnsafe = value.ToString("D") };
    }

    public partial record BooleanLiteral : Literal {
        private static readonly Regex pattern = new(@"^(true|false)$");
        protected override      Regex Pattern => pattern;

        public static implicit operator BooleanLiteral(bool value) =>
            new() { TextUnsafe = value ? bool.TrueString : bool.FalseString };
    }

    public partial record QuotedStringLiteral : Literal {
        private static readonly Regex pattern = new(@"^""[^""\n\r]*""$");
        protected override      Regex Pattern => pattern;

        public static implicit operator QuotedStringLiteral(string content) =>
            new() { ValidatedText = $"\"{content}\"" };
    }

    // used in preprocessor, can be multiline if there is a "\\n" sequence
    public partial record TokenString : ValidatedToken<hlsl> {
        private static readonly Regex pattern = new(@"^(?:.|\\\n)*$");
        protected override      Regex Pattern => pattern;

        public TokenString() => TextUnsafe = "";
    }
}
