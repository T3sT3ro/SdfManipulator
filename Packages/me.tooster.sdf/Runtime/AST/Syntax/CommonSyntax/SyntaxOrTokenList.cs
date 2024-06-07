#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace me.tooster.sdf.AST.Syntax.CommonSyntax {
    /// <summary>
    /// List containing both syntax and tokens of specific language
    /// </summary>
    public record SyntaxOrTokenList<Lang> : Syntax<Lang>, IReadOnlyList<SyntaxOrToken<Lang>> {
        /// full list with syntax and tokens
        public IReadOnlyList<SyntaxOrToken<Lang>> FullList { get; } = Array.Empty<SyntaxOrToken<Lang>>();

        public SyntaxOrTokenList(IReadOnlyList<SyntaxOrToken<Lang>> fullList) {
#if DEBUG
            if (fullList.Any(s => s is null))
                throw new NullReferenceException("Attempted to create a SyntaxOrTokenList with nulls");
#endif
            FullList = fullList.ToList();
        }

        public static readonly SyntaxOrTokenList<Lang> Empty = new();

        public SyntaxOrTokenList(IEnumerable<SyntaxOrToken<Lang>> list) : this(list.ToList()) { }
        public SyntaxOrTokenList(params SyntaxOrToken<Lang>[] list) : this(list.ToList()) { }
        public SyntaxOrTokenList() { }

        public override IReadOnlyList<SyntaxOrToken<Lang>> ChildNodesAndTokens() => FullList;
        public virtual  IEnumerator<SyntaxOrToken<Lang>>   GetEnumerator()       => FullList.GetEnumerator();
        IEnumerator IEnumerable.                           GetEnumerator()       => GetEnumerator();
        public virtual int                                 Count                 => FullList.Count;

        // indexers
        public virtual SyntaxOrToken<Lang> this[int index] => FullList[index];

        // IMPORTANT: UNCOMMENTING BELOW OPERATORS CRASHES UNITY WITH NATIVE CODE EXCEPTION AND CRASHES IT WHEN TRYING TO COMPILE CODE
        // utility casts for trivias. Possibly could be migrated to typed Token list?
        // public static implicit operator SyntaxOrTokenList<Lang>(Token<Lang> singleToken)        => new((SyntaxOrToken<Lang>)singleToken);
        // public static implicit operator SyntaxOrTokenList<Lang>(Token<Lang>[] tokens)           => new(tokens.ToList());
        // public static implicit operator SyntaxOrTokenList<Lang>(List<SyntaxOrToken<Lang>> list) => new(list.AsEnumerable());

        public SyntaxOrTokenList<Lang> Slice(int start, int length) => new(FullList.Skip(start).Take(length));

        /// <inheritdoc cref="Extensions.Splice{T}(IEnumerable{T}, int, int, IEnumerable{T})"/>
        public virtual SyntaxOrTokenList<Lang> Splice(int index, int deleteCount, IEnumerable<SyntaxOrToken<Lang>> elements)
            => new(FullList.Splice(index, deleteCount, elements));

        internal override void Accept(Visitor<Lang> visitor, Anchor? parent) => visitor.Visit(Anchor.New(this, parent));

        internal override TR? Accept<TR>(Visitor<Lang, TR> visitor, Anchor? parent) where TR : default
            => visitor.Visit(Anchor.New(this, parent));

        public override string ToString() => WriteTo(new StringBuilder()).ToString();
    }
}
