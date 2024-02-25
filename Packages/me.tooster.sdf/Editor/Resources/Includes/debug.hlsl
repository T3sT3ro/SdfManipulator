#pragma once
#include <HLSLSupport.cginc>

namespace sdf { namespace debug {
    fixed4 worldgrid(float3 gridPos, float spacing = .25, float lineWidth = .01) {
        gridPos = abs(fmod(abs(gridPos) / spacing, 1) - .5);
        fixed3 isGrid = gridPos >= .5 - lineWidth;
        // fixed3 isGrid = smoothstep(.5 - lineWidth, .5, gridPos); //
        fixed4 gridColor = float4(isGrid, length(smoothstep(.5 - lineWidth, .5, gridPos))); // simple any(isGrid) gives flat lines
        return gridColor;
    }

    fixed4 visualizeNormal(in float3 normal) {
        return fixed4(normal * .5 + .5, 1); // domain normal color
    }

    /// visualize overhead as blue for no overhead (0) and red for max overhead (1)
    fixed4 visualizeOverhead(in float overhead) {
        return lerp(fixed4(0, 0, 1, 1), fixed4(1, 0, 0, 1), overhead);
    }
}}
