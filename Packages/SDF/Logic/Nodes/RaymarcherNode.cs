#nullable enable
using API;
using PortData;
using UnityEngine;

namespace Nodes {
    public record RaymarcherNode : Node {
        
        public InputPort<HlslSdfFunction> Sdf { get; }
        
        public OutputPort<HlslScalar> Distance { get; }
        public OutputPort<HlslScalar> ObjectID { get; }
        public OutputPort<HlslVector> Position { get; }
        public OutputPort<HlslVector> Normal   { get; }

        //         [Header(Raymarcher)]
        private Property<int>   maxSteps      = new Property<int>("MAX_STEPS", "max raymarching steps", 200);
        private Property<float> maxDistance   = new Property<float>("MAX_DISTANCE", "max raymarching distance", 200.0f);
        private Property<float> rayOriginBias = new Property<float>("RAY_ORIGIN_BIAS", "ray origin bias", 0);

        private Property<float> epsilonRay =
            new Property<float>("EPSILON_RAY", "epsilon step for ray to consider hit", 0.001f);

        private Property<float> epsilonNormal =
            new Property<float>("EPSILON_NORMAL", "epsilon for calculating normal", 0.001f);
        
        public RaymarcherNode(OutputPort<HlslSdfFunction>? Sdf) : base ("raymarch_sdf", "Raymarch SDF") {
            
            this.Sdf = this.CreateInput("SDF", Sdf ?? HlslSdfFunction.DefaultNode().Value);
            
            Distance = this.CreateOutput<HlslScalar>("distance", () => new(HlslSdfFunction.distanceAccessor));
            ObjectID = this.CreateOutput<HlslScalar>("object_id", () => new(HlslSdfFunction.idAccessor));
            Position = this.CreateOutput<HlslVector>("position", () => new(HlslSdfFunction.pointAccessor));
            Normal = this.CreateOutput<HlslVector>("normal", () => new(HlslSdfFunction.normalAccessor));
        }
    }

    public delegate string Evaluator(SdfNode sdf);
}
