#nullable enable
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Statements {
    // for (start conditions; conditions; increments) { body }
    // start conditions:
    //    declarations: int i, j = 0;
    //    initializers: i = 0, j = 0;
    public partial record For : Statement {
        private readonly ForKeyword                       /*_*/forKeyword;
        private readonly OpenParenToken                   /*_*/openParen;
        private readonly Initializer?                     /*_*/initializer;
        private readonly SemiToken                        /*_*/firstSemiToken;
        private readonly Expression?                      /*_*/condition;
        private readonly SemiToken                        /*_*/secondSemiToken;
        private readonly SeparatedList<Hlsl, Expression>? /*_*/increments;
        private readonly CloseParenToken                  /*_*/closeParen;
        private readonly Statement                        /*_*/body;

        public override IReadOnlyList<SyntaxOrToken<Hlsl>> ChildNodesAndTokens => new SyntaxOrToken<Hlsl>?[]
            {
                forKeyword, openParen, initializer, firstSemiToken, condition, secondSemiToken, increments, closeParen,
                body,
            }.FilterNotNull()
            .ToList();

        public abstract partial record Initializer : Syntax<Hlsl>;
    }
}
