using System.Collections.Generic;
using API;

namespace Logic.Nodes {
    public class UnlitRaymarcherMasterNode : MasterNode {
        public ISet<InputPort> InputPorts   { get; }
        public string          InternalName { get; }
        public string          DisplayName  { get; }

        //         [Header(Raymarcher)]
        private Property<int>   maxSteps      = new Property<int>("MAX_STEPS", "max raymarching steps", 200);
        private Property<float> maxDistance   = new Property<float>("MAX_DISTANCE", "max raymarching distance", 200.0f);
        private Property<float> rayOriginBias = new Property<float>("RAY_ORIGIN_BIAS", "ray origin bias", 0);
        private Property<float> epsilonRay    = new Property<float>("EPSILON_RAY", "epsilon step for ray to consider hit", 0.001f);
        private Property<float> epsilonNormal = new Property<float>("EPSILON_NORMAL", "epsilon for calculating normal", 0.001f);
    }
}
