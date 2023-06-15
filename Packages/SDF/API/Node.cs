using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace API {
    /// <summary>
    /// Abstraction over graph nodes as holders properties,
    /// suppliers of bodies (local generation) and properties (global generation)
    /// </summary>
    public interface Node : Representable {
        // TODO support #pragma shader_feature for node toggles
        // TODO support static (node-global) definitions and local (instance) definitions

        public IEnumerable<T> CollectPorts<T>() where T : class, Port {
            // find all ports by reflection
            return this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Where(f => f.FieldType.IsSubclassOf(typeof(T)))
                .Select(f => f.GetValue(this) as T);
        }
        
        public List<Variable> CollectVariables() {
            return this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(f => f.FieldType.IsSubclassOf(typeof(Variable)))
                .Select(f => f.GetValue(this) as Variable)
                .ToList();
        }

        public ISet<string> CollectIncludes() {
            return this.GetType().GetCustomAttributes(typeof(ShaderIncludeAttribute), true)
                .Cast<ShaderIncludeAttribute>()
                .SelectMany(attr => attr.ShaderIncludes)
                .ToHashSet();
        }
    }

    public interface ConsumerNode : Node {
        ISet<InputPort> InputPorts =>
            this.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(f => f.FieldType.IsSubclassOf(typeof(InputPort)))
                .Select(f => f.GetValue(this) as InputPort)
                .ToHashSet();
    }

    public interface ProducerNode : Node {
        // using reflection return all OutputPorts 
        ISet<OutputPort> OutputPorts =>
            this.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(f => f.FieldType.IsSubclassOf(typeof(OutputPort)))
                .Select(f => f.GetValue(this) as OutputPort)
                .ToHashSet();
    }
}
