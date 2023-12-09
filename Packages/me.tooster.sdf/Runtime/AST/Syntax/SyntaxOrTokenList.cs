using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace me.tooster.sdf.AST.Syntax {
    /// <summary>
    /// List containing both syntax and tokens of specific language
    /// </summary>
    public record SyntaxOrTokenList<Lang> : Syntax<Lang>, IReadOnlyList<SyntaxOrToken<Lang>> {
        /// full list with syntax and tokens
        public IReadOnlyList<SyntaxOrToken<Lang>> FullList { get; }

        public SyntaxOrTokenList(IReadOnlyList<SyntaxOrToken<Lang>> fullList) => FullList = fullList.ToList();

        public SyntaxOrTokenList(IEnumerable<SyntaxOrToken<Lang>> list) : this(list.ToList()) { }
        public SyntaxOrTokenList(params SyntaxOrToken<Lang>[] list) : this(list.AsEnumerable()) { }
        public SyntaxOrTokenList() : this(Array.Empty<SyntaxOrToken<Lang>>()) { }

        public override IReadOnlyList<SyntaxOrToken<Lang>> ChildNodesAndTokens => FullList;
        public virtual  IEnumerator<SyntaxOrToken<Lang>>   GetEnumerator()     => FullList.GetEnumerator();
        IEnumerator IEnumerable.                           GetEnumerator()     => GetEnumerator();
        public virtual int                                 Count               => FullList.Count;

        // indexers
        public virtual SyntaxOrToken<Lang> this[int index] => FullList[index];

        public virtual SyntaxOrTokenList<Lang> Slice(int start, int length) => new(FullList.Skip(start).Take(length));

        public virtual SyntaxOrTokenList<Lang> Splice(int index, int deleteCount,
            IEnumerable<SyntaxOrToken<Lang>> elements)
            => new(FullList.Splice(index, deleteCount, elements));

        internal override void Accept(Visitor<Lang> visitor, Anchor parent) => visitor.Visit(Anchor.New(this, parent));

        internal override TR? Accept<TR>(Visitor<Lang, TR> visitor, Anchor parent) where TR : default =>
            visitor.Visit(Anchor.New(this, parent));
    }
}
