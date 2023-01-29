using System.Collections.Generic;
using API;
using Logic.Properties;

namespace Logic.Nodes {
    public abstract class SdfNode : ConsumerNode, ProducerNode {
        public string InternalName => "sdf";
        public string DisplayName  => "SDF";

        private InputPort<IntProperty>  sphereRadius; // FIXME yeet
        private OutputPort<IntProperty> distance;

        public ISet<InputPort>  InputPorts  => new HashSet<InputPort> { sphereRadius };
        public ISet<OutputPort> OutputPorts => new HashSet<OutputPort> { distance };

        #region selective visitor pattern

        public interface Visitor<out R> {
            public R visit(SdfNode node);
        }

        public R accept<R>(Visitor<R>      visitor) => visitor.visit(this);
        public R accept<R>(Node.Visitor<R> visitor) => accept((Visitor<R>)visitor);

        #endregion
    }
}
