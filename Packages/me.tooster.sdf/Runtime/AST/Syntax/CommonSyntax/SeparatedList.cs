using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace me.tooster.sdf.AST.Syntax.CommonSyntax {
    public interface ISeparatedList : IEnumerable { }



    public record SeparatedList<Lang, TSyntax> : SyntaxOrTokenList<Lang>, ISeparatedList where TSyntax : Syntax<Lang> {
        public SeparatedList(IReadOnlyList<SyntaxOrToken<Lang>> fullList) : base(fullList) { }
        public SeparatedList(IEnumerable<SyntaxOrToken<Lang>> listWithSeparators) : base(listWithSeparators) { }

        public SeparatedList(params SyntaxOrToken<Lang>[] listWithSeparators) :
            this(listWithSeparators.AsEnumerable()) { }

        public override string ToString() => WriteTo(new StringBuilder()).ToString();

        public static implicit operator SeparatedList<Lang, TSyntax>(TSyntax singleton) => new(new SyntaxOrToken<Lang>[] { singleton });


        #region specializations

        /// Returns number of syntax nodes in this separated list 
        public override int Count => FullList.Count >> 1;

        public new TSyntax this[int index] => (TSyntax)base[index << 1];

        public static SeparatedList<Lang, TSyntax> Empty { get; } = new(Array.Empty<SyntaxOrToken<Lang>>());

        /// returns a valid, separated syntax-list with tokens
        /// A, B, C, D -> slice(1, 2) -> B, C
        /// start and length relate to syntax nodes, not tokens
        public SeparatedList<Lang, TSyntax> Slice<TTok>(int start, int length) where TTok : Token<Lang>
            => new(FullList.OfType<TSyntax>().Skip(start << 1).Take((length << 1) - 1));

        #endregion


        #region utilities

        /// <summary>Returns only syntax nodes, not separators</summary>
        public IEnumerable<TSyntax> Nodes => FullList.Where((_, index) => (index & 1) == 0).Cast<TSyntax>();

        /// <summary>Returns only separators</summary>
        public IEnumerable<Token<Lang>> Separators => FullList.Where((_, index) => (index & 1) != 0).Cast<Token<Lang>>();

        public int SeparatorCount => FullList.Count >> 1;

        /// <summary>
        /// Gets the separator at the given index in this list.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public Token<Lang> GetSeparator(int index) => (Token<Lang>)FullList[(index << 1) + 1];

        /// Builds list with separators from A B C -> A [tok] B [tok] C 
        public static SeparatedList<Lang, TSyntax> WithSeparator<TTok>(IEnumerable<TSyntax> list)
            where TTok : Token<Lang>, new()
            => new(
                list.SelectMany(
                    (x, i) => i == 0
                        ? new SyntaxOrToken<Lang>[] { x }
                        : new SyntaxOrToken<Lang>[] { new TTok(), x }
                )
            );

        /// <summary>see <see cref="WithSeparator{TTok}(System.Collections.Generic.IEnumerable{TSyntax})"/></summary>
        public static SeparatedList<Lang, TSyntax> WithSeparator<TTok>(params TSyntax[] list) where TTok : Token<Lang>, new()
            => WithSeparator<TTok>(list.AsEnumerable());

        #endregion
    }
}
