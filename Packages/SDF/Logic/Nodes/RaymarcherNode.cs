using API;
using UnityEngine;

namespace Nodes {
    public class RaymarcherNode : ConsumerNode, ProducerNode {
        public string InternalName { get; }
        public string DisplayName  { get; }

        public OutputPort<Variable<float>>   Distance { get; }
        public OutputPort<Variable<int>>     ObjectID { get; }
        public OutputPort<Variable<Vector3>> Position { get; }
        public OutputPort<Variable<Vector3>> Normal   { get; }

        public RaymarcherNode() {
            InternalName = "raymarcher";
            DisplayName = "Raymarcher";
            Distance = new OutputPort<Variable<float>>(this, "distance");
            ObjectID = new OutputPort<Variable<int>>(this, "objectId");
            Position = new OutputPort<Variable<Vector3>>(this, "point");
            Normal = new OutputPort<Variable<Vector3>>(this, "normal");
        }

        //         [Header(Raymarcher)]
        private Variable<int>    maxSteps      = new Variable<int>("MAX_STEPS", "max raymarching steps", 200);
        private Variable<float>  maxDistance   = new Variable<float>("MAX_DISTANCE", "max raymarching distance", 200.0f);
        private Variable<float>  rayOriginBias = new Variable<float>("RAY_ORIGIN_BIAS", "ray origin bias", 0);
        private Variable<float>  epsilonRay    = new Variable<float>("EPSILON_RAY", "epsilon step for ray to consider hit", 0.001f);
        private Variable<float>  epsilonNormal = new Variable<float>("EPSILON_NORMAL", "epsilon for calculating normal", 0.001f);
    }
    
    public delegate string Evaluator(SdfNode sdf);

}