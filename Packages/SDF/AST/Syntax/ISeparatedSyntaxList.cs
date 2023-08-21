using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AST.Syntax {
    // list of syntax and tokens with utility methods for slicing, indexers, enumerators and constructors.
    public interface ISyntaxOrTokenList<TNode, TBase, TSelf> : IReadOnlyList<TBase>
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TBase : ISyntaxNodeOrToken<TNode, TBase>
        where TSelf : ISyntaxOrTokenList<TNode, TBase, TSelf>, new() {
        public IReadOnlyList<TBase> All { get; protected init; }


        int IReadOnlyCollection<TBase>.Count => this.Count;
        TBase IReadOnlyList<TBase>.this[int index] => this[index];

        // 3 below generate the [^i] and [s..e] syntax
        // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-8.0/ranges
        public new int   Count                        => All.Count;
        public     TSelf Slice(int start, int length) => Of(this.Skip(start).Take(length));
        public new TBase this[int  index] => All[index];

        /// Build another strongly typed list of the same type
        protected static TSelf Of(IEnumerable<TBase> elements) => new() { All = elements.ToList() };

        /// Rturn new list, but starting at index with `deleteCount` elements removed and `elements` inserted instead
        public TSelf ToSpliced(int index, int deleteCount, IEnumerable<TBase> elements) =>
            Of(this.Splice(index, deleteCount, elements));


        IEnumerator<TBase> IEnumerable<TBase>.GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.              GetEnumerator() => GetEnumerator();


        private sealed class Enumerator : IEnumerator<TBase> {
            private readonly ISyntaxOrTokenList<TNode, TBase, TSelf> list;
            private          int                                     index;

            internal Enumerator(ISyntaxOrTokenList<TNode, TBase, TSelf> list) {
                this.list = list;
                index = -1;
            }

            public TBase       Current => list[index];
            object IEnumerator.Current => list[index];

            public void Dispose() { }

            public bool MoveNext() {
                int newIndex = index + 1;
                if (newIndex >= list.Count) return false;

                index = newIndex;
                return true;
            }

            public void Reset() { index = -1; }
        }
    }

    // list of a concrete syntax

    public interface ISyntaxList<TNode, TBase, TSelf> : ISyntaxOrTokenList<TNode, TBase, TSelf>
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TBase : ISyntaxNodeOrToken<TNode, TBase>
        where TSelf : ISyntaxOrTokenList<TNode, TBase, TSelf>, new() {
        new IReadOnlyList<TNode> All { get; init; }

        IReadOnlyList<TBase> ISyntaxOrTokenList<TNode, TBase, TSelf>.All {
            get => All;
            init => All = value.Cast<TNode>().ToList();
        }
    }


    // ========================================================================

    public interface ISeparatedSyntaxList<TNode, TToken, TBase> : IReadOnlyList<TNode>
        where TNode : SyntaxNode<TNode, TBase>, TBase
        where TToken : TBase
        where TBase : ISyntaxNodeOrToken<TNode, TBase> {
        public IReadOnlyList<TBase> FullList { get; }

        public IEnumerable<TToken> Separators => FullList.Where((_, index) => index % 2 != 0).Cast<TToken>();
        public IEnumerable<TNode>  Nodes      => FullList.Where((_, index) => index % 2 == 0).Cast<TNode>();

        /// returns the number of sytnax nodes in the list
        int IReadOnlyCollection<TNode>.Count => (FullList.Count + 1) >> 1;

        public int SeparatorCount => FullList.Count >> 1;

        new TNode this[int                  index] => (TNode)FullList[index << 1];
        TNode IReadOnlyList<TNode>.this[int index] => this[index];

        // TODO support index and range operators: [^2] and [1..3]
        // support :
        // - with(index, element)
        // - with(index, IEnumerable)
        // - [^x] from end index
        // - [a..b] range slice
        // - splice(index, deleteCount, IEnumerable inserted)

        /// <summary>
        /// Gets the separator at the given index in this list.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public TToken GetSeparator(int index) { return (TToken)FullList[(index << 1) + 1]; }

        IEnumerator<TNode> IEnumerable<TNode>.GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.              GetEnumerator() => GetEnumerator();

        private sealed class Enumerator : IEnumerator<TNode> {
            private readonly ISeparatedSyntaxList<TNode, TToken, TBase> _list;

            private int _index;

            internal Enumerator(ISeparatedSyntaxList<TNode, TToken, TBase> list) {
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
