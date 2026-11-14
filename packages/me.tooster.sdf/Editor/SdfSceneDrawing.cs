#nullable enable
using System.Collections.Generic;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

namespace me.tooster.sdf.Editor {
    /// <summary>Helpers for the editor views that draw SDF scenes with <see cref="Graphics.DrawMeshNow(Mesh, Matrix4x4)" />.</summary>
    [InitializeOnLoad]
    static class SdfSceneDrawing {
        // a copy of each renderer material, which holds the uniforms of the renderer's property block while it draws
        static readonly Dictionary<Material, Material> copies = new();
        static readonly MaterialPropertyBlock          block  = new();

        static SdfSceneDrawing() => AssemblyReloadEvents.beforeAssemblyReload += DestroyCopies;

        static void DestroyCopies() {
            foreach (var material in copies.Values)
                if (material != null)
                    Object.DestroyImmediate(material);
            copies.Clear();
        }

        /// <summary>
        ///     Returns a copy of <paramref name="source" /> with the uniforms of the renderer's property block and the scene's
        ///     rendering settings. Immediate drawing can't use a property block, and the views have to march the same rays
        ///     as the renderer.
        /// </summary>
        internal static Material MaterialWithUniforms(Material source, Renderer renderer, SdfScene scene) {
            // a copy of a material variant is a variant too, whose shader can't be set, so build the copy from the shader
            if (!copies.TryGetValue(source, out var material) || material == null || material.shader != source.shader) {
                if (material != null)
                    Object.DestroyImmediate(material);
                copies[source] = material = new Material(source.shader) { hideFlags = HideFlags.HideAndDontSave };
            }
            material.CopyPropertiesFromMaterial(source);
            CopyUniforms(material, renderer, scene);
            return material;
        }

        /// <summary>
        ///     Sets the uniforms of the renderer's property block, the scene's rendering settings and the scene's space on
        ///     <paramref name="material" />, so that the material draws the scene like the renderer does.
        /// </summary>
        /// <param name="onlyAssets">
        ///     Skip textures that aren't assets, e.g. the white texture of a missing map, for a material that is saved
        /// </param>
        internal static void CopyUniforms(Material material, Renderer renderer, SdfScene scene, bool onlyAssets = false) {
            renderer.GetPropertyBlock(block);
            foreach (var property in scene.sceneData.Properties) {
                var type = property.property.DeclaredValueType();
                if (type == typeof(float) || type == typeof(bool))
                    material.SetFloat(property.shaderId, block.GetFloat(property.shaderId));
                else if (type == typeof(int))
                    material.SetInteger(property.shaderId, block.GetInteger(property.shaderId));
                else if (type == typeof(Matrix4x4))
                    material.SetMatrix(property.shaderId, block.GetMatrix(property.shaderId));
                else if (type == typeof(Texture2D) || type == typeof(Texture3D)) {
                    var texture = block.GetTexture(property.shaderId);
                    if (!onlyAssets || (texture != null && EditorUtility.IsPersistent(texture)))
                        material.SetTexture(property.shaderId, texture);
                }
                else // vectors and colors, which the block already holds in linear space
                    material.SetVector(property.shaderId, block.GetVector(property.shaderId));
            }
            scene.rendering.ApplyTo(material);
            material.SetMatrix(SdfScene.worldToSceneId, scene.transform.worldToLocalMatrix);
            material.SetFloat(SdfScene.sceneDistanceScaleId, TransformController.DistanceScaleOf(scene.transform.worldToLocalMatrix));
        }

        /// <summary>
        ///     Calls <paramref name="draw" /> with each mesh renderer of <paramref name="scene" /> that the scene view shows,
        ///     its mesh and the index of the pass <paramref name="passName" /> in its material.
        /// </summary>
        internal static bool ForEachRenderer(SdfScene scene, string passName, Camera? camera, System.Action<MeshRenderer, Mesh, Material, int> draw) {
            var drawn = false;
            foreach (var renderer in scene.Renderers) {
                if (renderer is not MeshRenderer { enabled: true } meshRenderer
                 || !meshRenderer.gameObject.activeInHierarchy
                 || (camera != null && (camera.cullingMask & (1 << meshRenderer.gameObject.layer)) == 0)
                 || SceneVisibilityManager.instance.IsHidden(meshRenderer.gameObject)
                 || !meshRenderer.TryGetComponent<MeshFilter>(out var meshFilter)
                 || meshFilter.sharedMesh == null
                 || meshRenderer.sharedMaterial is not { } source)
                    continue;
                var pass = source.FindPass(passName);
                if (pass < 0)
                    continue;
                draw(meshRenderer, meshFilter.sharedMesh, source, pass);
                drawn = true;
            }
            return drawn;
        }
    }
}
