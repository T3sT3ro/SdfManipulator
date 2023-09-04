using System;
using System.Text.RegularExpressions;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    // @formatter off
    // Keywords
    public record LineFeedToken                         : Token<Shaderlab> { public override string Text => "\n"; }
    
    public record OpenParenToken                        : Token<Shaderlab> { public override string Text => "("; }
    public record CloseParenToken                       : Token<Shaderlab> { public override string Text => ")"; }
    public record OpenBraceToken                        : Token<Shaderlab> { public override string Text => "{"; }
    public record CloseBraceToken                       : Token<Shaderlab> { public override string Text => "}"; }
    public record OpenBracketToken                      : Token<Shaderlab> { public override string Text => "["; }
    public record CloseBracketToken                     : Token<Shaderlab> { public override string Text => "]"; }
    
    public record TildeToken                            : Token<Shaderlab> { public override string Text => "~"; }
    public record NotToken                              : Token<Shaderlab> { public override string Text => "!"; }
    public record MinusToken                            : Token<Shaderlab> { public override string Text => "-"; }
    public record PlusToken                             : Token<Shaderlab> { public override string Text => "+"; }
    public record PercentToken                          : Token<Shaderlab> { public override string Text => "%"; }
    public record CaretToken                            : Token<Shaderlab> { public override string Text => "^"; }
    public record AmpersandToken                        : Token<Shaderlab> { public override string Text => "&"; }
    public record AsteriskToken                         : Token<Shaderlab> { public override string Text => "*"; }
    public record EqualsToken                           : Token<Shaderlab> { public override string Text => "="; }
    public record BarToken                              : Token<Shaderlab> { public override string Text => "|"; }
    public record ColonToken                            : Token<Shaderlab> { public override string Text => ":"; }
    public record SemiToken                             : Token<Shaderlab> { public override string Text => ";"; }
    public record LessThanToken                         : Token<Shaderlab> { public override string Text => "<"; }
    public record GreaterThanToken                      : Token<Shaderlab> { public override string Text => ">"; }
    public record CommaToken                            : Token<Shaderlab> { public override string Text => ","; }
    public record DotToken                              : Token<Shaderlab> { public override string Text => "."; }
    public record QuestionToken                         : Token<Shaderlab> { public override string Text => "?"; }
    public record HashToken                             : Token<Shaderlab> { public override string Text => "#"; }
    public record SlashToken                            : Token<Shaderlab> { public override string Text => "/"; }
    // compound
    public record BarBarToken                           : Token<Shaderlab> { public override string Text => "||"; }
    public record AmpersandAmpersandToken               : Token<Shaderlab> { public override string Text => "&&"; }
    public record MinusMinusToken                       : Token<Shaderlab> { public override string Text => "--"; }
    public record PlusPlusToken                         : Token<Shaderlab> { public override string Text => "++"; }
    public record ColonColonToken                       : Token<Shaderlab> { public override string Text => "::"; }
    public record ExclamationEqualsToken                : Token<Shaderlab> { public override string Text => "!="; }
    public record EqualsEqualsToken                     : Token<Shaderlab> { public override string Text => "=="; }
    public record LessThanEqualsToken                   : Token<Shaderlab> { public override string Text => "<="; }
    public record LessThanLessThanToken                 : Token<Shaderlab> { public override string Text => "<<"; }
    public record LessThanLessThanEqualsToken           : Token<Shaderlab> { public override string Text => "<<="; }
    public record GreaterThanEqualsToken                : Token<Shaderlab> { public override string Text => ">="; }
    public record GreaterThanGreaterThanToken           : Token<Shaderlab> { public override string Text => ">>"; }
    public record GreaterThanGreaterThanEqualsToken     : Token<Shaderlab> { public override string Text => ">>="; }
    public record SlashEqualsToken                      : Token<Shaderlab> { public override string Text => "/="; }
    public record AsteriskEqualsToken                   : Token<Shaderlab> { public override string Text => "*="; }
    public record BarEqualsToken                        : Token<Shaderlab> { public override string Text => "|="; }
    public record AmpersandEqualsToken                  : Token<Shaderlab> { public override string Text => "&="; }
    public record PlusEqualsToken                       : Token<Shaderlab> { public override string Text => "+="; }
    public record MinusEqualsToken                      : Token<Shaderlab> { public override string Text => "-="; }
    public record CaretEqualsToken                      : Token<Shaderlab> { public override string Text => "^="; }
    public record PercentEqualsToken                    : Token<Shaderlab> { public override string Text => "%="; }
    
    // keywords
    public record ShaderKeyword                         : Token<Shaderlab> { public override string Text => "Shader"; }
    public record SubShaderKeyword                      : Token<Shaderlab> { public override string Text => "SubShader"; }
    public record PassKeyword                           : Token<Shaderlab> { public override string Text => "Pass"; }
    public record HlslIncludeKeyword                    : Token<Shaderlab> { public override string Text => "HLSLINCLUDE"; }
    public record HlslProgramKeyword                    : Token<Shaderlab> { public override string Text => "HLSLPROGRAM"; }
    public record EndHlslKeyword                        : Token<Shaderlab> { public override string Text => "ENDHLSL"; }
    public record FallbackKeyword                       : Token<Shaderlab> { public override string Text => "Fallback"; }
    public record CustomEditorKeyword                   : Token<Shaderlab> { public override string Text => "CustomEditor"; }
    public record CustomEditorForRenderPipelineKeyword  : Token<Shaderlab> { public override string Text => "CustomEditorForRenderPipeline"; }
    public record PropertiesKeyword                     : Token<Shaderlab> { public override string Text => "Properties"; }
    public record TagsKeyword                           : Token<Shaderlab> { public override string Text => "Tags"; }
    public record OnKeyword                             : Token<Shaderlab> { public override string Text => "On"; }
    public record TrueKeyword                           : Token<Shaderlab> { public override string Text => "True"; }
    public record FalseKeyword                          : Token<Shaderlab> { public override string Text => "False"; }
    public record LodKeyword                            : Token<Shaderlab> { public override string Text => "LOD"; }
    public record NameKeyword                           : Token<Shaderlab> { public override string Text => "Name"; }
    
    // commands
    public record AlphaToMaskKeyword                    : Token<Shaderlab> { public override string Text => "AlphaToMask"; }
    public record BlendKeyword                          : Token<Shaderlab> { public override string Text => "Blend"; }
    public record BlendOpKeyword                        : Token<Shaderlab> { public override string Text => "BlendOp"; }
    public record ColorMaskKeyword                      : Token<Shaderlab> { public override string Text => "ColorMask"; }
    public record ConservativeKeyword                   : Token<Shaderlab> { public override string Text => "Conservative"; }
    public record CullKeyword                           : Token<Shaderlab> { public override string Text => "Cull"; }
    public record OffsetKeyword                         : Token<Shaderlab> { public override string Text => "Offset"; }
    public record StencilKeyword                        : Token<Shaderlab> { public override string Text => "Stencil"; }
    public record ZClipKeyword                          : Token<Shaderlab> { public override string Text => "ZClip"; }
    public record ZTestKeyword                          : Token<Shaderlab> { public override string Text => "ZTest"; }
    public record ZWriteKeyword                         : Token<Shaderlab> { public override string Text => "ZWrite"; }
    public record UsePassKeyword                        : Token<Shaderlab> { public override string Text => "UsePass"; }
    public record GrabPassKeyword                       : Token<Shaderlab> { public override string Text => "GrabPass"; }
    // CommandARguments
    public abstract record ArgumentKeyword              : Token<Shaderlab>;
    // Blend
    public record OffKeyword                            : ArgumentKeyword { public override string Text => "Off"; }
    public record OneKeyword                            : ArgumentKeyword { public override string Text => "One"; }
    public record ZeroKeyword                           : ArgumentKeyword { public override string Text => "Zero"; }
    public record SrcColorKeyword                       : ArgumentKeyword { public override string Text => "SrcColor"; }
    public record SrcAlphaKeyword                       : ArgumentKeyword { public override string Text => "SrcAlpha"; }
    public record SrcAlphaSaturateKeyword               : ArgumentKeyword { public override string Text => "SrcAlphaSaturate"; }
    public record DstColorKeyword                       : ArgumentKeyword { public override string Text => "DstColor"; }
    public record DstAlphaKeyword                       : ArgumentKeyword { public override string Text => "DstAlpha"; }
    public record OneMinusSrcColorKeyword               : ArgumentKeyword { public override string Text => "OneMinusSrcColor"; }
    public record OneMinusSrcAlphaKeyword               : ArgumentKeyword { public override string Text => "OneMinusSrcAlpha"; }
    public record OneMinusDstColorKeyword               : ArgumentKeyword { public override string Text => "OneMinusDstColor"; }
    public record OneMinusDstAlphaKeyword               : ArgumentKeyword { public override string Text => "OneMinusDstAlpha"; }
    // BlendOp
    public record AddKeyword                            : ArgumentKeyword { public override string Text => "Add"; }
    public record SubKeyword                            : ArgumentKeyword { public override string Text => "Sub"; }
    public record RevSubKeyword                         : ArgumentKeyword { public override string Text => "RevSub"; }
    public record MinKeyword                            : ArgumentKeyword { public override string Text => "Min"; }
    public record MaxKeyword                            : ArgumentKeyword { public override string Text => "Max"; }
    public record LogicalClearKeyword                   : ArgumentKeyword { public override string Text => "LogicalClear"; }
    public record LogicalSetKeyword                     : ArgumentKeyword { public override string Text => "LogicalSet"; }
    public record LogicalCopyKeyword                    : ArgumentKeyword { public override string Text => "LogicalCopy"; }
    public record LogicalCopyInvertedKeyword            : ArgumentKeyword { public override string Text => "LogicalCopyInverted"; }
    public record LogicalNoopKeyword                    : ArgumentKeyword { public override string Text => "LogicalNoop"; }
    public record LogicalInvertKeyword                  : ArgumentKeyword { public override string Text => "LogicalInvert"; }
    public record LogicalAndKeyword                     : ArgumentKeyword { public override string Text => "LogicalAnd"; }
    public record LogicalNandKeyword                    : ArgumentKeyword { public override string Text => "LogicalNand"; }
    public record LogicalOrKeyword                      : ArgumentKeyword { public override string Text => "LogicalOr"; }
    public record LogicalNorKeyword                     : ArgumentKeyword { public override string Text => "LogicalNor"; }
    public record LogicalXorKeyword                     : ArgumentKeyword { public override string Text => "LogicalXor"; }
    public record LogicalEquivKeyword                   : ArgumentKeyword { public override string Text => "LogicalEquiv"; }
    public record LogicalAndReverseKeyword              : ArgumentKeyword { public override string Text => "LogicalAndReverse"; }
    public record LogicalAndInvertedKeyword             : ArgumentKeyword { public override string Text => "LogicalAndInverted"; }
    public record LogicalOrReverseKeyword               : ArgumentKeyword { public override string Text => "LogicalOrReverse"; }
    public record LogicalOrInvertedKeyword              : ArgumentKeyword { public override string Text => "LogicalOrInverted"; }
    public record MultiplyKeyword                       : ArgumentKeyword { public override string Text => "Multiply"; }
    public record ScreenKeyword                         : ArgumentKeyword { public override string Text => "Screen"; }
    public record OverlayKeyword                        : ArgumentKeyword { public override string Text => "Overlay"; }
    public record DarkenKeyword                         : ArgumentKeyword { public override string Text => "Darken"; }
    public record LightenKeyword                        : ArgumentKeyword { public override string Text => "Lighten"; }
    public record ColorDodgeKeyword                     : ArgumentKeyword { public override string Text => "ColorDodge"; }
    public record ColorBurnKeyword                      : ArgumentKeyword { public override string Text => "ColorBurn"; }
    public record HardLightKeyword                      : ArgumentKeyword { public override string Text => "HardLight"; }
    public record SoftLightKeyword                      : ArgumentKeyword { public override string Text => "SoftLight"; }
    public record DifferenceKeyword                     : ArgumentKeyword { public override string Text => "Difference"; }
    public record ExclusionKeyword                      : ArgumentKeyword { public override string Text => "Exclusion"; }
    public record HSLHueKeyword                         : ArgumentKeyword { public override string Text => "HSLHue"; }
    public record HSLSaturationKeyword                  : ArgumentKeyword { public override string Text => "HSLSaturation"; }
    public record HSLColorKeyword                       : ArgumentKeyword { public override string Text => "HSLColor"; }
    public record HSLLuminosityKeyword                  : ArgumentKeyword { public override string Text => "HSLLuminosity"; }
    // Cull
    public record BackKeyword                           : ArgumentKeyword { public override string Text => "Back"; }
    public record FrontKeyword                          : ArgumentKeyword { public override string Text => "Front"; }
    // ZTest from UnityEngine.Rendering.CompareFunction because docs are invalid
    public record DisabledKeyword                       : ArgumentKeyword { public override string Text => "Disabled"; }
    public record NeverKeyword                          : ArgumentKeyword { public override string Text => "Never"; }
    public record LessKeyword                           : ArgumentKeyword { public override string Text => "Less"; }
    public record EqualKeyword                          : ArgumentKeyword { public override string Text => "Equal"; }
    public record LessEqualKeyword                      : ArgumentKeyword { public override string Text => "LessEqual"; }
    public record GreaterKeyword                        : ArgumentKeyword { public override string Text => "Greater"; }
    public record NotEqualKeyword                       : ArgumentKeyword { public override string Text => "NotEqual"; }
    public record GreaterEqualKeyword                   : ArgumentKeyword { public override string Text => "GreaterEqual"; }
    public record AlwaysKeyword                         : ArgumentKeyword { public override string Text => "Always"; }
    
    // attributes: https://docs.unity3d.com/Manual/SL-Properties.html
    public abstract record AttributeToken               : Token<Shaderlab>;
    public record GammaAttribute                        : AttributeToken { public override string Text => "Gamma"; }
    public record HDRAttribute                          : AttributeToken { public override string Text => "HDR"; }
    public record HideInInspectorAttribute              : AttributeToken { public override string Text => "HideInInspector"; }
    public record MainTextureAttribute                  : AttributeToken { public override string Text => "MainTexture"; }
    public record MainColorAttribute                    : AttributeToken { public override string Text => "MainColor"; }
    public record NoScaleOffsetAttribute                : AttributeToken { public override string Text => "NoScaleOffset"; }
    public record NormalAttribute                       : AttributeToken { public override string Text => "Normal"; }
    public record PerRendererDataAttribute              : AttributeToken { public override string Text => "PerRendererData"; }
    // advanced attributes: https://docs.unity3d.com/ScriptReference/MaterialPropertyDrawer.html
    public record ToggleAttribute                       : AttributeToken { public override string Text => "Toggle"; }
    public record ToggleOffAttribute                    : AttributeToken { public override string Text => "ToggleOff"; }
    public record EnumAttribute                         : AttributeToken { public override string Text => "Enum"; }
    public record KeywordEnumAttribute                  : AttributeToken { public override string Text => "KeywordEnum"; }
    public record KeyEnumAttribute                      : AttributeToken { public override string Text => "KeyEnum"; }
    public record PowerSliderAttribute                  : AttributeToken { public override string Text => "PowerSlider"; }
    public record IntRangeAttribute                     : AttributeToken { public override string Text => "IntRange"; }
    public record SpaceAttribute                        : AttributeToken { public override string Text => "Space"; }
    public record HeaderAttribute                       : AttributeToken { public override string Text => "Header"; }
    
    // types: https://docs.unity3d.com/Manual/SL-Properties.html
    public abstract record TypeKeyword                  : Token<Shaderlab>;
    public record IntegerKeyword                        : TypeKeyword { public override string Text => "Integer"; }
    public record FloatKeyword                          : TypeKeyword { public override string Text => "Float"; }
    public record RangeKeyword                          : TypeKeyword { public override string Text => "Range"; }
    public record Texture2DKeyword                      : TypeKeyword { public override string Text => "2D"; }
    public record Texture2DArrayKeyword                 : TypeKeyword { public override string Text => "2DArray"; }
    public record Texture3DKeyword                      : TypeKeyword { public override string Text => "3D"; }
    public record CubeKeyword                           : TypeKeyword { public override string Text => "Cube"; }
    public record CubeArrayKeyword                      : TypeKeyword { public override string Text => "CubeArray"; }
    public record ColorKeyword                          : TypeKeyword { public override string Text => "Color"; }
    public record VectorKeyword                         : TypeKeyword { public override string Text => "Vector"; }
    
    
    // @formatter on

    public abstract record ValidatedToken<Shaderlab> : Token<Shaderlab> {
        protected          string TextUnsafe { get; init; }
        public override    string Text       => TextUnsafe;
        protected abstract Regex  Pattern    { get; }


        /// <summary>Creates new token by validating the text.</summary>
        /// <exception cref="ArgumentException">if text doesn't match pattern</exception>
        public virtual string ValidatedText {
            init {
                if (!Pattern.IsMatch(value))
                    throw new ArgumentException($"Token text: {value} doesn't match pattern: {Pattern}");

                TextUnsafe = value;
            }
        }
    }

    public record IdentifierToken : ValidatedToken<Shaderlab> {
        private static readonly Regex pattern = new(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
        protected override      Regex Pattern => pattern;

        public static implicit operator IdentifierToken(string name) =>
            new IdentifierToken { ValidatedText = name };
    }

    public abstract record Literal : ValidatedToken<Shaderlab>;

    public record QuotedStringLiteral : Literal {
        private static readonly Regex pattern = new Regex(@"^""[^""\n\r]*""$");
        protected override      Regex Pattern => pattern;

        public static implicit operator QuotedStringLiteral(string content) =>
            new QuotedStringLiteral { ValidatedText = $"\"{content}\"" };
    }

    public record AttributeStringLiteral : Literal {
        private static readonly Regex pattern = new Regex(@"^[a-zA-Z0-9_#. \t]+$");
        protected override      Regex Pattern => pattern;

        public static implicit operator AttributeStringLiteral(string str) =>
            new AttributeStringLiteral { ValidatedText = str };
    }

    public record BooleanLiteral : Literal {
        private static readonly Regex pattern = new(@"^(true|false)$");
        protected override      Regex Pattern => pattern;

        public static implicit operator BooleanLiteral(bool value) =>
            new BooleanLiteral { TextUnsafe = value ? bool.TrueString : bool.FalseString };
    }

    public abstract record NumberLiteral : Literal;

    public record FloatLiteral : NumberLiteral {
        private static readonly Regex pattern = new(@"^(\d+\.\d*|\d*\.\d+)$");
        protected override      Regex Pattern => pattern;

        public static implicit operator FloatLiteral(float value) =>
            new FloatLiteral { TextUnsafe = value.ToString("F") };
    }

    public record IntLiteral : NumberLiteral {
        private static readonly Regex pattern = new(@"^\d+$");
        protected override      Regex Pattern => pattern;

        public static implicit operator IntLiteral(int value) =>
            new IntLiteral { TextUnsafe = value.ToString("D") };
    }
}
