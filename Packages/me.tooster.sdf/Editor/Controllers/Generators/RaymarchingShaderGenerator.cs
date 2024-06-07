using System;
using System.Collections.Generic;
using System.Linq;
using me.tooster.sdf.Editor.Controllers.SDF;
namespace me.tooster.sdf.Editor.Controllers.Generators {
    public abstract class RaymarchingShaderGenerator {
        protected SdfScene scene;
        public RaymarchingShaderGenerator(SdfScene scene) => this.scene = scene;

        public abstract string MainShader();

        public static readonly Dictionary<string, Type> allGenerators;

        static RaymarchingShaderGenerator() {
            allGenerators = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(domainAssembly => domainAssembly.GetTypes())
                .Where(
                    type => type.IsSubclassOf(typeof(RaymarchingShaderGenerator))
                     && !type.IsAbstract
                     && type.GetConstructor(new[] { typeof(SdfScene) }) != null
                ).ToDictionary(t => t.FullName);
        }

        public static RaymarchingShaderGenerator InstantiateGenerator(string name, SdfScene scene)
            => (RaymarchingShaderGenerator)allGenerators[name].GetConstructor(new[] { typeof(SdfScene) })!.Invoke(new object[] { scene });
    }
}
