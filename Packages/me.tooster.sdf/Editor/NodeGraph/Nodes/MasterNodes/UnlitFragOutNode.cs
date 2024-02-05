#nullable enable
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements.Declarations;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.NodeGraph.PortData;
using Identifier = me.tooster.sdf.AST.Hlsl.Syntax.Identifier;
using StructMember = me.tooster.sdf.AST.Hlsl.Syntax.Type.Struct.Member;
using MemberAccess = me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators.Member;
using Type = me.tooster.sdf.AST.Hlsl.Syntax.Type;

namespace me.tooster.sdf.Editor.NodeGraph.Nodes.MasterNodes {
    public record UnlitFragOutNode : Node, IFragmentOutNode {
        public IInputPort<HlslVector> color { get; }

        public UnlitFragOutNode(IOutputPort<HlslVector>? color)
            : base("frag_out", "Fragment Output") {
            this.color = CreateInput("Color", color ?? HlslVector.DefaultNode().Value);
        }

        public const string FragOutStructName = "f_out";

        private const string colorMemberName  = "color";
        private const string normalMemberName = "normal";
        private const string idMemberName     = "id";

        public Type.Struct FragmentOutStruct => new Type.Struct
        {
            name = FragOutStructName,
            members = new[]
            {
                new StructMember
                {
                    type = new VectorTypeToken(),
                    id = colorMemberName,
                    semantic = new ColorSemantic()
                },
                new StructMember
                {
                    type = new VectorTypeToken(),
                    id = normalMemberName,
                    semantic = new NormalSemantic()
                },
                new StructMember
                {
                    type = new VectorTypeToken(),
                    id = idMemberName,
                    semantic = new SvTargetSemantic { n = 0 }
                }
            }
        };
        
        public static MemberAccess ColorMemberAccess  => new MemberAccess { member = colorMemberName };
        

    }
}
