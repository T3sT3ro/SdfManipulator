#nullable enable
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.NodeGraph.PortData;

namespace me.tooster.sdf.Editor.NodeGraph.Nodes {
    public record EvaluateSdfFunction : Node {
        public IInputPort<HlslSdfFunction> sdfFunction { get; }
        public IInputPort<HlslVector>      position    { get; }

        public IOutputPort<HlslScalar> distance { get; }

        public EvaluateSdfFunction(IOutputPort<HlslSdfFunction>? sdfFunction, IOutputPort<HlslVector>? position)
            : base("sdf_eval", "Evaluate SDF") {
            this.sdfFunction = CreateInput("SDF Function", sdfFunction ?? Util.DefaultSdfNode().sdf);
            this.position = CreateInput("Position", position ?? HlslVector.DefaultNode().Value);

            distance = CreateOutput("Distance", sdfDistanceResult);
        }

        private HlslScalar sdfDistanceResult() => new HlslScalar(HlslSdfFunction.distanceAccessor with
        {
            expression = sdfFunction.Eval().withPosArgument(position.Eval().vectorExpression)
        });
    }
}
