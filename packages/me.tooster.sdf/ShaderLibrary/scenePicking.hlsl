#pragma once

/*
 * Passes with which the scene view outlines selected objects and picks the object under the cursor. Like the other
 * passes, they march the ray of each fragment and write the depth of the hit, so outlines and picking follow the SDF
 * surfaces instead of the domain mesh. The uniforms are the ones that Unity's own shaders declare for these passes.
 */

#include "builtInShading.hlsl"

/// Set by the editor for the scene selection pass: the selected object and the outline pass
float _ObjectId;
float _PassValue;
/// Set by the editor for the picking pass: the picking index of the renderer, encoded as a color
float4 _SelectionID;

SdfVertexOutput sceneSelectionVertex(appdata_base v) { return sdfVertex(v); }

float4 sceneSelectionFragment(SdfVertexOutput i, out float depth : SV_Depth) : SV_Target {
    Ray3D     ray;
    SdfResult sdf = marchFragment(i.clipPos, ray);
    depth = clipSpaceDepth(sdf.p);
    return float4(_ObjectId, _PassValue, 1, 1);
}

SdfVertexOutput pickingVertex(appdata_base v) { return sdfVertex(v); }

float4 pickingFragment(SdfVertexOutput i, out float depth : SV_Depth) : SV_Target {
    Ray3D     ray;
    SdfResult sdf = marchFragment(i.clipPos, ray);
    depth = clipSpaceDepth(sdf.p);
    return _SelectionID;
}
