using System;
namespace me.tooster.sdf.Editor.Controllers.Data {
    [Serializable]
    public record HlslIncludeFileRequirement(string includeFile) : API.Data.Requirement;
}
