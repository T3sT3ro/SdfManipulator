#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Preprocessor {
    public record Define : PreprocessorSyntax {
        public DefineKeyword defineKeyword { get; init; } = new();

        // For the functional macros: #define FUNCTIONAL(X) X*X
        public ArgumentList<Identifier>? argList { get; init; } =
            new() { arguments = SeparatedList<Hlsl, Identifier>.Empty };

        public Identifier  id          { get; init; }
        public TokenString tokenString { get; init; }


        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
            { hashToken, defineKeyword, argList, id, tokenString }.FilterNotNull().ToList();
    }
}
