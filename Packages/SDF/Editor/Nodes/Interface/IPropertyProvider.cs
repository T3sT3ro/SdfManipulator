using System;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine.Rendering;

namespace SDF.Interface {
    [Obsolete("use AbstractShaderProperty from shadegraph")]
    public interface IPropertyProvider {
        List<AbstractShaderProperty> Properties { get; }
        // PropertyType                 Type           { get; }
        // string                       Name           { get; }
        // string                       Default        { get; }
    }
}
