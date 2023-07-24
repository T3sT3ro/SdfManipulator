using System.Collections.Generic;
using AST.Hlsl.Syntax.Expressions;

namespace AST.Hlsl.Syntax.Statements {
    public record For : Statement {
        public interface Initializer { }

        public Initializer?             initializer { get; set; }
        public Expression               test        { get; set; }
        public Expression?              iteration   { get; set; }
        public IReadOnlyList<Statement> body        { get; set; }
    }
}