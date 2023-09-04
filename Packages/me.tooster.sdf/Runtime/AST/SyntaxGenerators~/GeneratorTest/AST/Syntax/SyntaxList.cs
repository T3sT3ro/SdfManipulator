using System.Collections.Generic;
using System.Linq;

namespace me.tooster.sdf.AST.Syntax {
    /// <summary>
    /// List containing only syntax of specific type
    /// </summary>
    /// <typeparam name="Lang">see <see cref="Syntax{Lang}"/></typeparam>
    /// <typeparam name="TSyntax"></typeparam>
    public record SyntaxList<Lang, TSyntax> : SyntaxOrTokenList<Lang> where TSyntax : Syntax<Lang> {
        public SyntaxList(IEnumerable<TSyntax> list) : base(list) { }
        public SyntaxList(params TSyntax[]     list) : this(list.AsEnumerable()) { }

        public SyntaxList() : base() {}

        new public TSyntax this[int index] => (TSyntax)base[index];

        new public SyntaxList<Lang, TSyntax> Slice(int start, int length) =>
            new(FullList.Skip(start).Take(length).Cast<TSyntax>());

        new public SyntaxList<Lang, TSyntax> Splice
            (int index, int deleteCount, IEnumerable<SyntaxOrToken<Lang>> elements) =>
            new(FullList.Splice(index, deleteCount, elements).Cast<TSyntax>());

        public static implicit operator SyntaxList<Lang, TSyntax>(TSyntax[]     list) => new(list);
        public static implicit operator SyntaxList<Lang, TSyntax>(List<TSyntax> list) => new(list.AsEnumerable());

        public override string ToString() => base.ToString();
    }
}
