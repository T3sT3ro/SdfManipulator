#nullable enable
using System.Collections.Generic;

namespace AST.Hlsl.Syntax.Type {
    public abstract record Type : AST.Syntax {
        public record Scalar : Type {
            public enum Kind { BOOL, INT, UINT, HALF, FLOAT, DOUBLE }

            public          Kind             kind       { get; set; }
            
            public override List<AST.Syntax> ChildNodes { get; }
        }

        public record Vector : Type {
            public Scalar scalar { get; set; }
            public ushort arity  { get; set; }
        }

        public record Matrix : Type {
            public Scalar scalar { get; set; }
            public ushort rows   { get; set; }
            public ushort cols   { get; set; }
        }

        public record Texture : Type {
            public enum Kind { _1D, _1D_ARRAY, _2D, _2D_ARRAY, _3D, _CUBE, }

            public Kind kind { get; set; }
        }

        public record Struct : Type {
            public          IdentifierName   id         { get; set; }
            public override List<AST.Syntax> ChildNodes => new List<AST.Syntax> { id };
        }
    }
}
