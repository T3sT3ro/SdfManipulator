#nullable enable
using API;
using PortData;

namespace Nodes {
    public record EvaluateSdfFunction : Node {
        public InputPort<HlslSdfFunction> sdfFunction { get; }
        public InputPort<HlslVector>      position    { get; }

        public OutputPort<HlslScalar> distance { get; }

        public EvaluateSdfFunction(OutputPort<HlslSdfFunction>? sdfFunction, OutputPort<HlslVector>? position)
            : base("sdf_eval", "Evaluate SDF") {
            this.sdfFunction = this.CreateInput("SDF Function", sdfFunction ?? Util.DefaultSdfNode().sdf);
            this.position = this.CreateInput("Position", position ?? HlslVector.DefaultNode().Value);

            this.distance = this.CreateOutput("Distance", sdfDistanceResult);
        }

        private HlslScalar sdfDistanceResult() => new HlslScalar(HlslSdfFunction.distanceAccessor with
        {
            expression = this.sdfFunction.Eval().withPosArgument(this.position.Eval().vectorExpression)
        });
    }
}
