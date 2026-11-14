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
- Generated shaders have a forward add pass for each additional pixel light. Light attenuation is computed at the
  raymarched hit instead of at the domain mesh.
- Generated shaders have a shadow caster pass, so SDF scenes cast shadows and write their raymarched depth into the
  camera depth texture. SDF scenes receive realtime shadows, which are sampled at the raymarched hit.
- *SDF Material* components give SDF objects the properties of the Standard material: albedo, metallic, smoothness
  and emission. A material applies to the SDF object on its game object and to the objects below it that have no
  material of their own, so one material on the root combination colors the whole scene. Objects without a material
  are white. The values are uniforms, so changing them doesn't regenerate the shader. `Color` properties of
  controllers become `float4` uniforms.
- Generated shaders have scene selection and picking passes, so the scene view outlines a selected scene along its
  SDF surfaces and picks it only where its rays hit, instead of anywhere on its domain mesh.
- Clicking an SDF surface in the scene view picks the object that the ray hit. The first click selects the scene,
  which is a selection base, and the next click the object. Objects with picking disabled are skipped.
- The scene view outlines selected SDF objects, and a selected combination with all of its objects.
- Generated shaders have a function that evaluates one object by its ID, `sdfObject(p, id)`, for editor views of
  single objects.
- The scene view shows selected SDF objects on their own as translucent ghosts, also where other objects hide them or
  an operation removes them, e.g. an object subtracted from another. *Tools > SDF > Show Selected Objects* turns it on
  and off (#37).

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
  the render state. A renderer that no `SdfScene` updates, e.g. in a prefab preview, renders nothing, because all
  its uniforms are 0.
- Generated uniform and function names are built from an identifier that each controller gets once, e.g.
  `SDF_3f9a1c2b_Radius`. Renaming or moving objects no longer renames them, and no name contains `__`, which HLSL
  reserves. A comment above each generated function shows the object's place in the hierarchy. Run *Tools > SDF >
  Migrate Scenes* to give the controllers of existing scenes identifiers.
- Generated shaders are named `SDF/<prefab name> <first 8 characters of the prefab GUID>`, so scenes with the same name
  get different shaders. Slashes and double quotes in the name are replaced.
- The shader preset dropdown shows the scene's preset, and picking its type again keeps the preset's settings. The
  editor finds presets with `TypeCache`.
- Controllers detect structural changes in `OnValidate` by comparing the values of their `[ShaderStructural]`
  properties, so edits that bypass the property setters regenerate the shader: the default inspector, undo, and
  editing several objects at once. The inspector no longer notifies controllers itself and no longer relies on a
  property with the capitalized name of each field to do so. The modifier list of an SDF controller and the target of
  an elongation are structural.
- Inspector layouts live in `Editor/UI` and load by their package path instead of from `Resources`, where their names
  could collide with files of other packages.
- The draw mode, the world grid and depth output are switches on uniforms instead of shader keywords, so changing
  them compiles no shader variant. Forward passes always output the depth of the hit, which the depth test uses also
  without ZWrite.
- The render state that commands read (`_Cull`, `_ZTest`, `_ZWrite`) uses `Float` material properties, like Unity's
  own shaders, instead of the legacy `Int` type, and the step counts are `int` uniforms.
- Combinations can subtract their other children from the first one, and intersect and subtract with a smooth edge.
  Where a subtraction cuts a surface, the surface belongs to the cutting object, so picking it selects that object.
- *Clip To Domain Box* in the rendering settings marches rays only inside the box of the domain mesh: they start
  where they enter the box and stop where they leave it, which skips the empty space in front of the domain, also in
  shadow maps (#111).
- The draw mode, the world grid and the raymarching settings are in the *Rendering* section of `SdfScene` instead of
  in the material. `SdfScene` sets them with its property block, so they're saved with the scene, prefab instances can
  override them, and regenerating the shader on save no longer resets them. The generated material only holds the
  render state. Material assets made with earlier versions keep their render state; set the other values on the
  scene.
- ShaderLab `Pass` blocks can hold tags and commands, and `On` is a command argument, e.g. `ZWrite On`.
- Each fragment builds its ray from the view and projection matrices, instead of each vertex from camera parameters.
- The scene functions are defined once, in an include block that all passes share, and the pragmas of a shader
  preset select the entry points of the forward base pass. Generated shaders disable batching, which would move the
  vertices of the domain mesh into world space.
- The *Material* draw mode uses the lighting of the Standard shader (`UnityPBSLighting.cginc`) with ambient light,
  reflection probes and fog, instead of a Lambert term and a shadow ray toward the main light. Each object is shaded
  with its material, which the shader reads by evaluating the scene once more at the hit. The forward passes are in `ShaderLibrary/builtInShading.hlsl`; `debugBaseShading.hlsl`
  includes it, for presets that list the old file.

### Fixed

- *GameObject > SDF* menus put the new object under the right-clicked object instead of the selection, in Prefab
  Mode into the prefab instead of the main scene, and the creation of operators can be undone (#77).
- Undo didn't revert dragging the scene handles of primitives, because the handles changed the controller before
  undo recorded it (#76).
- Rounded cylinders were taller by twice their rounding; the rounding now stays within their height (#100).
- Shader library helpers that nothing used yet: `repeat` mirrored negative coordinates, `elongate_fast` returned a
  float, `m_scale` was singular, `euler_to_rot` swapped two rows, `decompose` read the translation from the wrong row,
  the domain matrices of `domain.hlsl` were all scale matrices, `normalSdfOcclusion` compared with the square of the
  step, `depthToMaxRayDepth` normalized a `float4` and had a declaration that didn't match it, and `noise.hlsl` leaked
  its `A0` to `D0` macros (#100).
- Scene handles ignored the scale of the object and its parents, so they didn't match the rendered shape (#75).
- `[VisibleWhen]` compared enum values with their position in the enum's declaration (#80).
- The `SdfScene` inspector showed fields for the controlled material and the target shader and material paths, which
  no longer exist.
- Player builds failed to compile because runtime code used editor-only APIs.
- Object IDs were `fixed4`, which has a range of about ±2 on mobile, so IDs from 2 up collapsed into one (#99).
- Rays started at the focal length times the near distance from the camera instead of on the near plane, so geometry
  closer than that was cut flat (#97). Triangles of the domain mesh that cross the camera plane, e.g. with the camera
  inside the domain, produced wrong rays (#98).
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

- `ShaderLibrary/builtInForwardBase.hlsl`, an unused and untested adaptation of uRaymarching's forward pass, with
  `SdfPaths.ShaderLibrary.BuiltInForwardBase`; `cameraRayFromClipPos` and `cameraVsRayFromClipPos` from `camera.hlsl`;
  the `v2f` struct and `V2F_RAYS` from `raymarching.hlsl`.
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
