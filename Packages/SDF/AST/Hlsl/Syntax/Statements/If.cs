#nullable enable
using System.Collections.Generic;
using AST.Hlsl.Syntax.Expressions;

namespace AST.Hlsl.Syntax.Statements {
    public record If : Statement, If.Else {
        public Expression                test  { get; set; }
        public IReadOnlyList<Statement>  then  { get; set; }
        public IReadOnlyList<Statement>? @else { get; set; }

        public interface Then { }

        public interface Else { }
    }
}
