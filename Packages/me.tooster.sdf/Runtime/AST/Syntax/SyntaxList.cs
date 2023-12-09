using System.Collections.Generic;
using System.Linq;

namespace me.tooster.sdf.AST.Syntax {
    /// <summary>
    /// List containing only syntax of specific type
    /// </summary>
    /// <typeparam name="Lang">see <see cref="Syntax{Lang}"/></typeparam>
    /// <typeparam name="TSyntax"></typeparam>
    public record SyntaxList<Lang, TSyntax> : SyntaxOrTokenList<Lang>, IReadOnlyList<TSyntax>
        where TSyntax : Syntax<Lang> {
        public SyntaxList(IReadOnlyList<TSyntax> fullList) : base(fullList.Cast<SyntaxOrToken<Lang>>()) { }
        public SyntaxList(IEnumerable<TSyntax> list) : base(list) { }
        public SyntaxList(params TSyntax[] list) : this(list.AsEnumerable()) { }

        public SyntaxList() { }

        public new TSyntax this[int index] => (TSyntax)base[index];

        public new SyntaxList<Lang, TSyntax> Slice(int start, int length) =>
            new(FullList.Skip(start).Take(length).Cast<TSyntax>());

        public new SyntaxList<Lang, TSyntax> Splice
            (int index, int deleteCount, IEnumerable<SyntaxOrToken<Lang>> elements) =>
            new(FullList.Splice(index, deleteCount, elements).Cast<TSyntax>());

        public static implicit operator SyntaxList<Lang, TSyntax>(TSyntax[] list)     => new(list);
        public static implicit operator SyntaxList<Lang, TSyntax>(List<TSyntax> list) => new(list.AsEnumerable());

        public override string               ToString()      => base.ToString();
        public new      IEnumerator<TSyntax> GetEnumerator() => FullList.Cast<TSyntax>().GetEnumerator();
    }
}
