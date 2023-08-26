#nullable enable
using API;
using AST.Hlsl.Syntax;
using AST.Hlsl.Syntax.Expressions;
using AST.Hlsl.Syntax.Expressions.Operators;
using AST.Hlsl.Syntax.Statements;
using AST.Hlsl.Syntax.Statements.Declarations;
using PortData;
using StructMember = AST.Hlsl.Syntax.Type.Struct.Member;
using AccessMember = AST.Hlsl.Syntax.Expressions.Operators.Member;

namespace Assets.Nodes.MasterNodes {
    // TODO dynamic interpolators using InOutPorts and their lists
    public record BasicVertToFragNode : Node {
        public InOutPort<HlslVector> position { get; init; }
        public InOutPort<HlslVector> normal   { get; init; }
        public InOutPort<HlslVector> texCoord { get; init; }
        public InOutPort<HlslVector> color    { get; init; }

        public BasicVertToFragNode(
            IOutputPort<HlslVector>? position,
            IOutputPort<HlslVector>? normal,
            IOutputPort<HlslVector>? texCoord,
            IOutputPort<HlslVector>? color
        ) : base(v2fStructName, "V2F basic") {
            this.position = CreateInOut("Vertex position", position ?? HlslVector.DefaultNode().Value,
                _ => new HlslVector(PositionMember));
            this.normal = CreateInOut("Vertex normal", normal ?? HlslVector.DefaultNode().Value,
                _ => new HlslVector(NormalMember));
            this.texCoord = CreateInOut("Vertex texture coordinate",
                texCoord ?? HlslVector.DefaultNode().Value, _ => new HlslVector(TexCoordMember));
            this.color = CreateInOut("Vertex color",
                color ?? HlslVector.DefaultNode().Value, _ => new HlslVector(ColorMember));
        }

        public const string v2fStructName  = "v2f";
        public const string vertOutVarName = "vertOut";
        public const string fragInArgName  = "fragIn";

        private const string positionMemberName = "pos";
        private const string normalMemberName   = "normal";
        private const string texCoordMemberName = "texCoord";
        private const string colorMemberName    = "color";

        // struct v2f { ... };
        public static StructDeclaration VertToFragStructDeclaration => new Type.Struct
        {
            name = v2fStructName,
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
                type = v2fStructName, variables = new SeparatedList<VariableDeclarator.VariableDefinition>(new[]
                {
                    new VariableDeclarator.VariableDefinition { id = vertOutVarName }
                })
            },
        };

        // vertOut.{member} = {valueExpression};
        private static ExpressionStatement MemberAssignment(string memberName, Expression ValueExpression) =>
            new ExpressionStatement
            {
                expression = new AssignmentExpresion
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
            MemberAssignment(texCoordMemberName, texCoord.Eval().vectorExpression), // vertOut.texCoord = {input};
            MemberAssignment(colorMemberName, color.Eval().vectorExpression),       // vertOut.color = {input};
            new Return { expression = new Identifier { id = vertOutVarName } }      // return vertOut;
        };

        private static AccessMember MemberAccessor(string memberName) => new AccessMember { member = memberName };

        public static AccessMember PositionMember => MemberAccessor(positionMemberName);
        public static AccessMember NormalMember   => MemberAccessor(normalMemberName);
        public static AccessMember TexCoordMember => MemberAccessor(texCoordMemberName);
        public static AccessMember ColorMember    => MemberAccessor(colorMemberName);
    }
}
