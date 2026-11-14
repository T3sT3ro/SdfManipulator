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
  controlled, e.g. by changing a value that a uniform holds or the scene's *Rendering* settings.
- Add primitives and operators by adding components to game objects, or with *GameObject > SDF > Primitives* and
  *GameObject > SDF > Operators* in the hierarchy.
- The *Volume* primitive draws an SDF stored in the red channel of a 3D texture, e.g. one that the SDF Bake Tool of
  the Visual Effect Graph made from a mesh. The texture fills a box of the given *Size* around the object. *Distance
  Scale* converts the stored values to distances: for textures from the SDF Bake Tool, set it to the largest side of
  the box.
- A combination unites, intersects or subtracts its children, also with smooth edges, or dyes its first child with the
  materials of the others. The blend factor sets the width of smooth edges.
- Add *SDF > Operators > Repeat*, *Mirror* or *Twist* to an SDF object to repeat, mirror or twist its space; the
  component joins the object's SDF stack after the transform.
- Add an *SDF > Material* component to give SDF objects the properties of the Standard shader: albedo, metallic,
  smoothness and emission. A material applies to the SDF object on the same game object and to the objects below it
  that have no material of their own, so one material on the root combination colors the whole scene. Objects without
  a material are white. Where objects blend, e.g. in a smooth union, their materials blend too. An *Albedo Map*
  multiplies the albedo; it's projected from the two axes that face the surface most, in the space of the material's
  object.
- To drive several controllers with one value, add *SDF > Property Link* components: each makes a shader property of
  its target read the uniform of the same property on the driver.
- In the scene view, click an SDF surface to select the scene, and click it again to select the object under the
  cursor. The scene view outlines selected objects and shows them on their own as translucent ghosts, also where
  they're hidden or subtracted. *Tools > SDF > Show Selected Objects* turns the ghosts off.

## Writing your own controllers

Refer to the existing controllers in `Runtime/Primitives` and `Runtime/Operators`.

- A controller property is a serialized field annotated with `[DontCreateProperty]`, plus a property annotated with
  `[CreateProperty]` and `[ShaderProperty]` or `[ShaderStructural]` that returns and sets it with `SetField`, which
  notifies the scene. The default inspector edits the field. Give the property the field's name in `PascalCase`, e.g.
  field `someRadius` and property `SomeRadius`: the inspector finds the attributes of a field this way, to mark
  structural fields and to apply `[VisibleWhen]`.
- Operators that change the point that the rest of the SDF stack evaluates, like *Repeat*, derive from
  `SdfDomainOperator`, which puts them into the stack after the transform when they're added.
- Edits that bypass the property setters, e.g. in the inspector or by undo, reach the scene through `OnValidate`: it
  sets all uniforms and compares the values of the `[ShaderStructural]` properties to decide whether to regenerate the
  shader. A field without a property that changes the structure, e.g. a list of other controllers, can carry
  `[ShaderStructural]` itself.
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
      so regeneration doesn't change the file. Renderers without a material use the default material. The draw mode,
      the world grid and the raymarching settings are in the *Rendering* section of `SdfScene`: they're saved with the
      scene, and prefab instances can override them. To change the render state (culling, depth test and depth
      writes), create a material asset that uses the generated shader and assign it to the renderer.
    - In Prefab Mode, structural changes, undo and redo update the shader in memory right away, and saving the prefab
      regenerates it. *Tools > SDF > Regenerate All Scene Shaders* reimports every scene prefab. *Tools > SDF > Migrate
      Scenes* converts scenes that store a standalone `.shader` or an embedded shader and material, and gives
      controllers that have none an identifier.
    - Each controller has an identifier that it gets once, e.g. `3f9a1c2b`. Generated uniform and function names are
      built from it (`SDF_3f9a1c2b_Radius`), so renaming or moving objects doesn't change them. A comment above each
      generated function shows the object's place in the hierarchy.
    - It listens to property change events emitted by controllers and sets the changed uniforms on its renderers with
      a `MaterialPropertyBlock`, so instances of the same prefab can have different values. The generated shader
      declares the uniforms without values and has no material properties for them, so changing a value doesn't change
      the shader. The material only holds the render state. A renderer that no `SdfScene` updates, e.g. in a prefab
      preview, renders nothing: all its uniforms are 0, including the step count.
    - *Bake Values* writes the current values of the controllers into the shader as `static const` variables instead
      of uniforms, for scenes that don't change. The compiler can then fold them, and the scene sets fewer uniforms.
      Changing a baked value regenerates the shader, which happens only in Prefab Mode, so instances of the prefab
      share the baked values. The scene can still move as a whole, and textures stay uniforms.
- **Shader presets** (`Runtime/Generators`): for now just `BIRPShaderPreset`, a shader generation preset tailored for
  the built-in forward rendering pipeline. Essentially a stateful factory for shader generators; think of it as a
  "cooking school" for creating chefs.
- **Shader generators** (`Runtime/Generators`): for now just `BuiltInGenerator`, which generates shaders specialized
  for the built-in forward rendering pipeline. The scene functions go into an include block that all passes share. The
  pragmas of the preset select the entry points of the forward base pass.
- **Shader library** (`ShaderLibrary/`): HLSL include files with easy to edit parts that controllers use.
    - `operators.hlsl` defines common SDF operators, for example `union` or `intersection`.
    - `primitives.hlsl` defines SDF primitive functions like `torus` or `sphere`.
    - `raymarching.hlsl` defines structs commonly used by SDFs, like `SdfResult`, and the functions that march rays
      and compute normals and occlusion.
    - `builtInShading.hlsl` defines the passes of the built-in pipeline: forward base, forward add, deferred and shadow
      caster.
      Each fragment builds its ray from the view and projection matrices, so the shadow caster pass renders shadow
      maps and the camera depth texture with the same code.
    - `scenePicking.hlsl` defines the passes with which the scene view outlines and picks objects, and the object
      picking pass, which `Editor/SdfScenePicking.cs` draws while the scene view picks, so that a click finds the
      object under the cursor instead of the whole scene.
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
- The built-in generator shades with the lighting of the Standard shader and casts and receives realtime shadows.
  Objects have the albedo, metallic, smoothness and emission of the Standard material, but no textures. You can
  customize generated shaders by extending the existing generators and implementing your own shading.
- Some UI elements are still being improved; limitations of the Unity editor API make this ongoing work.
- Generated shaders aren't tested for VR or for render pipelines other than the Built-in one. In deferred rendering,
  Unity's fog doesn't apply to SDF scenes. Lightmaps and shadow masks don't apply to SDF scenes.
- The HLSL syntax tree API is in its early stages, and a smooth developer experience was preferred over performance.
  This shouldn't matter now: generation isn't frequent, and syntax tree operations aren't a bottleneck in realistic
  use.
- The syntax tree API only implements the subset of the HLSL and ShaderLab grammars it needs so far. It should be easy
  to extend thanks to the source generators, which build the required classes automatically.
- SDF scenes are implemented with prefabs, which provide "a controllable skeleton" for them. An earlier version stored
  the shader and material inside the prefab file, which caused needless reimports on every save and large diffs. Now
  an asset postprocessor generates them as sub-assets of the prefab's import result instead.
- Performance with OpenGL on Linux is so-so. Use the editor in Vulkan mode to work with SDFs interactively.
