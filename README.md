# SDF Manipulator

This is a monorepo for the SDF manipulator (work title) Unity Package. It implements a visual editor for SDF scenes using easy to use gizmos and composable primitives. The tool runs in realtime in the editor and generates ready to use HLSL shaders drawing the SDF scenes.

## Table of Contents

- [Repository layout](#repository-layout)
- [Gallery](#gallery)
- [Installing the package](#installing-the-package)
- [Development](#development)
- [Releasing](#releasing)
- [Documentation](#documentation)
- [Notes for contributors](#notes-for-contributors)

## Repository layout

| Path | Contents |
|---|---|
| [`packages/me.tooster.sdf/`](packages/me.tooster.sdf) | The Unity package: SDF primitives and operators, their visual controllers, shader generators, a collection of HLSL functions for raymarching shaders, and an HLSL/ShaderLab syntax tree API for assembling shader code. |
| [`projects/SdfManipulator/`](projects/SdfManipulator) | Unity project for developing and demoing the package; it references the package as a local package. `Assets/Examples/` holds self-contained SDF scenes (one folder per prefab with its generated shader and material) that should always work; `Assets/Sandbox/` holds experiments, test assets and playground scenes that may break; `Assets/DevTools/` holds editor tooling such as batchmode checks. |
| [`tools/SyntaxGenerators/`](tools/SyntaxGenerators) | Roslyn source generators behind the syntax tree API, independent of Unity. The API is modeled after Roslyn's Syntax API and builds and transforms ShaderLab and HLSL syntax trees, which is safer than string concatenation. Not optimized yet and in need of refactoring (don't judge me on the code quality now, :P), but it covers a big part of both languages. |
| [`docs/`](docs) | Design notes, diagrams and RenderDoc capture settings. |

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

## Installing the package

The package is pre-release and has no tagged versions yet. To try it in your own project, open
*Window > Package Manager*, choose *+ > Add package from git URL…* and enter:

```
https://github.com/T3sT3ro/SdfManipulator.git?path=/packages/me.tooster.sdf
```

It is developed and tested with Unity 6.6 (`6000.6.8f1`) and the Built-in Render Pipeline. Other versions and pipelines
are untested. The package ships an **Examples** sample; see the [documentation](#documentation) for how to use it.

## Development

1. `git clone` the repository.
2. In Unity Hub, add `projects/SdfManipulator` and open it with Unity `6000.6.8f1`.
    - Preferably start Unity with Vulkan, because it provides superior rendering performance. OpenGL has serious
      performance issues on Linux. To start with Vulkan, pass `-force-vulkan -force-gfx-mt` as extra command line
      arguments (can be set inside Unity Hub).
3. The package appears under *Packages* in the Project window; its files live in `packages/me.tooster.sdf/`.
4. After changing the source generators, rebuild them with
   `dotnet build -c Release tools/SyntaxGenerators/SyntaxGenerators/SyntaxGenerators.csproj`. The Release build
   copies `Generators.dll` into `packages/me.tooster.sdf/Runtime/AST/`.
5. `Assets/Examples` is the source of the package's **Examples** sample. After changing it, run `tools/sync-samples.sh`
   and commit both copies.
6. Checks that run without opening the editor (from the repository root):
   ```
   # EditMode tests of the package
   Unity -batchmode -projectPath projects/SdfManipulator -runTests -testPlatform EditMode -testResults results.xml
   # the package's runtime assemblies compile for a player
   Unity -batchmode -quit -projectPath projects/SdfManipulator -executeMethod SdfManipulator.DevTools.PlayerScriptsCheck.Run
   # shaders in Examples and the package compile
   Unity -batchmode -quit -projectPath projects/SdfManipulator -executeMethod SdfManipulator.DevTools.ShaderErrorsCheck.Run
   # the package sample matches Assets/Examples
   tools/sync-samples.sh --check
   ```

## Releasing

CI (`.github/workflows/ci.yml`) checks the package sample and the generators on every push and PR. Its Unity job needs
the `UNITY_LICENSE`, `UNITY_EMAIL` and `UNITY_PASSWORD` repository secrets ([GameCI activation](https://game.ci/docs/github/activation)).

1. Set `version` in `packages/me.tooster.sdf/package.json` and date its entry in the package's `CHANGELOG.md`
   (`## [X.Y.Z] - YYYY-MM-DD`).
2. Merge to `master`, tag that commit `vX.Y.Z` and push the tag.
3. `.github/workflows/release.yml` checks the version, changelog and sample, then publishes:
    - a GitHub release with the packed `me.tooster.sdf-X.Y.Z.tgz` and the changelog entry as notes,
    - the `upm` branch (the package at the repository root) and a `upm/vX.Y.Z` tag on it.

Users can then install `https://github.com/T3sT3ro/SdfManipulator.git#upm/vX.Y.Z`, or
`https://github.com/T3sT3ro/SdfManipulator.git?path=/packages/me.tooster.sdf#vX.Y.Z`.

## Documentation

Usage, concepts and known limitations are in the
[package documentation](packages/me.tooster.sdf/Documentation~/index.md).

## Notes for contributors

- `projects/SdfManipulator/Assets/Sandbox/Shaders/Raytracer.shader` is the "Über shader": the shader that started it
  all and has a bunch of code (also legacy code) ready to be ported to individual controllers or shader
  templates/partials. For some reason it doesn't render on vulkan at the time of writing (although it sometimes renders
  in the material preview???). It worked flawlessly on OpenGL on linux though.
- `tools/SyntaxGenerators` is a separate .NET solution (independent of Unity) for the syntax generator that generates
  the required partial classes for the package's AST classes. It uses Roslyn heavily.
- The source generator emits a syntax node's children in member declaration order, so code cleanup that rearranges
  members of syntax records changes the generated shader code. The package's shader generation test catches this.
