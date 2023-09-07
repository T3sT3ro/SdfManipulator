using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace me.tooster.sdf.AST.Syntax {
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
        public SyntaxOrTokenList(params SyntaxOrToken<Lang>[] list) : this(list.AsEnumerable()) { }
        public SyntaxOrTokenList() : this(Array.Empty<SyntaxOrToken<Lang>>()) { }

        public override IReadOnlyList<SyntaxOrToken<Lang>> ChildNodesAndTokens => FullList;
        public          IEnumerator<SyntaxOrToken<Lang>>   GetEnumerator()     => FullList.GetEnumerator();
        IEnumerator IEnumerable.                           GetEnumerator()     => GetEnumerator();
        public int                                         Count               => FullList.Count;

        // indexers
        public SyntaxOrToken<Lang> this[int index] => FullList[index];

        public SyntaxOrTokenList<Lang> Slice(int start, int length) => new(FullList.Skip(start).Take(length));

        public SyntaxOrTokenList<Lang> Splice(int index, int deleteCount, IEnumerable<SyntaxOrToken<Lang>> elements)
            => new(FullList.Splice(index, deleteCount, elements));

        public override string ToString() => base.ToString();

        public override Syntax<Lang> MapWith(Mapper<Lang> mapper) => mapper.Map((dynamic)this);
    }
}
