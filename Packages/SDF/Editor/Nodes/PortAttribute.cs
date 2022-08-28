using System;
using SDF.Interface;

namespace Editor.Nodes {
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class PortAttribute : Attribute {
        private PortType type;

        public PortAttribute(PortType type) {
            this.type = type;
        }
    }
}
