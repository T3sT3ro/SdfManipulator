#nullable enable
using System.Linq;
using API;
using AST.Hlsl;
using AST.Hlsl.Syntax;
using AST.Hlsl.Syntax.Expressions;
using AST.Hlsl.Syntax.Expressions.Operators;
using AST.Syntax;
using Nodes;
using Nodes.SdfNodes;

namespace PortData {
    public record HlslSdfFunction(Call callsyntax) : Port.Data {
        public const  string SdfResultStructName   = "SdfResult";
        private const string SdfIdMemberName       = "id";
        private const string SdfPointMemberName    = "p";
        private const string SdfDistanceMemberName = "distance";
        private const string SdfNormalMemberName   = "normal";
        private const string SdfMaterialMemberName = "material";

        public Call withPosArgument(Expression pointArgument) => callsyntax with
        {
            // TODO: 
            arguments = ArgumentList<HlslSyntax>.Of(
                callsyntax.arguments
                .Splice(0, 0, new IHlslSyntaxOrToken[] { pointArgument, new Comma() })
                )
                
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
