using System;
using AST.Hlsl.Syntax;
using AST.Hlsl.Syntax.Expressions.Operators;
using AST.Hlsl.Syntax.Statements;
using Type = AST.Hlsl.Syntax.Type.Type;

namespace AST.Hlsl {
    public static class Stringifier {
        public static string Get(Type.Scalar scalar) => scalar.kind switch
        {
            Type.Scalar.Kind.BOOL   => "bool",
            Type.Scalar.Kind.INT    => "int",
            Type.Scalar.Kind.UINT   => "uint",
            Type.Scalar.Kind.HALF   => "half",
            Type.Scalar.Kind.FLOAT  => "float",
            Type.Scalar.Kind.DOUBLE => "double",

            _ => throw new ArgumentOutOfRangeException(nameof(scalar.kind), scalar.kind, "bad scalar type")
        };

        public static string Get(Type.Texture texture) => texture.kind switch
        {
            Type.Texture.Kind._1D       => "Texture1D",
            Type.Texture.Kind._1D_ARRAY => "Texture1DArray",
            Type.Texture.Kind._2D       => "Texture2D",
            Type.Texture.Kind._2D_ARRAY => "Texture2DArray",
            Type.Texture.Kind._3D       => "Texture3D",
            Type.Texture.Kind._CUBE     => "TextureCube",

            _ => throw new ArgumentOutOfRangeException(nameof(texture.kind), texture.kind, "bad texture type")
        };

        public static string Get(Type type) => type switch
        {
            Type.Matrix matrix   => $"{Get(matrix.scalar)}{matrix.rows}x{matrix.cols}",
            Type.Scalar scalar   => Get(scalar),
            Type.Struct @struct  => @struct.id.identifier.Text,
            Type.Texture texture => Get(texture),
            Type.Vector vector   => $"{Get(vector.scalar)}{vector.arity}",

            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "bad type")
        };

        public static string Get(Assignment.Kind assOp) => assOp switch
        {
            Assignment.Kind.ASSIGN             => "=",
            Assignment.Kind.ADD_ASSIGN         => "+=",
            Assignment.Kind.SUB_ASSIGN         => "-=",
            Assignment.Kind.MUL_ASSIGN         => "*=",
            Assignment.Kind.DIV_ASSIGN         => "/=",
            Assignment.Kind.MOD_ASSIGN         => "%=",
            Assignment.Kind.BIT_L_SHIFT_ASSIGN => "<<=",
            Assignment.Kind.BIT_R_SHIFT_ASSIGN => ">>=",
            Assignment.Kind.BIT_AND_ASSIGN     => "&=",
            Assignment.Kind.BIT_OR_ASSIGN      => "|=",
            Assignment.Kind.BIT_XOR_ASSIGN     => "^=",

            _ => throw new ArgumentOutOfRangeException(nameof(assOp), assOp, "bad assignment operator")
        };

        public static string Get(Binary.Kind binOp) => binOp switch
        {
            Binary.Kind.PLUS           => "+",
            Binary.Kind.MINUS          => "-",
            Binary.Kind.MUL            => "*",
            Binary.Kind.DIV            => "/",
            Binary.Kind.MOD            => "%",
            Binary.Kind.BIT_LSHIFT     => "<<",
            Binary.Kind.BIT_RSHIFT     => ">>",
            Binary.Kind.BIT_AND        => "&",
            Binary.Kind.BIT_OR         => "|",
            Binary.Kind.BIT_XOR        => "^",
            Binary.Kind.CMP_LESS       => "<",
            Binary.Kind.CMP_LESS_EQ    => "<=",
            Binary.Kind.CMP_GREATER    => ">",
            Binary.Kind.CMP_GREATER_EQ => ">=",
            Binary.Kind.CMP_EQ         => "==",
            Binary.Kind.CMP_NOT_EQ     => "!=",
            Binary.Kind.LOGICAL_AND    => "&&",
            Binary.Kind.LOGICAL_OR     => "||",

            _ => throw new ArgumentOutOfRangeException(nameof(binOp), binOp, "bad binary operator")
        };


        public static string Get(Unary.Kind unaryOp) => unaryOp switch
        {
            Unary.Kind.MINUS       => "-",
            Unary.Kind.PLUS        => "+",
            Unary.Kind.LOGICAL_NOT => "~",
            Unary.Kind.BIT_NOT     => "!",

            _ => throw new ArgumentOutOfRangeException(nameof(unaryOp), unaryOp, "bad unary operator")
        };

        public static string Get(Affix.Kind affix) => affix switch
        {
            Affix.Kind.DEC => "--",
            Affix.Kind.INC => "++",

            _ => throw new ArgumentOutOfRangeException(nameof(affix), affix, "bad affix operator")
        };

        public static string Get(Semantic.Kind semantic) => semantic switch
        {
            Semantic.Kind.BINORMAL     => "BINORMAL",
            Semantic.Kind.BLENDINDICES => "BLENDINDICES",
            Semantic.Kind.BLENDWEIGHT  => "BLENDWEIGHT",
            Semantic.Kind.COLOR        => "COLOR",
            Semantic.Kind.NORMAL       => "NORMAL",
            Semantic.Kind.POSITION     => "POSITION",
            Semantic.Kind.POSITIONT    => "POSITIONT",
            Semantic.Kind.PSIZE        => "PSIZE",
            Semantic.Kind.TANGENT      => "TANGENT",
            Semantic.Kind.TEXCOORD     => "TEXCOORD",
            Semantic.Kind.FOG          => "FOG",
            Semantic.Kind.TESSFACTOR   => "TESSFACTOR",
            Semantic.Kind.VFACE        => "VFACE",
            Semantic.Kind.VPOS         => "VPOS",
            Semantic.Kind.DEPTH        => "DEPTH",

            _ => throw new ArgumentOutOfRangeException(nameof(semantic), semantic, "bad semantic")
        };

        public static string Get(VariableDeclaration.Storage storage) => storage switch
        {
            VariableDeclaration.Storage.EXTERN          => "extern",
            VariableDeclaration.Storage.NOINTERPOLATION => "nointerpolation",
            VariableDeclaration.Storage.PRECISE         => "precise",
            VariableDeclaration.Storage.SHARED          => "shared",
            VariableDeclaration.Storage.GROUPSHARED     => "groupshared",
            VariableDeclaration.Storage.STATIC          => "static",
            VariableDeclaration.Storage.UNIFORM         => "uniform",

            _ => throw new ArgumentOutOfRangeException(nameof(storage), storage, "bad variable storage")
        };


        public static string Get(VariableDeclaration.TypeModifier typeMod) => typeMod switch
        {
            VariableDeclaration.TypeModifier.CONST        => "const",
            VariableDeclaration.TypeModifier.ROW_MAJOR    => "row_major",
            VariableDeclaration.TypeModifier.COLUMN_MAJOR => "column_major",

            _ => throw new ArgumentOutOfRangeException(nameof(typeMod), typeMod, "bad variable type modifier")
        };

        public static string Get(FunctionDeclaration.Argument.Modifier mod) => mod switch
        {
            FunctionDeclaration.Argument.Modifier.IN      => "in",
            FunctionDeclaration.Argument.Modifier.OUT     => "out",
            FunctionDeclaration.Argument.Modifier.INOUT   => "inout",
            FunctionDeclaration.Argument.Modifier.UNIFORM => "uniform",

            _ => throw new ArgumentOutOfRangeException(nameof(mod), mod, "bad argument modifier")
        };

        public static string Get(StructDeclaration.Member.Interpolation interp) => interp switch
        {
            StructDeclaration.Member.Interpolation.LINEAR          => "linear",
            StructDeclaration.Member.Interpolation.CENTROID        => "centroid",
            StructDeclaration.Member.Interpolation.NOINTERPOLATION => "nointerpolation",
            StructDeclaration.Member.Interpolation.NOPERSPECTIVE   => "noperspective",
            StructDeclaration.Member.Interpolation.SAMPLE          => "sample",

            _ => throw new ArgumentOutOfRangeException(nameof(interp), interp, "bad interpolation")
        };
    }
}
