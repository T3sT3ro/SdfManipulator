using System.Collections.Generic;

namespace API {
    /// <summary>
    /// Abstraction over graph nodes as holders properties,
    /// suppliers of bodies (local generation) and properties (global generation)
    /// </summary>
    public interface Node : Representable {
        // TODO support #pragma shader_feature for node toggles
        // TODO support static (node-global) definitions and local (instance) definitions

        #region selective visitor pattern

        public interface Visitor<out R> {
            public R visit(Node node);
        }

        public R accept<R>(Visitor<R> visitor);

        #endregion
    }

    public interface ConsumerNode : Node {
        // getters for enabled properties 
        ISet<InputPort> InputPorts { get; }
    }

    public interface ProducerNode : Node {
        // list of include files in a form of "includes/someFile.h"
        ISet<OutputPort> OutputPorts { get; }
    }
    
    
}
