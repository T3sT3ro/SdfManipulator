# Overview

For now, this project contains:

- A package with vast collection of SDF primitives, operators and useful HLSL functions used to construct raymarching
  shaders.
- Several test assets in the main `Assets/` directory, of which `Prefabs/` holds main test scenes demonstrating
  usages. There are also several scenes showcasing the usage of instantiated SDF scenes.
- the tool itself as an embedded package inside `Packages/me.tooster.sdf/`
- There is some unused or experimental code for future work.

## Gallery

[Image gallery link on imgur](https://imgur.com/a/H5ey91M)

<!-- <video controls src="https://i.imgur.com/fQZA06Z.mp4" title="Title"></video> -->
![SDF modelling process](https://i.imgur.com/fQZA06Z.mp4)
![SDF bunny HDR](https://i.imgur.com/VPo1lo7.png)
![SDF primitives and operators](https://i.imgur.com/2eERgoE.png)
![SDF CSG operations and onion](https://i.imgur.com/TWIqg05.png)
![SDF unicorn](https://i.imgur.com/cv1sIsF.png)
![SDF guy](https://i.imgur.com/WAFLz9s.png)
![SDF CSG](https://i.imgur.com/GAZzuap.png)
![SDF scene with debug overlays](https://i.imgur.com/T2EUYrW.png)

## Installation:

1. `git clone` the repository
2. open in Unity (!) `2022.3.32f1 LTS` (!) — this version is important. Earlier versions triggered some editor bugs.
   Later
   version (2023.2....) does some changes to rendering that makes rendering and lighting weird (temporal GI?)
    - preferably start unity with vulkan, because it provides superior rendering performance. OpenGL has serious
      performance issues on linux. To start with vulkan use `-force-vulkan -force-gfx-mt` in extra command lines passed
      to Unity (can be
      set inside unity hub)

## Usage:

- Refer to existing controllers
    - for now (due to engine limitations) the properties of the controller must be defined with a field annotated
      with `[DontCreateProperty]` in lowerCase and a corresponding property annotated with `[CreateProperty]`
      and `[ShaderProperty]` or `[ShaderStructural]` returning and setting it (and invoking appropriate events). THE
      PROPERTY MUST BE NAMED THE SAME AS THE FIELD FOR NOW, ONLY STARTING WITH "PascalCase", e.g. field `someRadius` and
      property `SomeRadius`. Without it, the default inspector won't generate and bind property, so manual editor
      creation will be necessary
    - return a subclass of `Data` that is used to build the AST. `Data` forms a kind of contract: two separate
      components can expect an SdfData to be of some certain shape and hold some required data. For example `SdfData`
      may include `evaluationExpression` that can be assumed to hold an expression of type `(float3) -> SdfResult` and a
      bunch of `Requirements` including a path to the required `hlsl` file or an exported SdfFunction required to be
      present in the generated shader for the evaluation expression to evaluate.
    - `Processor`, `ShaderPreset` and `RaymarchingShaderGenerator` for assembling final shader code. Refer to their
    - usage for detailed understanding. Basically `Processor` is mainly a marker/handler of requirements/evaluation
      context, `ShaderPreset` is configurable generator factory and generators are stateful but ephemeral and disposable
      generators based on `Processor`.
- Creating scenes:
    - Use context menu in the Assets folder and select `SDF > Scene` option. Opening the prefab will open the stage for
      editing the scene. Editing SDF scenes takes place inside prefabs, so be aware that modifying instances may not
      yield any results.
    - The scenes are not editable outside the prefab stage (e.g. adding new primitive). They are only controllable (e.g.
      updating a uniform/keyword)
    - Adding primitives is as simple as adding components to game objects or using `SDF` context menu in game object
      hierarchy window.

## TOP-LEVEL concepts:

- the paths below refer to paths inside `Packages/me.tooster.sdf/`
- "Controllers" (in the package's `Editor/Controllers` directory) have 2 purposes:
    - export parts of data required to build AST (the "editor" part for working with scenes)
    - control the scene data via uniforms and whatnot (the "runtime" part)
- "Controller Editors" in `Editor/Controllers/Editors` handle inspector and scene gizmos for objects
- "Sdf Scene" — a root component of a scene. It stores reference to a material and shader. Often referred simply as "
  scene" because I don't utilize regular scenes. It is attached to the prefab root.
    - for editor part, it handles regeneration of the shader asset and coordinates all controllers under itself
    - for controller part, it listens to property change events emitted by controllers updates controlled materials
- "Shader presets" - for now just a `BirpPreset`: a shader generation preset tailored for built-in forward renderer
  pipeline.
  Essentially a stateful factory for final shader generators. Think of it as a "cooking school" for creating chefs.
- "Shader generators" - for now just a `BuiltInGenerator` which can generate specialized shader for built-in, forward
  render pipeline.
- hlsl include files in `Editor/Resources/Includes` (probably should be moved to Runtime?) store easy to edit hlsl parts
  that can be used by controllers
    - `operators.hlsl` defines common SDF operators, for example `union` or `intersection`
    - `primitives.hlsl` defines a bunch of SDF primitive functions like `torus` or `sphere`
    - `raymarching.hlsl` defines structs used commonly by SDFs, like `SdfResult`, and a bunch of ready to use functions,
      like vertex and fragment function, for doing the actual raymarching.
    - and many more for easier shader composition
- "Über shader" — the shader that started it all and has a bunch of code (also legacy code) ready to be ported to
  individual controllers or shader templates/partials. For some reason it doesn't render on vulkan at the time of
  writing (although it sometimes renders in the material preview???). It worked flawlessly on OpenGL on linux though.
- "AST library" - the `Runtime/` and `SyntaxGenerators/` part. The `Runtime/AST` holds classes used to work with syntax
  trees for hlsl and shaderlab.
    - The `Syntax` holds common syntactic forms or data structures like AST nodes. The `Hlsl` and `Shaderlab` dirs hold
      C# classes for ASTs of those languages.
    - The `Generators.dll` is a compiled binary copied
      from `SyntaxGenerators/SyntaxGenerators/bin/Debug/netstandard2.0/Generators.dll`.
    - The `SyntaxGenerators` defines a sub-project (separate and independent of unity) for a syntax generator used to
      generate required partial classes for AST classes. This subproject uses Roslyn heavily.
- "IsExternalInitShim" - this is a separate asmdef that can be easily included to add support for records and `init`
  only proeprties in the current C# version used by unity.

# Used packages

- `Unity.Properties` (built-in, auto included)
- ~~`UI Toolkit Runtime bindings`~~ for now UI Toolkit editor bindings are used
- Roslyn compiler, bundled in unity.
