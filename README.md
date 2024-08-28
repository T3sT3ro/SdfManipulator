# Overview

This is a monorepo for the SDF manipulator (work title) Unity Package. It implements a visual editor for SDF scenes using easy to use gizmos and composable primitieves. The tool runs in realtime in the editor and generates ready to use HLSL shaders drawing the SDF scenes.

For now, this project contains:

- A package with vast collection of SDF primitives, operators and useful HLSL functions used to construct raymarching
  shaders.
- Several test assets in the main `Assets/` directory, of which `Prefabs/` holds main test scenes demonstrating
  usages. There are also several scenes showcasing the usage of instantiated SDF scenes.
- the tool itself as an embedded package inside `Packages/me.tooster.sdf/`, containing definitions of primitives, operators, their visual controllers, shader generators and an embedded HLSL syntax tree API for assembling shader code using ASTs.
- Some unused or experimental code meant to be organized and cleaned later.

## Table of Contents

- [Overview](#overview)
  - [Table of Contents](#table-of-contents)
  - [Gallery](#gallery)
  - [Installation:](#installation)
  - [Usage:](#usage)
  - [TOP-LEVEL concepts:](#top-level-concepts)
  - [Used packages](#used-packages)
  - [Limitations, known issues](#limitations-known-issues)

## Gallery

[Link to Imgur gallery](https://imgur.com/a/sdf-raymarching-shader-generator-tool-unity-H5ey91M)

https://github.com/user-attachments/assets/f52581a2-1cf7-49b1-a8d6-941ec3a6ccc7

![SDF Bunny HDR](https://github.com/user-attachments/assets/99d74eb7-17c4-4dc5-9b28-2ded13da247a)
*SDF Bunny HDR*
![SDF Primitives and operators](https://github.com/user-attachments/assets/86403f4f-b96c-44e1-85b0-f66b316dd952)
![SDF CSG example with onion skin](https://github.com/user-attachments/assets/595ddb11-8099-44d0-940e-a3a7bdea3cfa)
![Uncanny SDF unicorn, 3 minutes of work](https://github.com/user-attachments/assets/065294c6-b160-43e2-8b9d-eb89e11b3e78)
![a simple smooth figure of a guy](https://github.com/user-attachments/assets/e3ac588a-51a0-47f9-9dd8-e3e2ba1d6cd9)
![Standard CSG example with Lambert shading](https://github.com/user-attachments/assets/d54df9d2-de01-4790-89a5-39de7e10522f)
![SDF scene with debug overlays](https://github.com/user-attachments/assets/972dc458-562c-47c1-a7e5-cd9f7345de5c)

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
- `IsExternalInitShim` - this is a separate asmdef that can be easily included to add support for records and `init`
  only properties in the current C# version used by unity. In the future version of Unity with more comprehensive support for records/struct records, this shim won't be needed anymore.

## Used packages

- `Unity.Properties` (built-in, auto included)
- ~~`UI Toolkit Runtime bindings`~~ for now UI Toolkit editor bindings are used instead, but some investigative work is being done to migrate to this new system and make the interface more reactive and editor integration deeper.
- Roslyn compiler, bundled in unity (for syntax generators).

## Limitations, known issues

- Due to how Unity shaders and assets are processed recompiled, some changes may cause needless shader regenerations and recompilations, while some others may require manual refresh to fully regenerate. This is a work-in-progress issue.
- Currently only basic fake Lambert/debug/unshaded drawing is implemented in the default generators. Users can define and customize generated shaders by extending existing generators and implementing their own shading etc.
- Some UI elements are still being enhanced to provide smooth UX/DX. Due to the limitations of Unity editor API, this issue is currently being worked on.
- Current generated shaders aren't tested for VR and for other rendering pipelines except forward Built-in. Extending the support with shadow caster pass, culing, depth etc. is being worked on, and can be done directly by users by extending the existing generators.
- The HLSL AST API is still in its's early stages and the smooth DX was preferred over performance for now. This should be currently a non-issue, as the generation isn't too frequent and the AST operations aren't a bottleneck in current realistic usage scenarios.
- The AST API implements currently only the necessary subset of HLSL and Shaderlab grammars. They should be easily extendable by users though, thanks to the presence of source generators which automatically build necessarry classes for the AST API.
- Currently the SDF scenes are implemented based on prefabs and shader assets. An attempt was made to use prefabs and sub-asssets, but this caused needless asset reimports on every save (causing regenerations). Another attempt was made with custom asset types, asset importers and postprocessors, but the API for that in Unity is quite limited, difficult to use and poorly documented. Soem work may be done in the future to use better format, but for now prefabs must suffice (additional benefit of prefabs is that they provide "a controllable skeleton" for SDF scenes).
- Performance in OpenGL on linux is so-so. It is highly recommended (often necesssary) to use Vulkan mode of the unity editor to work interactively with SDFs.
