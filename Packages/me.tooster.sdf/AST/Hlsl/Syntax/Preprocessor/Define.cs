#nullable enable
using System.Collections.Generic;
using System.Linq;
using AST.Syntax;

namespace AST.Hlsl.Syntax.Preprocessor {
    public partial record Define : PreprocessorSyntax {
        private readonly DefineKeyword _defineKeyword;
        private readonly ArgumentList<Identifier>? _argList;

        private readonly Identifier  _id;
        private readonly TokenString _tokenString;


        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
            { hashToken, defineKeyword, argList, id, tokenString }.FilterNotNull().ToList();
    }
}
