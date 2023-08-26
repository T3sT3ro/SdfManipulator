#nullable enable
using API;
using AST.Hlsl.Syntax;
using AST.Hlsl.Syntax.Statements.Declarations;
using PortData;
using StructMember = AST.Hlsl.Syntax.Type.Struct.Member;

namespace Assets.Nodes.MasterNodes {
    public record UnlitFragOutNode : Node {
        public IInputPort<HlslVector> color { get; }

        public UnlitFragOutNode(IOutputPort<HlslVector>? color)
            : base("frag_out", "Fragment Output") {
            this.color = CreateInput("Color", color ?? HlslVector.DefaultNode().Value);
        }

        public const string FragOutStructName = "f_out";

        private const string colorMemberName  = "color";
        private const string normalMemberName = "normal";
        private const string idMemberName     = "id";

        public static StructDeclaration FOutStructDeclaration => new Type.Struct
        {
            name = FragOutStructName,
            members = new[]
            {
                new StructMember
                {
                    type = new VectorToken { arity = 4, type = new FloatKeyword() },
                    id = colorMemberName,
                    semantic = new ColorSemantic()
                },
                new StructMember
                {
                    type = new VectorToken { arity = 4, type = new FloatKeyword() },
                    id = normalMemberName,
                    semantic = new NormalSemantic()
                },
                new StructMember
                {
                    type = new VectorToken { arity = 4, type = new FloatKeyword() },
                    id = idMemberName,
                    semantic = new SvTargetSemantic { n = 0 }
                }
            }
        };
    }
}
