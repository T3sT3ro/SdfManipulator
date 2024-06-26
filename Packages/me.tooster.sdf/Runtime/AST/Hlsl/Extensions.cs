using System;
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
using Expression = me.tooster.sdf.AST.Syntax.CommonSyntax.Expression<me.tooster.sdf.AST.hlsl>;
using Statement = me.tooster.sdf.AST.Syntax.CommonSyntax.Statement<me.tooster.sdf.AST.hlsl>;
using Type = me.tooster.sdf.AST.Hlsl.Syntax.Type;
using VariableDefinition = me.tooster.sdf.AST.Hlsl.Syntax.Statements.VariableDefinition;

namespace me.tooster.sdf.AST.Hlsl {
    public static class Extensions {
        // todo: parenthesize by default, simplify with rules under associativity and precedence
        public static Expression ParenthesizeFor(this Expression child, Expression parent)
            => child.Precedence() > parent.Precedence() ? new Parenthesized { expression = child } : child;

        // convert enumerables to appropriate lists
        public static SyntaxList<hlsl, T> ToSyntaxList<T>(this IEnumerable<T> args) where T : Syntax<hlsl> => new(args);

        public static ArgumentList<T> ToArgumentList<T>(this IEnumerable<T> args) where T : Syntax<hlsl>
            => new() { arguments = args.CommaSeparated() };

        public static BracedList<T> ToBracedList<T>(this IEnumerable<T> args) where T : Syntax<hlsl>
            => new() { arguments = args.CommaSeparated() };


        public static SeparatedList<hlsl, T> CommaSeparated<T>(this IEnumerable<T> nodes)
            where T : Syntax<hlsl>
            => SeparatedList<hlsl, T>.WithSeparator<CommaToken>(nodes);

        public static SeparatedList<hlsl, T> CommaSeparated<T>(this T singleton)
            where T : Syntax<hlsl>
            => SeparatedList<hlsl, T>.WithSeparator<CommaToken>(singleton);

        /// from a left string like 'a.b.c' generates assignment expression 
        public static Statement Assignment(string left, Expression right, AssignmentToken assignmentToken = null)
            => new ExpressionStatement
            {
                expression = new AssignmentExpression
                {
                    left = LValue(left),
                    assignmentToken = assignmentToken ?? new EqualsToken(),
                    right = right,
                },
            };

        public static Call FunctionCall(string function, IEnumerable<Expression> args)
            => new()
            {
                calee = NameFrom(function),
                argList = args.ToArgumentList(),
            };

        public static Call FunctionCall(string function, params Expression[] args) => FunctionCall(function, args.AsEnumerable());

        /// takes string like 'x' or 'a.b.c' and generates an lvalue expression, like <see cref="Identifier"/> or <see cref="Member"/>
        public static Expression LValue(string path) {
            var names = path.Split('.');
            if (names.Length == 1) return (Identifier)names[0];
            return names.Skip(1).Aggregate(
                (Identifier)names.First() as Expression,
                (left, name) => new Member { expression = left, member = name }
            );
        }

        public static VariableDefinition Var(Type type, string name, Expression? initializer = null) {
            var definition = new VariableDeclarator.Definition { id = name };
            return new VariableDeclarator
            {
                type = type,
                variables = initializer == null ? definition : definition with { initializer = initializer },
            };
        }

        public static NameSyntax NameFrom(params string[] names) {
            switch (names.Length) {
                case 0: throw new ArgumentException("can't create empty name");
                case 1: {
                    names = names[0].Split("::");
                    if (names.Length == 1) return new Identifier { id = names[0] };
                    break;
                }
            }
            var identifiers = names.Select(name => new Identifier { id = name }).ToArray();
            // creates left-associative tree like (((a::b)::c)::d)
            return identifiers.Skip(1).Aggregate(
                identifiers.First() as NameSyntax,
                (current, id) => new QualifiedNameSyntax { Left = current, Right = id }
            );
        }
    }
}
