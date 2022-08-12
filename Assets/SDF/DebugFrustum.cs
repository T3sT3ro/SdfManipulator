using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class Util {
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source) {
        return source.Select((item, index) => (item, index));
    }
}


public class DebugFrustum : MonoBehaviour {
    public Camera         _camera;
    public MeshRenderer[] renderers;
    
    private void Start() {}

    // Start is called before the first frame update
    private void OnDrawGizmos() {
        if(!_camera) _camera      = GetComponent<Camera>();
        
        if(!enabled) return;
        foreach (var renderer in renderers) {
            if (!renderer.isVisible || SceneVisibilityManager.instance.IsHidden(renderer.gameObject)) continue;
            
            visualizePoints(renderer.GetComponent<MeshFilter>());
        }
    }

    private void visualizePoints(MeshFilter mf) {
        var M = mf.transform.localToWorldMatrix;
        var V = _camera.worldToCameraMatrix;
        var P = _camera.projectionMatrix;

        Gizmos.color = Color.gray;
        // Gizmos.matrix = P*V;
        // Gizmos.DrawFrustum(_camera.transform.position, _camera.fieldOfView, _camera.farClipPlane, _camera.nearClipPlane, _camera.aspect);
        CameraEditorUtils.DrawFrustumGizmo(_camera);


        Gizmos.color = Handles.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 2);
        foreach (var (pos, i) in new Vector3[]
                 {
                     new(-1, -1, -1),
                     new(1, 1, 1),
                 }.WithIndex()) {
            var p = (P * V).inverse.MultiplyPoint(pos);
            Gizmos.DrawRay(transform.position, p - transform.position);
            drawAnnotatedPoint(pos, $"{i}");
            drawAnnotatedPoint(p, $"{i}");
        }

        Dictionary<Vector3, List<int>> verts = new Dictionary<Vector3, List<int>>();
        foreach (var (v, i) in mf.sharedMesh.vertices.WithIndex()) {
            List<int> labels;
            if (!verts.TryGetValue(v, out labels)) {
                labels   = new List<int>();
                verts[v] = labels;
            }
            labels.Add(i);
        }
        foreach (var (v, l) in verts) {
            var p = (P * V * M).MultiplyPoint(v); // clip pos
            var i = String.Join(",", l);
            Gizmos.color = Color.green;
            drawAnnotatedPoint(p, $"v{i}");
            drawAnnotatedPoint(M.MultiplyPoint(v), $"v{i}");
    
            var ro = new Vector3(p.x, p.y, -1);
            var re = new Vector3(p.x, p.y, 1);
            var f_ro = (P * V).inverse.MultiplyPoint(ro);
            var f_re = (P * V).inverse.MultiplyPoint(re);
            
            Gizmos.color = Color.cyan;
            drawAnnotatedPoint(ro, $"ro_{i}");
            drawAnnotatedPoint(f_ro, $"ro_{i}");

            Gizmos.color = Color.blue;
            drawAnnotatedPoint(re, $"re_{i}");
            drawAnnotatedPoint(f_re, $"re_{i}");
            Gizmos.DrawRay(ro, re - ro);
            Gizmos.DrawRay(f_ro, f_re - f_ro);
        }
    }

    private static void drawAnnotatedPoint(Vector3 pos, string text) {
        Gizmos.DrawSphere(pos, .01f);
        Handles.Label(pos + new Vector3(0f, .03f, 0), text);
    }
}
