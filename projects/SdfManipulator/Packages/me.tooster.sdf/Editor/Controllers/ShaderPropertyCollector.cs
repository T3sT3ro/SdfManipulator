using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
namespace me.tooster.sdf.Editor.Controllers {
    public class ShaderPropertyCollector : IPropertyBagVisitor {
        public List<(PropertyPath Path, IProperty Property)> ShaderProperties { get; private set; }

        public void Visit<TContainer>(IPropertyBag<TContainer> properties, ref TContainer container) {
            ShaderProperties = properties.GetProperties(ref container)
                .Where(p => p.HasAttribute<ShaderPropertyAttribute>())
                .Select(p => (Path: PropertyPath.AppendProperty(default, p), Property: (IProperty)p))
                .ToList();
        }
    }
}
