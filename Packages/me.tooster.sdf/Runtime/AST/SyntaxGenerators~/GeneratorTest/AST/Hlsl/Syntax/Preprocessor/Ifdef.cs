using System.Collections.Generic;
using me.tooster.sdf.AST.Syntax;

namespace me.tooster.sdf.AST.Hlsl.Syntax.Preprocessor {
    [AstSyntax] public partial record Ifdef : PreprocessorSyntax {
        public IfdefKeyword ifdefKeyword { get; init; } = new();
        public Identifier   id           { get; init; }
    }
}
