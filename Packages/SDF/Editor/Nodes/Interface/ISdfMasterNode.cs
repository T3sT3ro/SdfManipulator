using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;

namespace SDF.Interface {
    public interface ISdfMasterNode {
        string                       ShaderCode { get; }
        List<AbstractShaderProperty> Properties { get; }
        string                       SceneSDF   { get; }
    }
}
