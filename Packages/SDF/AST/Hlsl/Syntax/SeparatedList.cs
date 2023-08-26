using System;
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax {
    using HlslSeparatedList = ISeparatedSyntaxList<HlslSyntax, HlslToken, IHlslSyntaxOrToken>;
    using IHlslSyntaxOrTokenList = ISyntaxOrTokenList<HlslSyntax, IHlslSyntaxOrToken, HlslList>;

    public record HlslList(IEnumerable<IHlslSyntaxOrToken> All)
        : HlslSyntax, IHlslSyntaxOrTokenList {
        public HlslList() : this(Array.Empty<IHlslSyntaxOrToken>()) { }
        IReadOnlyList<IHlslSyntaxOrToken> IHlslSyntaxOrTokenList.All { get; init; } = All.ToList();

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => (IReadOnlyList<IHlslSyntaxOrToken>)All;

        // required implementations for generation of indexers and accessing methods without casting to interface
        public int Count => ((IHlslSyntaxOrTokenList)this).Count;
        public IHlslSyntaxOrToken this[int index] => ((IReadOnlyList<IHlslSyntaxOrToken>)All)[index];
        public HlslList Slice(int          start, int length) => ((IHlslSyntaxOrTokenList)this).Slice(start, length);
        public HlslList ToSpliced(int index, int deleteCount, IEnumerable<IHlslSyntaxOrToken> elements)
            => ((IHlslSyntaxOrTokenList)this).ToSpliced(index, deleteCount, elements);
    }

    // TODO: remove
    class Test {
        private void test() {
            HlslList list = new HlslList(new IHlslSyntaxOrToken[] { });
            var x = list[^1];
            var y = list[1..2];
        }
    }


// SYNTAX TOKEN SYNTAX TOKEN SYNTAX ...
    public record SeparatedList<TSyntax>(IEnumerable<IHlslSyntaxOrToken> ListWithSeparators)
        : HlslSyntax, HlslSeparatedList
        where TSyntax : HlslSyntax {
        IReadOnlyList<IHlslSyntaxOrToken> ISeparatedSyntaxList<HlslSyntax, HlslToken, IHlslSyntaxOrToken>.
            FullList { get; } = ListWithSeparators.ToList();

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens => ((HlslSeparatedList)this).FullList;

        public HlslSyntax this[int index] => ((ISeparatedSyntaxList<HlslSyntax, HlslToken, IHlslSyntaxOrToken>)this)[1];

        public static SeparatedList<TSyntax> SeparatedWith<TTok>(IEnumerable<TSyntax> nodes)
            where TTok : HlslToken, new() {
            var commaSeparatedList = new List<IHlslSyntaxOrToken>();
            bool skipComma = true;
            foreach (var node in nodes) {
                commaSeparatedList.Add(node);
                if (skipComma) skipComma = false;
                else commaSeparatedList.Add(new TTok());
            }

            return new(commaSeparatedList);
        }
    }

    // ( SYNTAX TOKEN SYNTAX TOKEN ... )
    public record ArgumentList<TSyntax>(IEnumerable<IHlslSyntaxOrToken> ListWithSeparators)
        : SeparatedList<TSyntax>(ListWithSeparators)
        where TSyntax : HlslSyntax {
        public OpenParenToken  openParenToken  { get; init; } = new OpenParenToken();
        public CloseParenToken closeParenToken { get; init; } = new CloseParenToken();

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens =>
            ListWithSeparators.Prepend(openParenToken).Append(closeParenToken).ToList();

        public static ArgumentList<TSyntax> Empty() => new(new List<IHlslSyntaxOrToken>());

        public static ArgumentList<TSyntax> Of(IEnumerable<TSyntax> syntaxList) =>
            new ArgumentList<TSyntax>(SeparatedWith<CommaToken>(syntaxList));

        public static implicit operator ArgumentList<TSyntax>(TSyntax[] arguments) => Of(arguments);
    }

    // { SYNTAX TOKEN SYNTAX TOKEN ... }
    public record BracketInitializerList<TSyntax>(IEnumerable<IHlslSyntaxOrToken> ListWithSeparators)
        : SeparatedList<TSyntax>(ListWithSeparators)
        where TSyntax : HlslSyntax {
        public OpenBraceToken  openBraceToken  { get; init; } = new OpenBraceToken();
        public CloseBraceToken closeBraceToken { get; init; } = new CloseBraceToken();

        public override IReadOnlyList<IHlslSyntaxOrToken> ChildNodesAndTokens =>
            ListWithSeparators.Prepend(openBraceToken).Append(closeBraceToken).ToList();

        public static BracketInitializerList<TSyntax> Empty() => new(new List<IHlslSyntaxOrToken>());

        public static BracketInitializerList<TSyntax> Of(IEnumerable<TSyntax> syntaxList) =>
            new BracketInitializerList<TSyntax>(SeparatedWith<CommaToken>(syntaxList));

        public static implicit operator BracketInitializerList<TSyntax>(TSyntax[] arguments) => Of(arguments);
    }
}
