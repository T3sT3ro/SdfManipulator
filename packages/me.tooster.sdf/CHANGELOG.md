# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.1.0] - Unreleased

### Added

- **Examples** sample: example SDF scenes (Hello SDF, primitives, CSG box, bunny, rhino-rat) and a showcase scene.
- Package documentation in `Documentation~/index.md`.
- Editor tests: shader generation from the scene template must produce a shader that compiles; the HLSL syntax tests
  run again.
- Importing an SDF scene prefab generates its shader and a default material as sub-assets of the prefab, like Shader
  Graph does for a graph. Nothing generated is stored in the prefab file. In Prefab Mode, structural changes update the
  shader in memory before the prefab is saved.
- *Tools > SDF > Regenerate All Scene Shaders* reimports every SDF scene prefab.
- *Tools > SDF > Migrate Scenes* converts scenes that store a standalone `.shader` or an embedded shader and material,
  and gives controllers that have none an identifier. Material assets that used the old shader switch to the generated
  one.
- Printing tests for every HLSL and ShaderLab syntax node, and a test that fails when a node type has none. Tests check
  that walkers and mappers visit every token in syntax order.
- `Walker.Walk` and `Mapper.Map` run a visitor over a tree.
- In Prefab Mode, the shader also regenerates after undo and redo, and after components or objects are added,
  removed, reordered or moved.

### Changed

- License changed from MIT to Apache-2.0.
- The package lives in a monorepo at `packages/me.tooster.sdf`; install it from git with
  `?path=/packages/me.tooster.sdf`.
- Runtime components (`SdfScene`, controllers, data, shader presets and generators) moved from an assembly under
  `Editor/` to the runtime assembly `me.tooster.sdf`. Inspectors are in the editor-only assembly `me.tooster.sdf.Editor`.
- Namespaces were flattened: `me.tooster.sdf.Editor.Controllers.*` became `me.tooster.sdf`,
  `me.tooster.sdf.Primitives`, `me.tooster.sdf.Operators`, `me.tooster.sdf.Data` and `me.tooster.sdf.Generators`;
  inspectors are in `me.tooster.sdf.Editor.Inspectors`. Serialized shader presets load through `[MovedFrom]`.
- The HLSL library moved from `Editor/Resources/Includes` to `ShaderLibrary/`, and the fallback shader to `Shaders/`.
  SDF scenes made with earlier versions store the old include paths; update them or regenerate the scene.
- Minimum Unity version is 6000.6.
- Generated shaders no longer contain a timestamp, so the same scene always generates the same shader.
- Scenes regenerate when their prefab is imported: on save, after a change to the generator code (a custom asset
  dependency) and after a change to the shader library.
- Uniforms are set on each renderer with a `MaterialPropertyBlock` instead of in the material, and only the ones that
  changed. Instances of a scene prefab can have different values, and material assets no longer change while you edit.
- Generated shaders have no material properties for controller values and declare their uniforms without values, so
  changing a value or moving an object no longer changes the shader and recompiles it on save. Materials only hold
  the rendering settings, such as the draw mode. A renderer that no `SdfScene` updates, e.g. in a prefab preview,
  renders with all controller uniforms set to 0.
- Generated uniform and function names are built from an identifier that each controller gets once, e.g.
  `SDF_3f9a1c2b_Radius`. Renaming or moving objects no longer renames them, and no name contains `__`, which HLSL
  reserves. A comment above each generated function shows the object's place in the hierarchy. Run *Tools > SDF >
  Migrate Scenes* to give the controllers of existing scenes identifiers.
- Generated shaders are named `SDF/<prefab name> <first 8 characters of the prefab GUID>`, so scenes with the same name
  get different shaders. Slashes and double quotes in the name are replaced.
- The shader preset dropdown shows the scene's preset, and picking its type again keeps the preset's settings. The
  editor finds presets with `TypeCache`.

### Fixed

- Player builds failed to compile because runtime code used editor-only APIs.
- Generated ShaderLab was invalid: after a code reformat, argument and initializer lists printed their brackets after
  their contents (e.g. vector property defaults).
- Draw modes, the world grid and the ZWrite toggle did nothing, because the shader library's keyword pragmas were
  included with a plain `#include`. Generated shaders now use `#include_with_pragmas`; regenerate existing scenes.
- Generating a scene renamed every object with an elongate operator to `q`. The affected example objects got their
  names back.
- SDF scenes didn't work in player builds: the scene data was never built outside the editor, so every frame threw a
  `NullReferenceException` and no uniform was set.
- A scene without a controlled material threw an exception every frame; it now only shows the diagnostic. A Combine
  without children made generation fail; it now produces empty space.
- Scenes created with *Assets > Create > SDF > Scene Asset* could never regenerate their shader: they were copies of
  the template, which renders an embedded shader. A new scene now generates its own shader and default material.
- `#define` printed its argument list before the name (`#define () FOO`), so any define in a shader preset broke the
  shader.
- The *Regenerate shader* (formerly *Regenerate material*) and *Remove sub-assets* context menus of `SdfScene` never
  ran, and *Remove sub-assets* deleted a sub-asset when you clicked No.
- Shader library: shadow rays started a whole unit away from the surface; smooth union returned NaN for a blend factor
  of 0; orthographic depth was mirrored on platforms with reversed Z (D3D, Vulkan, Metal); the kink variant of the
  bend operator swapped the x and y axes.
- Moving an object with several controllers to another parent, for example moving one SDF scene into another,
  overflowed the stack. Controllers copied their parent's event subscribers and could end up subscribed to their own
  event. Controllers now notify their scene directly, and a scene stops listening to controllers that moved away.
- `Controller.ToString()` threw for a controller outside an SDF scene.
- `SeparatedList.Count` returned one less than the number of nodes for an odd-length list, and `Slice` dropped the
  separators and skipped the wrong nodes.
- ShaderLab `Blend` with separate RGB and alpha factors printed them in the wrong order.
- Token text: booleans printed as `True` and `False`, and system-value semantics had a "Semantic" suffix
  (`SV_TargetSemantic0`). Float literals were rounded to 6 decimals, so values below 0.000001 printed as `0.0`; they
  now print the shortest decimal that reads back as the same float, and NaN and infinity throw. A minus before a
  negative number printed `--`, which HLSL reads as a decrement.
- HLSL `switch` printed no braces, keyword or parentheses, `typedef` had no `;`, a parameter's default value had no
  `=`, and a call without arguments had no parentheses.
- `Walker` recursed forever and never visited statements and expressions. Tokens got the anchor of their grandparent,
  and nodes reached through a base type or a list were anchored to themselves.
- The generated mapper visited a node's own children before the ones of its base type, e.g. `--` before `i` in `i--`.
- HLSL formatter: a directive before a token in the middle of a tree ended up on the previous line, every `[` indented
  the rest of the file, the blank line before functions was never added, and matrix rows were indented by two levels.
  Formatted HLSL is also more conventional: `++i`, `a[0]`, `f(x);` in declarations, `} else`, `} while (x);`, `for`
  headers on one line and indented `case` labels. A ShaderLab texture default prints as `"white" {}`.

### Removed

- `SdfScene.targetShader` and `SdfScene.targetMaterial`. The generated shader is a sub-asset of the scene prefab.
- `SdfScene.controlledMaterial`. Uniforms go to the scene's renderers.
- `ShaderPreset.DetectedShaderPresets`, which scanned every loaded assembly. An assembly that failed to load made
  `SdfScene` fail to initialize.
- The serialized `SdfCombineController.children`. It was derived from the hierarchy and rewritten on load, which
  modified prefabs and created overrides. `Children` reads the hierarchy instead.

- The syntax generator sources are no longer shipped inside the package; they live in the repository's
  `tools/SyntaxGenerators`.

## [0.0.1] - WIP

### Added

- Generators for (a subset of) Hlsl and Shaderlab syntax trees with API similar to Roslyn
- Abstract (framework-agnostic) model for a shader node graph
- A collections of ready to use primitive graph nodes for constructing raymarching shaders
- Unity-specific controllers for creating SDF scenes
- code generator dll for generating syntax helpers
- a shim for enabling record init only properties to work in Unity
