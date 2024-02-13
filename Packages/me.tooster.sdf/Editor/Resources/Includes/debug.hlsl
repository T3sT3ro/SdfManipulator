#include <HLSLSupport.cginc>

namespace sdf { namespace debug {
    fixed4 worldgrid(float3 gridPos, float spacing = .25, float lineWidth = .01) {
        gridPos = fmod(abs(gridPos) / spacing, 1);
        fixed3 isGrid = gridPos <= lineWidth || gridPos >= 1 - lineWidth;
        fixed4 gridColor = float4(isGrid, any(isGrid));
        return gridColor;
    }
}}
