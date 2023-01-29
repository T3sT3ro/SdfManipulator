using System.Collections.Generic;
using API;
using Logic.Properties;

namespace Logic.Nodes {
    public class UnlitRaymarcherMaster : MasterNode {
        public ISet<InputPort> InputPorts   { get; }
        public string          InternalName { get; }
        public string          DisplayName  { get; }

        //         [Header(Raymarcher)]
        public IntProperty   maxSteps      = new IntProperty("MAX_STEPS", "max raymarching steps", 200);
        public FloatProperty maxDistance   = new FloatProperty("MAX_DISTANCE", "max raymarching distance", 200.0f);
        public FloatProperty rayOriginBias = new FloatProperty("RAY_ORIGIN_BIAS", "ray origin bias", 0);
        public FloatProperty epsilonRay    = new FloatProperty("EPSILON_RAY", "epsilon step for ray to consider hit", 0.001f);
        public FloatProperty epsilonNormal = new FloatProperty("EPSILON_NORMAL", "epsilon for calculating normal", 0.001f);

        #region selective visitor pattern

        public interface Visitor<out R> {
            public R visit(UnlitRaymarcherMaster master);
        }

        public R accept<R>(Visitor<R>      visitor) => visitor.visit(this);
        public R accept<R>(Node.Visitor<R> visitor) => accept((Visitor<R>)visitor);

        #endregion
    }
}
