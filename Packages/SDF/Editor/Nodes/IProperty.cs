namespace SDF.Interface {
    public interface IProperty {
        string Name           { get; }
        string ShaderlabBlock { get; }
        string HlslBlock      { get; }
    }
}
