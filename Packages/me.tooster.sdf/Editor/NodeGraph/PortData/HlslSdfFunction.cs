#nullable enable
using System.Linq;
using me.tooster.sdf.AST;
using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.AST.Syntax;
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.NodeGraph.Nodes;
using me.tooster.sdf.Editor.NodeGraph.Nodes.SdfNodes;
using Type = me.tooster.sdf.AST.Hlsl.Syntax.Type;

namespace me.tooster.sdf.Editor.NodeGraph.PortData {
    public record HlslSdfFunction(Call callsyntax) : Port.Data {
        public const  string SdfResultStructName   = "SdfResult";
        private const string SdfIdMemberName       = "id";
        private const string SdfPointMemberName    = "p";
        private const string SdfDistanceMemberName = "distance";
        private const string SdfNormalMemberName   = "normal";
        private const string SdfMaterialMemberName = "material";

        public Call withPosArgument(Expression pointArgument) => callsyntax with
        {
            argList = new AST.Hlsl.Syntax.ArgumentList<Syntax<hlsl>>
            {
                arguments = new SeparatedList<hlsl, Syntax<hlsl>>(
                    new SyntaxOrToken<hlsl>[] { pointArgument, new CommaToken() }.Concat(callsyntax.argList.arguments)
                )
            }
        };

        public static Type.Struct SdfResultStructShape => new Type.Struct
        {
            name = SdfResultStructName,
            members = new[]
            {
                new Type.Struct.Member { type = new IntKeyword(), id = SdfIdMemberName },
                new Type.Struct.Member
                {
                    type = new VectorToken { type = new FloatKeyword(), arity = 3 },
                    id = SdfPointMemberName
                },
                new Type.Struct.Member { type = new FloatKeyword(), id = SdfDistanceMemberName },
                new Type.Struct.Member
                {
                    type = new VectorToken { type = new FixedKeyword(), arity = 3 },
                    id = SdfNormalMemberName
                },
                new Type.Struct.Member
                    { type = HlslMaterial.MaterialStructName, id = SdfMaterialMemberName },
            }
        };

        public static Member idAccessor       => new Member { member = SdfIdMemberName };
        public static Member pointAccessor    => new Member { member = SdfPointMemberName };
        public static Member distanceAccessor => new Member { member = SdfDistanceMemberName };
        public static Member normalAccessor   => new Member { member = SdfNormalMemberName };
        public static Member materialAccessor => new Member { member = SdfMaterialMemberName };

        public static SdfNode DefaultNode() => new SdfSphereNode(null, null);
    }
}
