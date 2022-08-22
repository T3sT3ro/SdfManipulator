using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;

namespace SDF.Interface {
    public interface ISdfMasterNode {
        string                  ShaderCode { get; }
    }
}
