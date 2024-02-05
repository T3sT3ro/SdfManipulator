using me.tooster.sdf.AST.Hlsl;
using me.tooster.sdf.AST.Hlsl.Syntax;
using me.tooster.sdf.AST.Hlsl.Syntax.Expressions.Operators;
using me.tooster.sdf.Editor.API;
using Type = me.tooster.sdf.AST.Hlsl.Syntax.Type;

namespace me.tooster.sdf.Editor.NodeGraph.PortData {
    
// TODO: add support for automatic port type adapters, e.g. HlslVector -> HlslScalar, using virtual InOutPorts

// TODO: think about handling things like vector literal (float4(0,0,0,0)) vs using a variable holding that value

    public record HlslMaterial : Port.Data {
        public const  string MaterialStructName           = "Material";
        private const string MaterialAlbedoMemberName     = "albedo";
        private const string MaterialMetallicMemberName   = "metallic";
        private const string MaterialSmoothnessMemberName = "smoothness";
        private const string MaterialEmissionMemberName   = "emission";
        private const string MaterialOcclusionMemberName  = "occlusion";

        public Type.Struct StructDeclaration => new Type.Struct
        {
            name = MaterialStructName,
            members = new[]
            {
                new Type.Struct.Member
                {
                    type = new VectorTypeToken { type = Constants.ScalarKind.@fixed },
                    id = MaterialAlbedoMemberName
                },
                new Type.Struct.Member { type = new HalfKeyword(), id = MaterialMetallicMemberName },
                new Type.Struct.Member { type = new HalfKeyword(), id = MaterialSmoothnessMemberName },
                new Type.Struct.Member
                {
                    type = new VectorTypeToken { type = Constants.ScalarKind.half, arity = 3 },
                    id = MaterialEmissionMemberName
                },
                new Type.Struct.Member { type = new HalfKeyword(), id = MaterialOcclusionMemberName },
            }
        };

        public static Member albedoAccessor     => new Member { member = MaterialAlbedoMemberName };
        public static Member metallicAccessor   => new Member { member = MaterialMetallicMemberName };
        public static Member smoothnessAccessor => new Member { member = MaterialSmoothnessMemberName };
        public static Member emissionAccessor   => new Member { member = MaterialEmissionMemberName };
        public static Member occlusionAccessor  => new Member { member = MaterialOcclusionMemberName };
    }
}
