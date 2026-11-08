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
- *Tools > SDF > Regenerate All Scene Shaders* regenerates the shader of every SDF scene prefab without opening it.

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
- Generated shaders no longer contain a timestamp, and regeneration skips the write when the shader didn't change. It
  imports only the shader file instead of refreshing the whole AssetDatabase.

### Fixed

- Player builds failed to compile because runtime code used editor-only APIs.
- Generated ShaderLab was invalid: after a code reformat, argument and initializer lists printed their brackets after
  their contents (e.g. vector property defaults).
- Draw modes, the world grid and the ZWrite toggle did nothing, because the shader library's keyword pragmas were
  included with a plain `#include`. Generated shaders now use `#include_with_pragmas`; regenerate existing scenes.
- Regeneration could overwrite a prefab with shader source when the target shader was a prefab sub-asset. It now only
  writes to standalone `.shader` files and logs an error otherwise.
- Generating a scene renamed every object with an elongate operator to `q`. The affected example objects got their
  names back.

### Removed

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
