using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AST.Hlsl;

namespace AST.Syntax {
    public abstract record SeparatedSyntaxList<TNode, TToken, TBase>
        : SyntaxNode<TNode, TBase>, IReadOnlyList<TNode>
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TToken : TBase
        where TBase : ISyntaxNodeOrToken<TNode, TBase> {
        private readonly IReadOnlyList<TBase> list;

        internal SeparatedSyntaxList(IEnumerable<TBase> list) { this.list = list.ToList(); }
        
        IEnumerable<TBase> Base;
        IEnumerable<TNode> Derived;
        
        internal SeparatedSyntaxList(params TNode[] expressions) { ;
            this.list = expressions.Select(x => (TBase)x).ToList().AsReadOnly();
        }

        public int Count => (list.Count + 1) >> 1;

        public int SeparatorCount => list.Count >> 1;

        public TNode this[int index] => (TNode)list[index << 1];

        /// <summary>
        /// Gets the separator at the given index in this list.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public TToken GetSeparator(int index) { return (TToken)list[(index << 1) + 1]; }

        public IEnumerable<TBase> GetWithSeparators() { return list; }

        public IEnumerable<TToken> GetSeparators() => (IEnumerable<TToken>)list.Where((_, index) => index % 2 != 0);

        public IEnumerator<TNode> GetEnumerator() { return new EnumeratorImpl(this); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public override int GetHashCode() => list.GetHashCode();

        private sealed class EnumeratorImpl : IEnumerator<TNode> {
            private readonly SeparatedSyntaxList<TNode, TToken, TBase> _list;

            private int _index;

            internal EnumeratorImpl(SeparatedSyntaxList<TNode, TToken, TBase> list) {
                _list = list;
                _index = -1;
            }

            public TNode Current => _list[_index];

            object IEnumerator.Current => _list[_index];

            public void Dispose() { }

            public bool MoveNext() {
                int newIndex = _index + 1;
                if (newIndex >= _list.Count) return false;

                _index = newIndex;
                return true;
            }

            public void Reset() { _index = -1; }
        }
    }
}
