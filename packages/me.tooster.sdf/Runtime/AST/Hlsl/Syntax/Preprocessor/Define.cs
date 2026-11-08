#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;
namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    /// <summary>
    ///     <c>#define id token-string</c> or a function-like <c>#define id(arg, ...) token-string</c>. The argument list and
    ///     the token string are optional.
    /// </summary>
    [SyntaxNode] public partial record Define : PreprocessorSyntax {
        // Declaration order is syntax order.
        public DefineKeyword             defineKeyword { get; init; } = new();
        public Identifier                id            { get; init; }
        public ArgumentList<Identifier>? argList       { get; init; }
        public TokenString?              tokenString   { get; init; }

        public override IReadOnlyList<SyntaxOrToken<hlsl>> ChildNodesAndTokens()
            => new SyntaxOrToken<hlsl>?[]
            {
                hashToken,
                defineKeyword,
                id,
                argList,
                tokenString,
                endOfDirectiveToken,
            }.Where(c => c is not null).Select(c => c!).ToList();
    }
}
