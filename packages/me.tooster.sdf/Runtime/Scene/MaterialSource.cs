namespace me.tooster.sdf {
    /// <summary>Where an SDF object takes its material from.</summary>
    public enum MaterialSource {
        /// <summary>From the nearest parent that has its own material, or the default white material if none has one.</summary>
        Inherited = 0,
        /// <summary>From the object's own material properties.</summary>
        Own = 1,
    }
}
