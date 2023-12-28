#nullable enable
using System.Collections.Generic;
using System.Collections.Specialized;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements;
using me.tooster.sdf.AST.Hlsl.Syntax.Statements.Declarations;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.NodeGraph.PortData;
using static me.tooster.sdf.AST.Hlsl.Syntax.VariableDeclarator;
using Identifier = me.tooster.sdf.AST.Hlsl.Syntax.Identifier;
using StructMember = me.tooster.sdf.AST.Hlsl.Syntax.Type.Struct.Member;
using MemberAccess = me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators.Member;
using Type = me.tooster.sdf.AST.Hlsl.Syntax.Type;
using VariableDeclarator = me.tooster.sdf.AST.Hlsl.Syntax.VariableDeclarator;

namespace me.tooster.sdf.Editor.NodeGraph.Nodes.MasterNodes {
    // TODO dynamic interpolators using InOutPorts and their lists
    public record BasicVertToFragNode : Node, IVertexToFragmentNode {
        
        public InOutPort<HlslVector> position { get; }
        public InOutPort<HlslVector> normal   { get; }
        public InOutPort<HlslVector> uv       { get; }
        public InOutPort<HlslVector> color    { get; }

        public BasicVertToFragNode(
            IOutputPort<HlslVector>? position,
            IOutputPort<HlslVector>? normal,
            IOutputPort<HlslVector>? uv,
            IOutputPort<HlslVector>? color
        ) : base(v2fStructTypeName, "V2F basic") {
            this.position = CreateInOut("Vertex position", position ?? HlslVector.DefaultNode().Value,
                _ => new HlslVector(PositionMember));
            this.normal = CreateInOut("Vertex normal", normal ?? HlslVector.DefaultNode().Value,
                _ => new HlslVector(NormalMember));
            this.uv = CreateInOut("Vertex UV coordinate",
                uv ?? HlslVector.DefaultNode().Value, _ => new HlslVector(TexCoordMember));
            this.color = CreateInOut("Vertex color",
                color ?? HlslVector.DefaultNode().Value, _ => new HlslVector(ColorMember));
        }

        public const string v2fStructTypeName = "v2f";
        public const string vertOutVarName    = "fragment";
        public const string fragInArgName     = "fragment";

        public const string positionMemberName = "pos";
        public const string normalMemberName   = "normal";
        public const string texCoordMemberName = "texCoord";
        public const string colorMemberName    = "color";

        // struct v2f { ... };
        public Type.Struct VertexToFragmentStruct => new Type.Struct
        {
            name = v2fStructTypeName,
            members = new[]
            {
                new StructMember
                {
                    type = new VectorToken { arity = 4, type = new FloatKeyword() },
                    id = positionMemberName,
                    semantic = new SvPositionSemantic()
                },
                new StructMember
                {
                    type = new VectorToken { arity = 4, type = new FloatKeyword() },
                    id = normalMemberName,
                    semantic = new NormalSemantic()
                },
                new StructMember
                {
                    type = new VectorToken { arity = 2, type = new FloatKeyword() },
                    id = texCoordMemberName,
                    semantic = new TexcoordSemantic { n = 0 }
                },
                new StructMember
                {
                    type = new VectorToken { arity = 4, type = new FloatKeyword() },
                    id = colorMemberName,
                    semantic = new ColorSemantic { n = 0 }
                },
            }
        };

        // v2f vertOut;
        private static VariableDeclaration VertToFragVariableDeclaration => new VariableDeclaration
        {
            declarator = new VariableDeclarator
            {
                type = v2fStructTypeName,
                variables = new[] { new VariableDefinition { id = vertOutVarName } }.CommaSeparated()
            },
        };

        // vertOut.{member} = {valueExpression};
        private static ExpressionStatement MemberAssignment(string memberName, Expression ValueExpression) =>
            new ExpressionStatement
            {
                expression = new AssignmentExpression
                {
                    left = MemberAccessor(memberName) with { expression = new Identifier { id = fragInArgName } },
                    right = ValueExpression
                }
            };

        /// returns statement used at the end of the vertex shader to return the data 
        public Statement[] VertOutputReturn => new Statement[]
        {
            VertToFragVariableDeclaration,                                          // v2f vertOut;
            MemberAssignment(positionMemberName, position.Eval().vectorExpression), // vertOut.pos = {input};
            MemberAssignment(normalMemberName, normal.Eval().vectorExpression),     // vertOut.normal = {input};
            MemberAssignment(texCoordMemberName, uv.Eval().vectorExpression), // vertOut.texCoord = {input};
            MemberAssignment(colorMemberName, color.Eval().vectorExpression),       // vertOut.color = {input};
            new Return { expression = new Identifier { id = vertOutVarName } }      // return vertOut;
        };

        private static MemberAccess MemberAccessor(string memberName) => new MemberAccess { member = memberName };

        public static MemberAccess PositionMember => MemberAccessor(positionMemberName);
        public static MemberAccess NormalMember   => MemberAccessor(normalMemberName);
        public static MemberAccess TexCoordMember => MemberAccessor(texCoordMemberName);
        public static MemberAccess ColorMember    => MemberAccessor(colorMemberName);
    }
}
