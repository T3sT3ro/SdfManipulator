#nullable enable
namespace AST.Hlsl {
    public interface Type {
        public enum Arity { _1, _2, _3, _4 }

        public record Scalar : Type {
            public enum Kind { BOOL, INT, UINT, HALF, FLOAT, DOUBLE }

            public Kind kind { get; set; }
        }

        public record Vector : Type {
            public Scalar scalar { get; set; }
            public Arity  arity  { get; set; }
        }

        public record Array : Type {
            public Type type { get; set; }
            public uint size { get; set; }
        }

        public record Matrix : Type {
            public Scalar scalar { get; set; }
            public Arity  rows   { get; set; }
            public Arity  cols   { get; set; }
        }

        public record Texture : Type {
            public enum Kind { _1D, _2D, _3D, _CUBE, }

            public Kind kind { get; set; }
        }

        public record Struct : Type {
            public Identifier id { get; set; }
        }
    }
}
