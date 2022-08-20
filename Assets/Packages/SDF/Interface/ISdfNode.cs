using System.Collections.Generic;
using UnityEditor;

namespace SDF.Interface {
    public interface ISdfNode : IPropertyProvider {
        string                  Include    { get; }
        string                  Body       { get; }
        string                  Name       { get; }
        GUID                    guid       => GUID.Generate();
    }
}
