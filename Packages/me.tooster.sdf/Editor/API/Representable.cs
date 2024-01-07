using System;
using Unity.VisualScripting.YamlDotNet.Serialization.NamingConventions;
using UnityEditor;

namespace me.tooster.sdf.Editor.API {
    /// <summary>
    /// Something with internal and external representation
    /// FIXME: is this level of abstraction needed?
    /// </summary>
    public interface Representable {
        // used for generators
        public string InternalName { get; }

        // used for user-friendly display in GUI
        public string DisplayName { get; }

        public Guid   Guid   { get; }
        public string IdName => formatter.Apply($"{InternalName}_{Guid.ToString()}");

        public static readonly UnderscoredNamingConvention formatter = new UnderscoredNamingConvention();
    }
}
