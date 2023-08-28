#nullable enable
using API;
using AST.Hlsl;
using AST.Hlsl.Syntax;
using AST.Hlsl.Syntax.Expressions.Operators;
using AST.Syntax;
using PortData;

namespace Assets.Nodes.SdfNodes {
    public record SdfSphereNode : SdfNode {
        public override IInputPort<HlslMatrix> transform { get; }
        public          IInputPort<HlslScalar> radius    { get; }

        public override IOutputPort<HlslSdfFunction> sdf { get; }


        // for now uses the primitives include
        public SdfSphereNode(IOutputPort<HlslMatrix>? transform, IOutputPort<HlslScalar>? radius)
            : base("sdf_sphere", "SDF Sphere") {
            this.transform = CreateInput("Transform", transform ?? HlslMatrix.DefaultNode().Value);
            this.radius = CreateInput("Radius", radius ?? HlslScalar.DefaultFloatNode().Value);

            sdf = CreateOutput("Sdf", () => sdfCallSyntax);
        }

        private HlslSdfFunction sdfCallSyntax => new(new Call
        {
            id = InternalName,
            argList = new[] { transform.Eval().matrixExpression, radius.Eval().scalarExpression }
        });
    }
}
