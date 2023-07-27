#nullable enable
namespace AST.Hlsl.Syntax.Statements.Declarations {
    public abstract record StructDeclaration : Statement {
        public Type.Struct type { get; set; }
        public Member[]         members;

        public record Member {
            public enum Interpolation { LINEAR, CENTROID, NOINTERPOLATION, NOPERSPECTIVE, SAMPLE }

            public Interpolation? interpolation { get; set; }
            public Type      type          { get; set; }
            public IdentifierName     id            { get; set; }
            public Semantic?      semantic      { get; set; }
        }
    }
}