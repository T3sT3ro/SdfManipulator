using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements.Declarations;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.NodeGraph.PortData;
using Identifier = me.tooster.sdf.AST.Hlsl.Syntax.Identifier;
using StructMember = me.tooster.sdf.AST.Hlsl.Syntax.Type.Struct.Member;
using MemberAccess = me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators.Member;
using Type = me.tooster.sdf.AST.Hlsl.Syntax.Type;


namespace me.tooster.sdf.Editor.NodeGraph.Nodes.MasterNodes {
    public record VertexInNode : Node, IVertexInputNode {
        public IOutputPort<HlslVector> position { get; }
        public IOutputPort<HlslVector> normal   { get; }
        public IOutputPort<HlslVector> uv       { get; }

        public VertexInNode() : base("v_in", "Vertex Input") {
            position = CreateOutput("Vertex position", () => new HlslVector(PositionMemberAccess));
            normal = CreateOutput("Vertex normal", () => new HlslVector(NormalMemberAccess));
            uv = CreateOutput("Vertex uv", () => new HlslVector(UVMemberAccess));
        }

        public record VertexInputData(string name, StructMember syntax);

        private const string positionMemberName = "pos";
        private const string normalMemberName   = "normal";
        private const string uvMemberName       = "uv";
        private const string hitposMemberName   = "hitpos";
        private const string rdCamMemberName    = "rd_cam";

        private static MemberAccess PositionMemberAccess => new MemberAccess { member = positionMemberName };
        private static MemberAccess NormalMemberAccess   => new MemberAccess { member = normalMemberName };
        private static MemberAccess UVMemberAccess       => new MemberAccess { member = uvMemberName };
        private static MemberAccess HitposMemberAccess   => new MemberAccess { member = hitposMemberName };
        private static MemberAccess RDCamMemberAccess    => new MemberAccess { member = rdCamMemberName };

        // struct v_in { float4 pos : SV_POSITION; ... };
        public Type.Struct VertexInputStruct => new Type.Struct
        {
            name = "vertex",
            members = new[]
            {
                new Type.Struct.Member
                {
                    type = new VectorToken { arity = 4, type = new FloatKeyword() },
                    id = positionMemberName,
                    semantic = new PositionSemantic()
                },
                new Type.Struct.Member
                {
                    type = new VectorToken { arity = 3, type = new FloatKeyword() },
                    id = normalMemberName,
                    semantic = new NormalSemantic()
                },
                new Type.Struct.Member
                {
                    type = new VectorToken { arity = 2, type = new FloatKeyword() },
                    id = uvMemberName,
                    semantic = new TexcoordSemantic { n = 0 }
                },
                new Type.Struct.Member
                {
                    type = new VectorToken { arity = 3, type = new FloatKeyword() },
                    id = hitposMemberName,
                    semantic = new TexcoordSemantic { n = 1 }
                },
                new Type.Struct.Member
                {
                    type = new VectorToken { arity = 3, type = new FloatKeyword() },
                    id = rdCamMemberName,
                    semantic = new TexcoordSemantic { n = 2 }
                }
            }
        };
    }
}
