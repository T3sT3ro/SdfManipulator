# SDF Manipulator

SDF Manipulator builds raymarched **S**igned **D**istance **F**ield scenes from components and gizmos in the Unity
editor, and generates the HLSL shaders that draw them.

- [Requirements](#requirements)
- [Installation](#installation)
- [Samples](#samples)
- [Creating SDF scenes](#creating-sdf-scenes)
- [Writing your own controllers](#writing-your-own-controllers)
- [Concepts](#concepts)
- [Dependencies](#dependencies)
- [Limitations and known issues](#limitations-and-known-issues)

## Requirements

- Unity 6.6 (developed and tested with `6000.6.8f1`). Other versions are untested.
- The Built-in Render Pipeline: generated shaders target forward rendering in the Built-in pipeline.
- Vulkan is strongly recommended for interactive editing on Linux (start the editor with
  `-force-vulkan -force-gfx-mt`). OpenGL performance there is poor.

## Installation

The package is pre-release. Open *Window > Package Manager*, choose *+ > Add package from git URL…* and enter:

```
https://github.com/T3sT3ro/SdfManipulator.git?path=/packages/me.tooster.sdf
```

## Samples

In the Package Manager, select *SDF Manipulator*, open *Samples* and import **Examples**. It contains several SDF
scenes (Hello SDF, primitives, a CSG box, a bunny and a rhino-rat), each in its own folder with its generated shader
and material, and `Showcase.unity`, which places all of them under an HDRI sky.

## Creating SDF scenes

- Create a scene with *Assets > Create > SDF > Scene Asset*. SDF scenes are prefabs: open the prefab to enter the
  prefab stage, where the scene is edited. Modifying prefab instances may not have any effect.
- Outside the prefab stage, scenes can't be edited structurally (e.g. adding a primitive). They can only be
  controlled, e.g. by updating a uniform or keyword.
- Add primitives and operators by adding components to game objects, or with *GameObject > SDF > Primitives* and
  *GameObject > SDF > Operators* in the hierarchy.

## Writing your own controllers

Refer to the existing controllers in `Runtime/Primitives` and `Runtime/Operators`.

- For now (due to engine limitations) a controller property must be defined as a field annotated with
  `[DontCreateProperty]` in `camelCase`, plus a property annotated with `[CreateProperty]` and `[ShaderProperty]` or
  `[ShaderStructural]` that returns and sets it (and invokes the appropriate events). The property must have the same
  name as the field, only in `PascalCase`, e.g. field `someRadius` and property `SomeRadius`. Without it, the default
  inspector won't generate and bind the property, and you'd have to write an editor by hand.
- A controller returns an `IData` implementation used to build the syntax tree. `IData` forms a kind of contract: two
  separate components can expect an `SdfData` to have a certain shape and hold certain data. For example, `SdfData`
  includes an `evaluationExpression` that can be assumed to hold an expression of type `(float3) -> SdfResult`, and a
  set of requirements, such as the path to a required `hlsl` file or an exported SDF function that must be present in
  the generated shader for the expression to evaluate.
- `Processor`, `ShaderPreset` and `RaymarchingShaderGenerator` assemble the final shader code; refer to their usage
  for details. `Processor` is mainly a marker and handler of requirements and evaluation context, `ShaderPreset` is a
  configurable generator factory, and generators are stateful but ephemeral, disposable objects based on `Processor`.

## Concepts

Paths are relative to the package root.

- **Controllers** (`Runtime/Scene`, `Runtime/Primitives`, `Runtime/Operators`) have two purposes:
    - export the parts of data required to build the syntax tree (the editor part, for working with scenes)
    - control the scene data via uniforms and the like (the runtime part)
- **Inspectors** (`Editor/Inspectors`) handle the inspector UI and scene gizmos for controllers.
- **`SdfScene`**: the root component of an SDF scene, attached to the prefab root.
    - Importing the prefab generates the scene's shader and a default material and adds them to the prefab as
      sub-assets, the way Shader Graph adds them to a graph. They live in the import result, not in the prefab file,
      so regeneration doesn't change the file. Renderers without a material use the default material. To change
      material settings such as the draw mode, create a material asset that uses the generated shader and assign it to
      the renderer.
    - In Prefab Mode, structural changes update the shader in memory right away, and saving the prefab regenerates it.
      *Tools > SDF > Regenerate All Scene Shaders* reimports every scene prefab, and *Tools > SDF > Migrate Scenes to
      Generated Sub-Assets* converts scenes that store a standalone `.shader` or an embedded shader and material.
    - At runtime, it listens to property change events emitted by controllers and updates the controlled material.
- **Shader presets** (`Runtime/Generators`): for now just `BIRPShaderPreset`, a shader generation preset tailored for
  the built-in forward rendering pipeline. Essentially a stateful factory for shader generators; think of it as a
  "cooking school" for creating chefs.
- **Shader generators** (`Runtime/Generators`): for now just `BuiltInGenerator`, which generates shaders specialized
  for the built-in forward rendering pipeline.
- **Shader library** (`ShaderLibrary/`): HLSL include files with easy to edit parts that controllers use.
    - `operators.hlsl` defines common SDF operators, for example `union` or `intersection`.
    - `primitives.hlsl` defines SDF primitive functions like `torus` or `sphere`.
    - `raymarching.hlsl` defines structs commonly used by SDFs, like `SdfResult`, and ready to use functions, like the
      vertex and fragment functions that do the actual raymarching.
    - and more, for easier shader composition.
- **AST library** (`Runtime/AST`): classes for working with HLSL and ShaderLab syntax trees, modeled after Roslyn's
  Syntax API.
    - `Syntax` holds common syntactic forms and data structures like syntax nodes; `Hlsl` and `Shaderlab` hold the
      syntax trees of those languages.
    - `Generators.dll` is a Roslyn source generator that writes the required partial classes for syntax nodes. Its
      sources live in the repository's `tools/SyntaxGenerators`.
- **`IsExternalInitShim`** (`Runtime/IsExternalInitShim`): a separate assembly that adds support for records and
  `init`-only properties in the C# version Unity uses. Future Unity versions with fuller record support won't need it.

## Dependencies

- `Unity.Properties` (built in).
- UI Toolkit editor bindings. Runtime bindings may replace them later, to make the interface more reactive and the
  editor integration deeper.
- The Roslyn compiler bundled with Unity (for the syntax generators).

## Limitations and known issues

- Because of how Unity processes and recompiles shaders and assets, some changes may cause needless shader
  regenerations and recompilations, while others may need a manual refresh to fully regenerate. This is being worked
  on.
- The default generators only implement basic fake Lambert, debug and unshaded drawing. You can customize generated
  shaders by extending the existing generators and implementing your own shading.
- Some UI elements are still being improved; limitations of the Unity editor API make this ongoing work.
- Generated shaders aren't tested for VR or for render pipelines other than forward Built-in. Support for a shadow
  caster pass, culling, depth etc. is being worked on, and you can add it yourself by extending the existing
  generators.
- The HLSL syntax tree API is in its early stages, and a smooth developer experience was preferred over performance.
  This shouldn't matter now: generation isn't frequent, and syntax tree operations aren't a bottleneck in realistic
  use.
- The syntax tree API only implements the subset of the HLSL and ShaderLab grammars it needs so far. It should be easy
  to extend thanks to the source generators, which build the required classes automatically.
- SDF scenes are implemented with prefabs, which provide "a controllable skeleton" for them. An earlier version stored
  the shader and material inside the prefab file, which caused needless reimports on every save and large diffs. Now
  an asset postprocessor generates them as sub-assets of the prefab's import result instead.
- Performance with OpenGL on Linux is so-so. Use the editor in Vulkan mode to work with SDFs interactively.
