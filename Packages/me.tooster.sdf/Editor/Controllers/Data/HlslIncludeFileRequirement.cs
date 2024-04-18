using System;
namespace me.tooster.sdf.Editor.Controllers.Data {
    [Serializable]
    public record HlslIncludeFileRequirement : API.Data.Requirement {
        public string includeFile { get; init; }
    }
}
