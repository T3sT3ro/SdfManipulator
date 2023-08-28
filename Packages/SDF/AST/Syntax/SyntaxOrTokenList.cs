using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AST.Syntax {
    /// <summary>
    /// List containing both syntax and tokens of specific language
    /// </summary>
    /// <param name="FullList">full list with syntax and tokens</param>
    public record SyntaxOrTokenList<Lang> : Syntax<Lang>, IReadOnlyList<SyntaxOrToken<Lang>> {
        public IReadOnlyList<SyntaxOrToken<Lang>> FullList { get; }

        public SyntaxOrTokenList(IReadOnlyList<SyntaxOrToken<Lang>> fullList) {
            FullList = fullList.Select(x => x with { Parent = this }).ToList();
        }
        public SyntaxOrTokenList(IEnumerable<SyntaxOrToken<Lang>> list) : this(list.ToList()) { }
        public SyntaxOrTokenList(params SyntaxOrToken<Lang>[]     list) : this(list.AsEnumerable()) { }

        public override IReadOnlyList<SyntaxOrToken<Lang>> ChildNodesAndTokens => FullList;
        public          IEnumerator<SyntaxOrToken<Lang>>   GetEnumerator()     => FullList.GetEnumerator();
        IEnumerator IEnumerable.                           GetEnumerator()     => GetEnumerator();
        public int                                         Count               => FullList.Count;

        // cached empty
        public static SyntaxOrTokenList<Lang> Empty { get; } =
            new SyntaxOrTokenList<Lang>(Array.Empty<SyntaxOrToken<Lang>>());

        // indexers
        public SyntaxOrToken<Lang> this[int index] => FullList[index];

        public SyntaxOrTokenList<Lang> Slice(int start, int length) =>
            new SyntaxOrTokenList<Lang>(FullList.Skip(start).Take(length));

        public SyntaxOrTokenList<Lang> Splice(int index, int deleteCount, IEnumerable<SyntaxOrToken<Lang>> elements)
            => new SyntaxOrTokenList<Lang>(FullList.Splice(index, deleteCount, elements));
    }
}
