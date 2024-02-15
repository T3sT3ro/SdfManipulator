#pragma once
#include <HLSLSupport.cginc>

namespace sdf { namespace debug {
    fixed4 worldgrid(float3 gridPos, float spacing = .25, float lineWidth = .01) {
        gridPos = fmod(abs(gridPos) / spacing, 1);
        fixed3 isGrid = gridPos <= lineWidth || gridPos >= 1 - lineWidth;
        fixed4 gridColor = float4(isGrid, any(isGrid));
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
