using API;
using AST.Hlsl.Syntax;
using AST.Hlsl.Syntax.Statements.Declarations;
using PortData;
using StructMember = AST.Hlsl.Syntax.Type.Struct.Member;
using MemberAccess = AST.Hlsl.Syntax.Expressions.Operators.Member;


namespace Nodes.MasterNodes {
    public record VertexInNode : Node {
        public OutputPort<HlslVector> position { get; }

        public VertexInNode() : base("v_in", "Vertex Input") {
            position = this.CreateOutput("Vertex position", () => new HlslVector(PositionMemberAccess));
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
