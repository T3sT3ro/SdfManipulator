using System.Text.RegularExpressions;

namespace AST.Hlsl.Syntax {
    // @formatter OFF
    // Tokens
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
    public record EqualsToken                           : HlslToken { public override string Text => "="; }
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
    // compound
    public record BarBarToken                           : HlslToken { public override string Text => "||"; }
    public record AmpersandAmpersandToken               : HlslToken { public override string Text => "&&"; }
    public record MinusMinusToken                       : HlslToken { public override string Text => "--"; }
    public record PlusPlusToken                         : HlslToken { public override string Text => "++"; }
    public record ColonColonToken                       : HlslToken { public override string Text => "::"; }
    public record ExclamationEqualsToken                : HlslToken { public override string Text => "!="; }
    public record EqualsEqualsToken                     : HlslToken { public override string Text => "=="; }
    public record LessThanEqualsToken                   : HlslToken { public override string Text => "<="; }
    public record LessThanLessThanToken                 : HlslToken { public override string Text => "<<"; }
    public record LessThanLessThanEqualsToken           : HlslToken { public override string Text => "<<="; }
    public record GreaterThanEqualsToken                : HlslToken { public override string Text => ">="; }
    public record GreaterThanGreaterThanToken           : HlslToken { public override string Text => ">>"; }
    public record GreaterThanGreaterThanEqualsToken     : HlslToken { public override string Text => ">>="; }
    public record SlashEqualsToken                      : HlslToken { public override string Text => "/="; }
    public record AsteriskEqualsToken                   : HlslToken { public override string Text => "*="; }
    public record BarEqualsToken                        : HlslToken { public override string Text => "|="; }
    public record AmpersandEqualsToken                  : HlslToken { public override string Text => "&="; }
    public record PlusEqualsToken                       : HlslToken { public override string Text => "+="; }
    public record MinusEqualsToken                      : HlslToken { public override string Text => "-="; }
    public record CaretEqualsToken                      : HlslToken { public override string Text => "^="; }
    public record PercentEqualsToken                    : HlslToken { public override string Text => "%="; }
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

    public record VoidKeyword               : HlslToken { public override string Text => "void"; }
    public record BoolKeyword               : HlslToken { public override string Text => "bool"; }
    public record HalfKeyword               : HlslToken { public override string Text => "half"; }
    public record IntKeyword                : HlslToken { public override string Text => "int"; }
    public record UintKeyword               : HlslToken { public override string Text => "uint"; }
    public record FloatKeyword              : HlslToken { public override string Text => "float"; }
    public record DoubleKeyword             : HlslToken { public override string Text => "double"; }
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
    public record BreakKeyword              : HlslToken { public override string Text => "break"; }
    public record ContinueKeyword           : HlslToken { public override string Text => "continue"; }
    public record ReturnKeyword             : HlslToken { public override string Text => "return"; }
    public record DiscardKeyword            : HlslToken { public override string Text => "discard"; }
    public record IfKeyword                 : HlslToken { public override string Text => "if"; }
    public record DoKeyword                 : HlslToken { public override string Text => "do"; }
    public record ElseKeyword               : HlslToken { public override string Text => "else"; }
    public record WhileKeyword              : HlslToken { public override string Text => "while"; }
    public record ForKeyword                : HlslToken { public override string Text => "for"; }

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
    public record BinormalSemantic          : HlslToken { public override string Text => "BINORMAL";}
    public record BlendindicesSemantic      : HlslToken { public override string Text => "BLENDINDICES";}
    public record BlendweightSemantic       : HlslToken { public override string Text => "BLENDWEIGHT";}
    public record ColorSemantic             : HlslToken { public override string Text => "COLOR";}
    public record NormalSemantic            : HlslToken { public override string Text => "NORMAL";}
    public record PositionSemantic          : HlslToken { public override string Text => "POSITION";}
    public record PositiontSemantic         : HlslToken { public override string Text => "POSITIONT";}
    public record PsizeSemantic             : HlslToken { public override string Text => "PSIZE";}
    public record TangentSemantic           : HlslToken { public override string Text => "TANGENT";}
    public record TexcoordSemantic          : HlslToken { public override string Text => "TEXCOORD";}
    public record FogSemantic               : HlslToken { public override string Text => "FOG";}
    public record TessfactorSemantic        : HlslToken { public override string Text => "TESSFACTOR";}
    public record VfaceSemantic             : HlslToken { public override string Text => "VFACE";}
    public record VposSemantic              : HlslToken { public override string Text => "VPOS";}
    public record DepthSemantic             : HlslToken { public override string Text => "DEPTH";}

    // @formatter ON

    public record LineFeedToken : HlslToken {
        public override string Text => "\n";
    }

    public record WhitespaceToken : HlslToken {
        public override string Text { get; set; }
        public static   Regex  WhitespaceRegex = new Regex(@"\s+");
    }

    public record MatrixTypeToken : HlslToken {
        public          HlslToken type { get; internal set; }
        public          uint      rows { get; internal set; }
        public          uint      cols { get; internal set; }
        public override string    Text => $"{type.Text}{rows.ToString()}x{cols.ToString()}";
    }

    public record VectorTypeToken : HlslToken {
        public          HlslToken type  { get; internal set; }
        public          uint      arity { get; internal set; }
        public override string    Text  => $"{type.Text}{arity.ToString()}";
    }

    public record IdentifierToken : HlslToken {
        public static readonly Regex identifierRegex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
    }

    public abstract record LiteralToken : HlslToken {
        public abstract Regex Pattern { get; }    }
    
    public record FloatToken: LiteralToken {
        public override Regex Pattern { get; } = new(@"^((\d*\.\d+|\d+\.\d*)([eE][+-]?\d+)?|\d+([eE][+-]?\d+))[hHfFlL]?$");
    }
    
    public record DecimanToken : LiteralToken {
        public override Regex Pattern { get; } = new(@"^(\d+|0\d+|0x\d+)[uUlL]?$");
    }

    public record BooleanToken : LiteralToken {
        public override Regex Pattern { get; } = new Regex(@"^(true|false)$");
    }
}
