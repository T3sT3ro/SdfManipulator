#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;

namespace AST.Syntax {
    public class SyntaxRewriter<TNode, TToken, TBase, TTrivia>
        : SyntaxVisitor<TNode, TNode, TToken, TBase, TTrivia>
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TToken : TBase, ISyntaxToken<TNode, TToken, TTrivia, TBase>
        where TBase : ISyntaxNodeOrToken<TNode, TBase>
        where TTrivia : SyntaxTrivia<TToken, TNode, TTrivia, TBase> {
        public bool DescendIntoStructuredTrivia { get; }

        public SyntaxRewriter(bool descendIntoStructuredTrivia = false) {
            DescendIntoStructuredTrivia = descendIntoStructuredTrivia;
        }

        // entrypoint
        protected override TNode Visit(TNode node) { return Visit((dynamic)new WithParent<dynamic>(node, null)); }

        protected virtual TNode Visit(WithParent<TNode> nodeWithParent) {
            var node = nodeWithParent.Value;
            TNode? modified = null; // will create clone only if children changed
            foreach (var p in node.GetType().GetProperties().Where(p => p.PropertyType.IsSubclassOf(typeof(TBase)))) {
                var child = p.GetValue(node);
                object parentedChild = Activator.CreateInstance(typeof(WithParent<>).MakeGenericType(p.PropertyType),
                    child, nodeWithParent);

                var newChild = Visit((dynamic)parentedChild);
                if (child == newChild)
                    continue;

                modified ??= node with { };
                p.SetValue(modified, newChild);
            }

            return modified ?? node;
        }

        protected virtual TToken Visit(TToken token) { return Visit((dynamic)new WithParent<TToken>(token, null)); }

        protected virtual TToken Visit(WithParent<TToken> tokenWithParent) {
            var token = tokenWithParent.Value;
            // TToken? modified = null;
            var newLeading = new List<TTrivia>();
            bool leadingChanged = false;
            foreach (var trivia in token.LeadingTrivia) {
                var parentedTrivia = Activator.CreateInstance(typeof(WithParent<>).MakeGenericType(trivia.GetType()),
                    trivia, tokenWithParent);
                var newTrivia = (TTrivia) Visit((dynamic)parentedTrivia);
                newLeading.Add(newTrivia);
                leadingChanged |= newTrivia != trivia;
            }

            return token with { };

            foreach (var trailingTrivia in token.TrailingTrivia)
                Visit(trailingTrivia);
        }


        protected virtual TNode Visit(TTrivia trivia) {
            if (trivia.Structure is not null)
                Visit(trivia.Structure);
        }

        protected class DynamicParentNode {
            public DynamicParentNode Parent { get; }

            protected DynamicParentNode(DynamicParentNode parent) { Parent = parent; }
        }

        // possibly extract to superclass
        // Instantiating would be possible only with https://stackoverflow.com/questions/8718199/passing-a-type-to-a-generic-constructor
        protected sealed class WithParent<T> : DynamicParentNode {
            public T Value { get; }
            public WithParent(T value, DynamicParentNode parent) : base(parent) { Value = value; }
        }
    }
}
