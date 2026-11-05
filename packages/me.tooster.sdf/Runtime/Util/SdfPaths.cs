namespace me.tooster.sdf.Util {
    /// <summary>
    ///     Asset paths inside this package. Unity resolves <c>Packages/me.tooster.sdf/…</c> by package name, wherever the
    ///     package lives on disk, so these only change when files move inside the package.
    /// </summary>
    /// <remarks>
    ///     Shader include paths are copied into generated shaders and into serialized shader presets, so changing them
    ///     also requires migrating existing SDF scenes.
    /// </remarks>
    public static class SdfPaths {
        public const string PackageName = "me.tooster.sdf";
        public const string PackageRoot = "Packages/" + PackageName;

        /// <summary>HLSL include files used by generated shaders.</summary>
        public static class ShaderLibrary {
            public const string Root               = PackageRoot + "/ShaderLibrary";
            public const string BuiltInForwardBase = Root + "/builtInForwardBase.hlsl";
            public const string DebugBaseShading   = Root + "/debugBaseShading.hlsl";
            public const string Matrix             = Root + "/matrix.hlsl";
            public const string Operators          = Root + "/operators.hlsl";
            public const string Primitives         = Root + "/primitives.hlsl";
            public const string Raymarching        = Root + "/raymarching.hlsl";
            public const string Util               = Root + "/util.hlsl";
        }

        public static class Icons {
            public const string Root     = PackageRoot + "/Editor/Icons";
            public const string Sdf      = Root + "/sdf-icon-256.png";
            public const string SdfScene = Root + "/sdf-scene-914.png";
        }

        public static class Templates {
            public const string HelloSdfPrefab = PackageRoot + "/Editor/Templates/Hello SDF.prefab";
        }
    }
}
