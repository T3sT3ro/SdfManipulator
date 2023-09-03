using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements.Declarations;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.NodeGraph.PortData;
using Identifier = me.tooster.sdf.AST.Hlsl.Syntax.Identifier;
using StructMember = me.tooster.sdf.AST.Hlsl.Syntax.Type.Struct.Member;
using MemberAccess = me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators.Member;
using Type = me.tooster.sdf.AST.Hlsl.Syntax.Type;


namespace me.tooster.sdf.Editor.NodeGraph.Nodes.MasterNodes {
    public record VertexInNode : Node {
        public IOutputPort<HlslVector> position { get; }

        public VertexInNode() : base("v_in", "Vertex Input") {
            position = CreateOutput("Vertex position", () => new HlslVector(PositionMemberAccess));
        }

        private const string v2fStructName      = "v_in";
        private const string positionMemberName = "pos";

        // struct v_in { float4 pos : SV_POSITION; };
        public static StructDeclaration VInStructDeclaration => new Type.Struct
        {
            name = v2fStructName,
            members = new[]
            {
                new StructMember
                {
                    type = new VectorToken { arity = 4, type = new FloatKeyword() },
                    id = positionMemberName,
                    semantic =  new PositionSemantic()
                },
            }
        };

        public static MemberAccess PositionMemberAccess => new MemberAccess
        {
            expression = new Identifier { id = v2fStructName },
            member = positionMemberName
        };
    }
}
