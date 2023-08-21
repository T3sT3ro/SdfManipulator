#nullable enable
using API;
using AST.Hlsl.Syntax.Expressions.Operators;
using PortData;

namespace Nodes.SdfNodes {
    public record SdfSphereNode : SdfNode {
        public InputPort<HlslVector> center { get; }
        public InputPort<HlslScalar> radius { get; }

        // for now uses the primitives include
        public SdfSphereNode(OutputPort<HlslVector>? center, OutputPort<HlslScalar>? radius)
            : base("sdf_sphere", "SDF Sphere") {
            this.center = this.CreateInput("Center", center ?? HlslVector.DefaultNode().Value);
            this.radius = this.CreateInput("Radius", radius ?? HlslScalar.DefaultFloatNode().Value);

            this.sdf = CreateOutput("Sdf", () => sdfCallSyntax);
        }

        private HlslSdfFunction sdfCallSyntax => new(new Call
        {
            id = InternalName,
            arguments = new[]
            {
                this.center.Eval().vectorExpression,
                this.radius.Eval().scalarExpression
            }
        });
    }
}
