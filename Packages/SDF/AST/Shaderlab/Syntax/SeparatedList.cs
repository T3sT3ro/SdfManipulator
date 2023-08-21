using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Shaderlab.Syntax {
    public record SeparatedList<TSyntax>(IReadOnlyList<IShaderlabSyntaxOrToken> ListWithSeparators)
        : ShaderlabSyntax, ISeparatedSyntaxList<ShaderlabSyntax, ShaderlabToken, IShaderlabSyntaxOrToken>
        where TSyntax : ShaderlabSyntax {
        IReadOnlyList<IShaderlabSyntaxOrToken>
            ISeparatedSyntaxList<ShaderlabSyntax, ShaderlabToken, IShaderlabSyntaxOrToken>.FullList { get; }
            = ListWithSeparators.ToList();

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens => ListWithSeparators;

        public static SeparatedList<TSyntax> SeparatedWith<TTok>(IEnumerable<TSyntax> nodes)
            where TTok : ShaderlabToken, new() {
            var commaSeparatedList = new List<IShaderlabSyntaxOrToken> { nodes.First() };
            foreach (var syntax in nodes.Skip(1)) {
                commaSeparatedList.Add(new TTok());
                commaSeparatedList.Add(syntax);
            }

            return new(commaSeparatedList);
        }
    }

    // ( SYNTAX TOKEN SYNTAX TOKEN )
    public record ArgumentList<TSyntax>(IReadOnlyList<IShaderlabSyntaxOrToken> ListWithSeparators)
        : SeparatedList<TSyntax>(ListWithSeparators) where TSyntax : ShaderlabSyntax {
        public ShaderlabToken         openParenToken  { get; set; }
        public ShaderlabToken         closeParenToken { get; set; }

        public override IReadOnlyList<IShaderlabSyntaxOrToken> ChildNodesAndTokens =>
            ListWithSeparators.Prepend(openParenToken).Append(closeParenToken).ToList();

        public static ArgumentList<TSyntax> Empty() => new(new List<IShaderlabSyntaxOrToken>());

        public static implicit operator ArgumentList<TSyntax>(TSyntax[] arguments) =>
            new ArgumentList<TSyntax>(SeparatedList<TSyntax>.SeparatedWith<CommaToken>(arguments));
    }
}
