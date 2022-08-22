using System.Collections.Generic;

namespace SDF.Interface {
    public enum PortType { Input, Output }

    public abstract class AbstractPort {
        public virtual string     Name       { get; }
        public         List<AbstractPort> Connection { get; }
        public         PortType   type       { get; }

        public virtual T TryConvertTo<T>() where T : AbstractPort { return null; }
    }
}
