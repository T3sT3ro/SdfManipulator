#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    public partial record Define : PreprocessorSyntax {
        private readonly DefineKeyword             /*_*/defineKeyword;
        private readonly ArgumentList<Identifier>? /*_*/argList;
        private readonly Identifier                /*_*/id;
        private readonly TokenString               /*_*/tokenString;


        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
            { hashToken, defineKeyword, argList, id, tokenString }.FilterNotNull().ToList();
    }
}
