# Third Party Notices

This package contains code and assets derived from or inspired by the following works.

## Roslyn

- Source: https://github.com/dotnet/roslyn
- License: MIT (https://github.com/dotnet/roslyn/blob/main/License.txt)
- Copyright: .NET Foundation and Contributors
- Use: the shape, structure and many design decisions of the AST library (`Runtime/AST`) are heavily based on the
  Roslyn compiler's Syntax API.

## HlslTools

- Source: https://github.com/tgjones/HlslTools
- License: Apache License 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
- Copyright: Tim Jones (tgjones)
- Use: several design decisions and parts of the code of the AST library are based on HlslTools.

## uRaymarching

- Source: https://github.com/hecomi/uRaymarching
- License: MIT (https://github.com/hecomi/uRaymarching)
- Copyright: hecomi
- Use: `ShaderLibrary/builtInForwardBase.hlsl` is adapted from uRaymarching's `ForwardBaseStandard.cginc`, and helpers
  in `ShaderLibrary/camera.hlsl` are based on uRaymarching.

## Shader Graph

- Source: Unity's `com.unity.shadergraph` package
- License: Unity Companion License (https://unity.com/legal/licenses/unity-companion-license)
- Use: parts of the controllers' serialization and asset logic were inspired by Shader Graph code.

## Notes

- Similarities in naming, structure and API design with the works above are deliberate, to ease the transfer of
  knowledge and possible future integration with those tools.
- Despite the similarities, the majority of the code was written by hand.
- Copyright of the respective files remains with their authors (tgjones, the Roslyn team, hecomi).

## Sample assets

- `Samples~/Examples/Shared/HDRI/lonely_road_afternoon_puresky_2k.hdr`: `lonely_road_afternoon_puresky` from Poly Haven
  (https://polyhaven.com/a/lonely_road_afternoon_puresky), CC0, downscaled to 2048x1024.
