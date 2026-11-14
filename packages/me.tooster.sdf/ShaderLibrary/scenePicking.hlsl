#pragma once

/*
 * Passes with which the scene view outlines selected objects and picks the object under the cursor. Like the other
 * passes, they march the ray of each fragment and write the depth of the hit, so outlines and picking follow the SDF
 * surfaces instead of the domain mesh. The uniforms are the ones that Unity's own shaders declare for these passes.
 */

#include "builtInShading.hlsl"

/// Evaluates the SDF object with the numeric ID \p id on its own, without the operations of its parents. Defined in the
/// generated shader.
SdfResult sdfObject(float3 p, int id);

/// Object IDs from this one up can't be picked or outlined on their own. Matches SdfScenePicking.MaxObjectIds.
#define SDF_MAX_OBJECT_IDS 512

/// Set by the editor for the scene selection pass: the selected object and the outline pass
float _ObjectId;
float _PassValue;
/// Set by SdfSceneOutline while it outlines selected objects of a scene: 1 to keep only the object IDs that have a
/// nonzero entry in _SdfOutlined. 0 while the editor outlines the whole scene.
float _SdfOutlineSelected;
float _SdfOutlined[SDF_MAX_OBJECT_IDS];

/// Returns whether the object ID \p id has a nonzero entry in \p flags, which has SDF_MAX_OBJECT_IDS entries
#define SDF_HAS_FLAG(flags, id) ((id) > 0 && (id) < SDF_MAX_OBJECT_IDS && (flags)[(id)] != 0)
/// Set by the editor for the picking pass: the picking index of the renderer, encoded as a color
float4 _SelectionID;

SdfVertexOutput sceneSelectionVertex(appdata_base v) { return sdfVertex(v); }

float4 sceneSelectionFragment(SdfVertexOutput i, out float depth : SV_Depth) : SV_Target {
    Ray3D     ray;
    SdfResult sdf = marchFragment(i.clipPos, ray);
    if (_SdfOutlineSelected != 0 && !SDF_HAS_FLAG(_SdfOutlined, (int)round(sdf.id.w)))
        discard;
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

#pragma region object picking

/// Set by SdfScenePicking: the picking index of object ID 0. Object ID n gets the index _SdfPickingIndex + n.
int   _SdfPickingIndex;
/// Set by SdfScenePicking: 1 for the object IDs that can be picked, 0 for the ones that the scene view skips
float _SdfPickable[SDF_MAX_OBJECT_IDS];

/// Encodes a picking index as a color, the same way as HandleUtility.EncodeSelectionId
float4 encodePickingIndex(uint index) {
    return float4(index & 0xFF, (index >> 8) & 0xFF, (index >> 16) & 0xFF, (index >> 24) & 0xFF) / 255.0;
}

SdfVertexOutput objectPickingVertex(appdata_base v) { return sdfVertex(v); }

/// Writes the picking index of the object that the ray hits. Objects that can't be picked are discarded, so the picking
/// pass of the same renderer identifies the whole scene there. The depth is moved a little toward the camera, so that
/// the object wins the depth test against that pass.
float4 objectPickingFragment(SdfVertexOutput i, out float depth : SV_Depth) : SV_Target {
    Ray3D     ray;
    SdfResult sdf = marchFragment(i.clipPos, ray);
    int       id = (int)round(sdf.id.w);
    if (!SDF_HAS_FLAG(_SdfPickable, id))
        discard;
    depth = clipSpaceDepth(sdf.p - ray.rd * max(1e-4 * ray.marchedDistance, 1e-5));
    return encodePickingIndex((uint)(_SdfPickingIndex + id));
}

#pragma endregion
