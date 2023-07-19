#nullable enable
namespace AST.Hlsl {
    public interface Statement {
        public record Block : Statement {
            public Statement[] statements { get; set; }
        }

        public record Semantic {
            public enum Kind {
                BINORMAL, BLENDINDICES, BLENDWEIGHT, COLOR, NORMAL,
                POSITION, POSITIONT, PSIZE, TANGENT, TEXCOORD,
                FOG, TESSFACTOR, VFACE, VPOS, DEPTH,
            }

            public Kind  kind  { get; set; }
            public uint? index { get; set; }
        }

        public abstract class Variable {
            public record Declaration : Statement, For.Initializer {
                public enum Storage {
                    EXTERN, NOINTERPOLATION, PRECISE, SHARED, GROUPSHARED, STATIC, UNIFORM, VOLATILE
                }

                public enum TypeModifier {
                    CONST, ROW_MAJOR, COLUMN_MAJOR
                }

                public Storage?      storage      { get; set; }
                public TypeModifier? typeModifier { get; set; }
                public Semantic?     semantic     { get; set; }

                public Type        type        { get; set; }
                public Identifier  id          { get; set; }
                public Expression? initializer { get; set; }
            }
        }

        public abstract class Function {
            public record Declaration : Statement {
                public Type       returnType     { get; set; }
                public Identifier id             { get; set; }
                public Argument[] arguments      { get; set; }
                public Semantic?  returnSemantic { get; set; }
                public Block      body           { get; set; }

                public record Argument {
                    public enum Modifier { IN, INOUT, OUT, UNIFORM }

                    public Type       type     { get; set; }
                    public Identifier id       { get; set; }
                    public Semantic?  semantic { get; set; }
                    public Modifier?  modifier { get; set; }
                }
            }
        }

        public abstract class Struct {
            public record Declaration : Statement {
                public Type.Struct type { get; set; }
                public Member[]    members;

                public record Member {
                    public enum Interpolation { LINEAR, CENTROID, NO_INTERPOLATION, NO_PERSPECTIVE, SAMPLE }

                    public Interpolation? interpolation { get; set; }
                    public Type           type          { get; set; }
                    public Identifier     id            { get; set; }
                    public Semantic?      semantic      { get; set; }
                }
            }
        }

        public record Return : Statement {
            public Expression expression { get; set; }
        }

        public record Break : Statement;

        public record Continuue : Statement;

        public record If : Statement {
            public Expression test       { get; set; }
            public Block      consequent { get; set; }
            public Block      alternate  { get; set; }
        }

        public record For : Statement {
            public interface Initializer { }

            public Initializer? initializer { get; set; }
            public Expression   test        { get; set; }
            public Expression?  iteration   { get; set; }
            public Block        body        { get; set; }
        }

        public record While : Statement {
            public Expression test { get; set; }
            public Block      body { get; set; }
        }

        public record DoWhile : Statement {
            public Block      body { get; set; }
            public Expression test { get; set; }
        }

        public record Switch : Statement {
            public Identifier selector { get; set; }
            public Case[]     cases    { get; set; }
            public Block      @default { get; set; }

            public class Case {
                public uint  label { get; set; }
                public Block body  { get; set; }
            }
        }

        public record Discard : Statement { }
    }
}
