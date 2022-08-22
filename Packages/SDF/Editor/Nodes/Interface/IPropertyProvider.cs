using System.Collections.Generic;

namespace SDF.Interface {
    public interface IPropertyProvider {
        public List<IProperty> Properties { get; }
    }
}
