using System;
using AST.Hlsl.Syntax;
using AST.Hlsl.Syntax.Expressions;
using AST.Hlsl.Syntax.Expressions.Literals;
using AST.Hlsl.Syntax.Expressions.Operators;

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
            Unary unary => unary.operatorToken switch
            {
                PlusToken or MinusToken => 2.1f,
                NotToken or TildeToken  => 2.2f,

                _ => throw new ArgumentOutOfRangeException()
            },
            Cast => 2.3f,
            Binary binary => binary.operatorToken switch
            {
                AsteriskToken or SlashToken or PercentToken                                        => 3,
                PlusToken or MinusToken                                                            => 4,
                LessThanLessThanToken or GreaterThanGreaterThanToken                               => 5,
                LessThanToken or LessThanEqualsToken or GreaterThanToken or GreaterThanEqualsToken => 6,
                EqualsEqualsToken or ExclamationEqualsToken                                        => 7,
                AmpersandToken                                                                     => 8,
                CaretToken                                                                         => 9,
                BarToken                                                                           => 10,
                AmpersandAmpersandToken                                                            => 11,
                BarBarToken                                                                        => 12,

                _ => throw new ArgumentOutOfRangeException()
            },
            Ternary => 13,
            AssignmentExpresion assignment => assignment.assignmentToken switch
            {
                EqualsToken                                                      => 14.1f,
                PlusEqualsToken or MinusEqualsToken                              => 14.2f,
                AsteriskEqualsToken or SlashEqualsToken or PercentEqualsToken    => 14.3f,
                LessThanLessThanEqualsToken or GreaterThanGreaterThanEqualsToken => 14.4f,
                AmpersandEqualsToken or BarEqualsToken or CaretEqualsToken       => 14.5f,

                _ => throw new ArgumentOutOfRangeException()
            },
            Comma => 15,

            _ => throw new ArgumentOutOfRangeException(nameof(expr))
        };
    }
}
