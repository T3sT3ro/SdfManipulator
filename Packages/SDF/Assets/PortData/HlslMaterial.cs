using API;
using AST.Hlsl.Syntax;
using AST.Hlsl.Syntax.Expressions.Operators;

namespace PortData {
    
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
                    type = new VectorToken { type = new FixedKeyword(), arity = 4 },
                    id = MaterialAlbedoMemberName
                },
                new Type.Struct.Member { type = new HalfKeyword(), id = MaterialMetallicMemberName },
                new Type.Struct.Member { type = new HalfKeyword(), id = MaterialSmoothnessMemberName },
                new Type.Struct.Member
                {
                    type = new VectorToken { type = new HalfKeyword(), arity = 3 },
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
