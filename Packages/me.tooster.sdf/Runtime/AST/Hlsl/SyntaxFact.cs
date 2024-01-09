using System;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;
using Statement = me.tooster.sdf.AST.Syntax.CommonSyntax.Statement<me.tooster.sdf.AST.hlsl>;
using Comma = me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators.Comma;

namespace me.tooster.sdf.AST.Hlsl {
    // Taken from cppreference table
    [Obsolete("this was written a long time before the final AST packages took form, nevertheless it contains many useful values for precedence that may be useful in the future")]
    public static class SyntaxFact {
        /// Operators whose children have higher precedence require parentheses around children
        public static float Precedence(this Expression expr) => expr switch
        {
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
            AssignmentExpression assignment => assignment.assignmentToken switch
            {
                EqualsToken                                                      => 14.1f,
                PlusEqualsToken or MinusEqualsToken                              => 14.2f,
                AsteriskEqualsToken or SlashEqualsToken or PercentEqualsToken    => 14.3f,
                LessThanLessThanEqualsToken or GreaterThanGreaterThanEqualsToken => 14.4f,
                AmpersandEqualsToken or BarEqualsToken or CaretEqualsToken       => 14.5f,

                _ => throw new ArgumentOutOfRangeException()
            },
            Comma => 15,

            _ => 0, // is that correct?
        };
    }
}
