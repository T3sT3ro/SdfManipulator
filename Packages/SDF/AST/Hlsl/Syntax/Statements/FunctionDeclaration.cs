using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Statements {
    public abstract partial record FunctionDeclaration : Statement {
        public Type.Type        returnType     { get; set; }
        public IdentifierName       id             { get; set; }
        public Argument[]       arguments      { get; set; }
        public Semantic?        returnSemantic { get; set; }
        public List<Statement>? body           { get; set; }
    }
}