using AST.Hlsl.Syntax.Expressions.Literals;

namespace AST.Hlsl.Syntax.Statements {
    public abstract partial record FunctionDeclaration {
        public record Argument : AST.Syntax {
            public enum Modifier { IN, INOUT, OUT, UNIFORM }

            public Modifier?  modifier { get; set; }
            public Type.Type  type     { get; set; }
            public IdentifierName id       { get; set; }
            public Semantic?  semantic { get; set; }

            public Literal? initializer { get; set; }
        }
    }
}
