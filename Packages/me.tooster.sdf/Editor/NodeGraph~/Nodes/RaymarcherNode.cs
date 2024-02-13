#nullable enable
using me.tooster.sdf.Editor.API;
using me.tooster.sdf.Editor.API.Graph;
using me.tooster.sdf.Editor.NodeGraph.PortData;
using UnityEngine;

namespace me.tooster.sdf.Editor.NodeGraph.Nodes {
    /// <summary>
    /// Node generating a raymarching loop for a given SDF function.
    /// TODO: take ray origin and direction as inputs
    /// </summary>
    public record RaymarcherNode : Node {
        public IInputPort<HlslVector>      rayOrigin { get; }
        public IInputPort<HlslVector>      rayDir    { get; }
        public IInputPort<HlslSdfFunction> sdf       { get; }

        // raymarch configuration properties
        public IInputPort<HlslScalar> maxSteps      { get; }
        public IInputPort<HlslScalar> maxDistance   { get; }
        public IInputPort<HlslScalar> rayOriginBias { get; }
        public IInputPort<HlslScalar> epsilonRay    { get; }
        public IInputPort<HlslScalar> epsilonNormal { get; }

        // raymarching outputs
        public IOutputPort<HlslScalar> distance { get; }
        public IOutputPort<HlslScalar> objectID { get; }
        public IOutputPort<HlslVector> position { get; }
        public IOutputPort<HlslVector> normal   { get; }


        // todo:        [Header(Raymarcher)] for material inspector and property group

        public RaymarcherNode(IOutputPort<HlslSdfFunction>? sdf) : base("raymarch_sdf", "Raymarch SDF") {
            rayOrigin = CreateInput("ray origin", HlslVector.DefaultNode(Vector4.zero).Value);
            rayDir = CreateInput("ray direction", HlslVector.DefaultNode(Vector4.one).Value);
            this.sdf = CreateInput("SDF", sdf ?? HlslSdfFunction.DefaultNode().sdf);

            maxSteps = CreateInput("max raymarching steps", HlslScalar.DefaultIntNode(200).Value);
            maxDistance = CreateInput("max raymarching distance", HlslScalar.DefaultFloatNode(200f).Value);
            rayOriginBias = CreateInput("ray origin bias", HlslScalar.DefaultFloatNode(0f).Value);
            epsilonRay = CreateInput("epsilon step for ray to consider hit", HlslScalar.DefaultFloatNode(0.001f).Value);
            epsilonNormal = CreateInput("epsilon for calculating normal", HlslScalar.DefaultFloatNode(0.001f).Value);

            distance = CreateOutput<HlslScalar>("distance", () => new(HlslSdfFunction.distanceAccessor));
            objectID = CreateOutput<HlslScalar>("object_id", () => new(HlslSdfFunction.idAccessor));
            position = CreateOutput<HlslVector>("position", () => new(HlslSdfFunction.pointAccessor));
            normal = CreateOutput<HlslVector>("normal", () => new(HlslSdfFunction.normalAccessor));
        }
    }
}
