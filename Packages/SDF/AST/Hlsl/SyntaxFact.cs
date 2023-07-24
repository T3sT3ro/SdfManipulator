using System;
using AST.Hlsl.Syntax.Expressions;
using AST.Hlsl.Syntax.Expressions.Literals;
using AST.Hlsl.Syntax.Expressions.Operators;
using static AST.Hlsl.Syntax.Expressions.Operators.Binary.Kind;
using static AST.Hlsl.Syntax.Expressions.Operators.Assignment.Kind;
using static AST.Hlsl.Syntax.Expressions.Operators.Unary;

namespace AST.Hlsl {
    public static class SyntaxFact {
        /// Operators whose children have higher precedence require parentheses around children
        public static float Precedence(this Expression expr) => expr switch
        {
            Literal    => 0, // is that correct?
            Affix.Pre  => 1.1f,
            Affix.Post => 1.2f,
            Call       => 1.3f,
            Indexer    => 1.4f,
            Member     => 1.5f,
            Unary unary => unary.kind switch
            {
                Kind.PLUS or Kind.MINUS          => 2.1f,
                Kind.LOGICAL_NOT or Kind.BIT_NOT => 2.2f,

                _ => throw new ArgumentOutOfRangeException()
            },
            Cast => 2.3f,
            Binary binary => binary.kind switch
            {
                MUL or DIV or MOD                                        => 3,
                PLUS or MINUS                                            => 4,
                BIT_LSHIFT or BIT_RSHIFT                                 => 5,
                CMP_LESS or CMP_LESS_EQ or CMP_GREATER or CMP_GREATER_EQ => 6,
                CMP_EQ or CMP_NOT_EQ                                     => 7,
                BIT_AND                                                  => 8,
                BIT_XOR                                                  => 9,
                BIT_OR                                                   => 10,
                LOGICAL_AND                                              => 11,
                LOGICAL_OR                                               => 12,

                _ => throw new ArgumentOutOfRangeException()
            },
            Ternary => 13,
            Assignment assignment => assignment.kind switch
            {
                ASSIGN                                            => 14.1f,
                ADD_ASSIGN or SUB_ASSIGN                          => 14.2f,
                MUL_ASSIGN or DIV_ASSIGN or MOD_ASSIGN            => 14.3f,
                BIT_L_SHIFT_ASSIGN or BIT_R_SHIFT_ASSIGN          => 14.4f,
                BIT_AND_ASSIGN or BIT_OR_ASSIGN or BIT_XOR_ASSIGN => 14.5f,

                _ => throw new ArgumentOutOfRangeException()
            },
            Comma => 15,

            _ => throw new ArgumentOutOfRangeException(nameof(expr))
        };

    }
}
