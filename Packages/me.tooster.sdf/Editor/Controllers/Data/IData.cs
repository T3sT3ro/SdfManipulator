namespace me.tooster.sdf.Editor.Controllers.Data {
    /// <summary>
    /// Marker interface for data passed between modifiers
    /// It's a convenient wrapper for data passed to construct a shader. Originally it was meant for the graph model.
    /// A port data acts as a contract and context for constructing partial shader syntax.
    /// For example an a ScalarData can bundle information about the type of scalar and how it's value can be evaluated in the shader
    /// </summary>
    public interface IData { }
}
