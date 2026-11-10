using System;
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.AST.Syntax.CommonSyntax;
namespace me.tooster.sdf.Tests.Editor.AST {
    /// Finds the syntax node types of a language, and the ones that a tree uses.
    static class SyntaxTypes {
        /// Concrete syntax node types in <paramref name="namespacePrefix" />; generic ones as their type definition.
        public static IEnumerable<Type> All<Lang>(string namespacePrefix)
            => typeof(Syntax<Lang>).Assembly.GetTypes()
                .Where(t => !t.IsAbstract && typeof(Syntax<Lang>).IsAssignableFrom(t) || t.IsGenericTypeDefinition && IsSyntaxDefinition<Lang>(t))
                .Where(t => !t.IsAbstract && t.Namespace?.StartsWith(namespacePrefix, StringComparison.Ordinal) == true)
                .Select(Normalize)
                .Distinct();

        /// The syntax node types used in <paramref name="root" />, including structured trivia.
        public static IEnumerable<Type> Of<Lang>(Tree<Lang>.Node root) {
            var collector = new Collector<Lang>();
            collector.Walk(root);
            return collector.types;
        }

        static bool IsSyntaxDefinition<Lang>(Type definition) {
            for (var t = definition.BaseType; t != null; t = t.BaseType)
                if (t == typeof(Syntax<Lang>))
                    return true;
            return false;
        }

        static Type Normalize(Type t) => t.IsGenericType ? t.GetGenericTypeDefinition() : t;

        class Collector<Lang> : Walker<Lang> {
            public readonly HashSet<Type> types = new();

            public Collector() : base(descendIntoStructuredTrivia: true) { }

            public override void Visit(Anchor<Syntax<Lang>> a) {
                types.Add(Normalize(a.Node.GetType()));
                base.Visit(a);
            }

            public override void Visit(Anchor<Statement<Lang>> a) {
                types.Add(Normalize(a.Node.GetType()));
                base.Visit(a);
            }

            public override void Visit(Anchor<Expression<Lang>> a) {
                types.Add(Normalize(a.Node.GetType()));
                base.Visit(a);
            }
        }
    }
}
