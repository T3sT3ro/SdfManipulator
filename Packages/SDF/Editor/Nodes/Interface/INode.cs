using System.Collections.Generic;
using UnityEditor;

namespace SDF.Interface {
    public interface INode {
        HashSet<string> Includes { get; }
        string          Body     { get; }
        string          Name     { get; }
        GUID            guid     => GUID.Generate();

        List<AbstractPort> InputPorts  { get; }
        List<AbstractPort> OutputPorts { get; }
    }
}
