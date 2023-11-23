using System;
using System.Text.RegularExpressions;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Shaderlab.Syntax {
    // @formatter off
    // Keywords
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
    [TokenNode("=")] public partial record EqualsToken;
    [TokenNode("|")] public partial record BarToken;
    [TokenNode(":")] public partial record ColonToken;
    [TokenNode(";")] public partial record SemiToken;
    [TokenNode("<")] public partial record LessThanToken;
    [TokenNode(">")] public partial record GreaterThanToken;
    [TokenNode(",")] public partial record CommaToken;
    [TokenNode(".")] public partial record DotToken;
    [TokenNode("?")] public partial record QuestionToken;
    [TokenNode("#")] public partial record HashToken;
    [TokenNode("/")] public partial record SlashToken;
    // compound
    [TokenNode("||")] public partial record BarBarToken;
    [TokenNode("&&")] public partial record AmpersandAmpersandToken;
    [TokenNode("--")] public partial record MinusMinusToken;
    [TokenNode("++")] public partial record PlusPlusToken;
    [TokenNode("::")] public partial record ColonColonToken;
    [TokenNode("!=")] public partial record ExclamationEqualsToken;
    [TokenNode("==")] public partial record EqualsEqualsToken;
    [TokenNode("<=")] public partial record LessThanEqualsToken;
    [TokenNode("<<")] public partial record LessThanLessThanToken;
    [TokenNode("<<=")] public partial record LessThanLessThanEqualsToken;
    [TokenNode(">=")] public partial record GreaterThanEqualsToken;
    [TokenNode(">>")] public partial record GreaterThanGreaterThanToken;
    [TokenNode(">>=")] public partial record GreaterThanGreaterThanEqualsToken;
    [TokenNode("/=")] public partial record SlashEqualsToken;
    [TokenNode("*=")] public partial record AsteriskEqualsToken;
    [TokenNode("|=")] public partial record BarEqualsToken;
    [TokenNode("&=")] public partial record AmpersandEqualsToken;
    [TokenNode("+=")] public partial record PlusEqualsToken;
    [TokenNode("-=")] public partial record MinusEqualsToken;
    [TokenNode("^=")] public partial record CaretEqualsToken;
    [TokenNode("%=")] public partial record PercentEqualsToken;
    
    // keywords
    [TokenNode("Shader")] public partial record ShaderKeyword;
    [TokenNode("SubShader")] public partial record SubShaderKeyword;
    [TokenNode("Pass")] public partial record PassKeyword;
    [TokenNode("HLSLINCLUDE")] public partial record HlslIncludeKeyword;
    [TokenNode("HLSLPROGRAM")] public partial record HlslProgramKeyword;
    [TokenNode("ENDHLSL")] public partial record EndHlslKeyword;
    [TokenNode("Fallback")] public partial record FallbackKeyword;
    [TokenNode("CustomEditor")] public partial record CustomEditorKeyword;
    [TokenNode("CustomEditorForRenderPipeline")] public partial record CustomEditorForRenderPipelineKeyword;
    [TokenNode("Properties")] public partial record PropertiesKeyword;
    [TokenNode("Tags")] public partial record TagsKeyword;
    [TokenNode("On")] public partial record OnKeyword;
    [TokenNode("True")] public partial record TrueKeyword;
    [TokenNode("False")] public partial record FalseKeyword;
    [TokenNode("LOD")] public partial record LodKeyword;
    [TokenNode("Name")] public partial record NameKeyword;
    
    // commands
    [TokenNode("AlphaToMask")] public partial record AlphaToMaskKeyword;
    [TokenNode("Blend")] public partial record BlendKeyword;
    [TokenNode("BlendOp")] public partial record BlendOpKeyword;
    [TokenNode("ColorMask")] public partial record ColorMaskKeyword;
    [TokenNode("Conservative")] public partial record ConservativeKeyword;
    [TokenNode("Cull")] public partial record CullKeyword;
    [TokenNode("Offset")] public partial record OffsetKeyword;
    [TokenNode("Stencil")] public partial record StencilKeyword;
    [TokenNode("ZClip")] public partial record ZClipKeyword;
    [TokenNode("ZTest")] public partial record ZTestKeyword;
    [TokenNode("ZWrite")] public partial record ZWriteKeyword;
    [TokenNode("UsePass")] public partial record UsePassKeyword;
    [TokenNode("GrabPass")] public partial record GrabPassKeyword;
    // CommandARguments
    public abstract record ArgumentKeyword : Token<shaderlab>;
    // Blend
    [TokenNode("Off")] public partial record OffKeyword : ArgumentKeyword;
    [TokenNode("One")] public partial record OneKeyword : ArgumentKeyword;
    [TokenNode("Zero")] public partial record ZeroKeyword : ArgumentKeyword;
    [TokenNode("SrcColor")] public partial record SrcColorKeyword : ArgumentKeyword;
    [TokenNode("SrcAlpha")] public partial record SrcAlphaKeyword : ArgumentKeyword;
    [TokenNode("SrcAlphaSaturate")] public partial record SrcAlphaSaturateKeyword : ArgumentKeyword;
    [TokenNode("DstColor")] public partial record DstColorKeyword : ArgumentKeyword;
    [TokenNode("DstAlpha")] public partial record DstAlphaKeyword : ArgumentKeyword;
    [TokenNode("OneMinusSrcColor")] public partial record OneMinusSrcColorKeyword : ArgumentKeyword;
    [TokenNode("OneMinusSrcAlpha")] public partial record OneMinusSrcAlphaKeyword : ArgumentKeyword;
    [TokenNode("OneMinusDstColor")] public partial record OneMinusDstColorKeyword : ArgumentKeyword;
    [TokenNode("OneMinusDstAlpha")] public partial record OneMinusDstAlphaKeyword : ArgumentKeyword;
    // BlendOp
    [TokenNode("Add")] public partial record AddKeyword : ArgumentKeyword;
    [TokenNode("Sub")] public partial record SubKeyword : ArgumentKeyword;
    [TokenNode("RevSub")] public partial record RevSubKeyword : ArgumentKeyword;
    [TokenNode("Min")] public partial record MinKeyword : ArgumentKeyword;
    [TokenNode("Max")] public partial record MaxKeyword : ArgumentKeyword;
    [TokenNode("LogicalClear")] public partial record LogicalClearKeyword : ArgumentKeyword;
    [TokenNode("LogicalSet")] public partial record LogicalSetKeyword : ArgumentKeyword;
    [TokenNode("LogicalCopy")] public partial record LogicalCopyKeyword : ArgumentKeyword;
    [TokenNode("LogicalCopyInverted")] public partial record LogicalCopyInvertedKeyword : ArgumentKeyword;
    [TokenNode("LogicalNoop")] public partial record LogicalNoopKeyword : ArgumentKeyword;
    [TokenNode("LogicalInvert")] public partial record LogicalInvertKeyword : ArgumentKeyword;
    [TokenNode("LogicalAnd")] public partial record LogicalAndKeyword : ArgumentKeyword;
    [TokenNode("LogicalNand")] public partial record LogicalNandKeyword : ArgumentKeyword;
    [TokenNode("LogicalOr")] public partial record LogicalOrKeyword : ArgumentKeyword;
    [TokenNode("LogicalNor")] public partial record LogicalNorKeyword : ArgumentKeyword;
    [TokenNode("LogicalXor")] public partial record LogicalXorKeyword : ArgumentKeyword;
    [TokenNode("LogicalEquiv")] public partial record LogicalEquivKeyword : ArgumentKeyword;
    [TokenNode("LogicalAndReverse")] public partial record LogicalAndReverseKeyword : ArgumentKeyword;
    [TokenNode("LogicalAndInverted")] public partial record LogicalAndInvertedKeyword : ArgumentKeyword;
    [TokenNode("LogicalOrReverse")] public partial record LogicalOrReverseKeyword : ArgumentKeyword;
    [TokenNode("LogicalOrInverted")] public partial record LogicalOrInvertedKeyword : ArgumentKeyword;
    [TokenNode("Multiply")] public partial record MultiplyKeyword : ArgumentKeyword;
    [TokenNode("Screen")] public partial record ScreenKeyword : ArgumentKeyword;
    [TokenNode("Overlay")] public partial record OverlayKeyword : ArgumentKeyword;
    [TokenNode("Darken")] public partial record DarkenKeyword : ArgumentKeyword;
    [TokenNode("Lighten")] public partial record LightenKeyword : ArgumentKeyword;
    [TokenNode("ColorDodge")] public partial record ColorDodgeKeyword : ArgumentKeyword;
    [TokenNode("ColorBurn")] public partial record ColorBurnKeyword : ArgumentKeyword;
    [TokenNode("HardLight")] public partial record HardLightKeyword : ArgumentKeyword;
    [TokenNode("SoftLight")] public partial record SoftLightKeyword : ArgumentKeyword;
    [TokenNode("Difference")] public partial record DifferenceKeyword : ArgumentKeyword;
    [TokenNode("Exclusion")] public partial record ExclusionKeyword : ArgumentKeyword;
    [TokenNode("HSLHue")] public partial record HSLHueKeyword : ArgumentKeyword;
    [TokenNode("HSLSaturation")] public partial record HSLSaturationKeyword : ArgumentKeyword;
    [TokenNode("HSLColor")] public partial record HSLColorKeyword : ArgumentKeyword;
    [TokenNode("HSLLuminosity")] public partial record HSLLuminosityKeyword : ArgumentKeyword;
    // Cull
    [TokenNode("Back")] public partial record BackKeyword : ArgumentKeyword;
    [TokenNode("Front")] public partial record FrontKeyword : ArgumentKeyword;
    // ZTest from UnityEngine.Rendering.CompareFunction because docs are invalid
    [TokenNode("Disabled")] public partial record DisabledKeyword : ArgumentKeyword;
    [TokenNode("Never")] public partial record NeverKeyword : ArgumentKeyword;
    [TokenNode("Less")] public partial record LessKeyword : ArgumentKeyword;
    [TokenNode("Equal")] public partial record EqualKeyword : ArgumentKeyword;
    [TokenNode("LessEqual")] public partial record LessEqualKeyword : ArgumentKeyword;
    [TokenNode("Greater")] public partial record GreaterKeyword : ArgumentKeyword;
    [TokenNode("NotEqual")] public partial record NotEqualKeyword : ArgumentKeyword;
    [TokenNode("GreaterEqual")] public partial record GreaterEqualKeyword : ArgumentKeyword;
    [TokenNode("Always")] public partial record AlwaysKeyword : ArgumentKeyword;
    
    // attributes: https://docs.unity3d.com/Manual/SL-Properties.html
    public abstract record AttributeToken : Token<shaderlab>;
    [TokenNode("Gamma")] public partial record GammaAttribute : AttributeToken;
    [TokenNode("HDR")] public partial record HDRAttribute : AttributeToken;
    [TokenNode("HideInInspector")] public partial record HideInInspectorAttribute : AttributeToken;
    [TokenNode("MainTexture")] public partial record MainTextureAttribute : AttributeToken;
    [TokenNode("MainColor")] public partial record MainColorAttribute : AttributeToken;
    [TokenNode("NoScaleOffset")] public partial record NoScaleOffsetAttribute : AttributeToken;
    [TokenNode("Normal")] public partial record NormalAttribute : AttributeToken;
    [TokenNode("PerRendererData")] public partial record PerRendererDataAttribute : AttributeToken;
    // advanced attributes: https://docs.unity3d.com/ScriptReference/MaterialPropertyDrawer.html
    [TokenNode("Toggle")] public partial record ToggleAttribute : AttributeToken;
    [TokenNode("ToggleOff")] public partial record ToggleOffAttribute : AttributeToken;
    [TokenNode("Enum")] public partial record EnumAttribute : AttributeToken;
    [TokenNode("KeywordEnum")] public partial record KeywordEnumAttribute : AttributeToken;
    [TokenNode("KeyEnum")] public partial record KeyEnumAttribute : AttributeToken;
    [TokenNode("PowerSlider")] public partial record PowerSliderAttribute : AttributeToken;
    [TokenNode("IntRange")] public partial record IntRangeAttribute : AttributeToken;
    [TokenNode("Space")] public partial record SpaceAttribute : AttributeToken;
    [TokenNode("Header")] public partial record HeaderAttribute : AttributeToken;
    
    // types: https://docs.unity3d.com/Manual/SL-Properties.html
    public abstract record TypeKeyword : Token<shaderlab>;
    [TokenNode("Integer")] public partial record IntegerKeyword : TypeKeyword;
    [TokenNode("Float")] public partial record FloatKeyword : TypeKeyword;
    [TokenNode("Range")] public partial record RangeKeyword : TypeKeyword;
    [TokenNode("2D")] public partial record Texture2DKeyword : TypeKeyword;
    [TokenNode("2DArray")] public partial record Texture2DArrayKeyword : TypeKeyword;
    [TokenNode("3D")] public partial record Texture3DKeyword : TypeKeyword;
    [TokenNode("Cube")] public partial record CubeKeyword : TypeKeyword;
    [TokenNode("CubeArray")] public partial record CubeArrayKeyword : TypeKeyword;
    [TokenNode("Color")] public partial record ColorKeyword : TypeKeyword;
    [TokenNode("Vector")] public partial record VectorKeyword : TypeKeyword;
    
    
    // @formatter on

    public abstract record ValidatedToken<Shaderlab> : Token<Shaderlab> {
        protected          string TextUnsafe { get; init; }
        public override    string FullText       => TextUnsafe;
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

    public record IdentifierToken : ValidatedToken<shaderlab> {
        private static readonly Regex pattern = new(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
        protected override      Regex Pattern => pattern;

        public static implicit operator IdentifierToken(string name) =>
            new() { ValidatedText = name };
    }

    public abstract record Literal : ValidatedToken<shaderlab>;

    public record QuotedStringLiteral : Literal {
        private static readonly Regex pattern = new(@"^""[^""\n\r]*""$");
        protected override      Regex Pattern => pattern;

        public static implicit operator QuotedStringLiteral(string content) =>
            new() { ValidatedText = $"\"{content}\"" };
    }

    public record AttributeStringLiteral : Literal {
        private static readonly Regex pattern = new(@"^[a-zA-Z0-9_#. \t]+$");
        protected override      Regex Pattern => pattern;

        public static implicit operator AttributeStringLiteral(string str) =>
            new() { ValidatedText = str };
    }

    public record BooleanLiteral : Literal {
        private static readonly Regex pattern = new(@"^(true|false)$");
        protected override      Regex Pattern => pattern;

        public static implicit operator BooleanLiteral(bool value) =>
            new() { TextUnsafe = value ? bool.TrueString : bool.FalseString };
    }

    public abstract record NumberLiteral : Literal;

    public record FloatLiteral : NumberLiteral {
        private static readonly Regex pattern = new(@"^(\d+\.\d*|\d*\.\d+)$");
        protected override      Regex Pattern => pattern;

        public static implicit operator FloatLiteral(float value) =>
            new() { TextUnsafe = value.ToString("F") };
    }

    public record IntLiteral : NumberLiteral {
        private static readonly Regex pattern = new(@"^\d+$");
        protected override      Regex Pattern => pattern;

        public static implicit operator IntLiteral(int value) =>
            new() { TextUnsafe = value.ToString("D") };
    }
}
