using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Statements.Declarations {
    public abstract partial record FunctionDeclaration : Statement {
        public Type        returnType     { get; set; }
        public IdentifierName       id             { get; set; }
        public Argument[]       arguments      { get; set; }
        public Semantic?        returnSemantic { get; set; }
        public List<Statement>? body           { get; set; }
    }
}