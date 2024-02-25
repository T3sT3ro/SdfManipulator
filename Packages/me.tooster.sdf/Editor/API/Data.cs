#nullable enable
namespace me.tooster.sdf.Editor.API {
    /// <summary>
    /// It's a convenient wrapper for data passed to construct a shader. Originally it was meant for the graph model.
    /// A port data acts as a contract and context for constructing partial shader syntax.
    /// For example an SdfFunctionData would define:
    /// - a contract for generating certain type of shader syntax for defining and using SDF functions
    /// - a contextual data needed to create the SDF function, such as required material properties, slots for them and siimilar.
    /// TODO: add validation method
    /// TODO: add "requirements" as object array storing requrired context like required function definition, an include file etc
    /// </summary>
    public abstract record Data;
}
