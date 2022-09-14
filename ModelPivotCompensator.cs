using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

class ModelPivotCompensator : MonoBehaviour {
    enum OffsetOption { center, on, under }
    [SerializeField] Transform modelTransform = null;
    [SerializeField] Renderer modelRenderer = null;
    [SerializeField] Vector3 offset = Vector3.zero;
    [SerializeField] OffsetOption x = OffsetOption.center;
    [SerializeField] OffsetOption y = OffsetOption.center;
    [SerializeField] OffsetOption z = OffsetOption.center;
    public void Compensate() {
        Bounds bounds = this.modelRenderer.bounds;
        OffsetOption[] options = GetOffsetOptions().ToArray();
        Vector3 pos = new Vector3();
        for (int i = 0; i < 3; i++) {
            pos[i] = options[i] switch {
                OffsetOption.center => modelTransform.position[i] - bounds.center[i],
                OffsetOption.on => modelTransform.position[i] - bounds.center[i] + bounds.extents[i],
                OffsetOption.under => modelTransform.position[i] - bounds.center[i] - bounds.extents[i],
                _ => 0,
            };
        }
        pos += this.offset;
        Transform parent = this.modelTransform.parent;
        if (parent != null) {
            Matrix4x4 localToWorld = Matrix4x4.TRS(parent.position, parent.rotation, Vector3.one);
            this.modelTransform.position = localToWorld.MultiplyPoint(pos);
        } else this.modelTransform.position = pos;
    }
    /*public void SearchModel() {
        if (this.modelRenderer == null) this.modelRenderer = this.GetComponentInChildren<Renderer>();
        Transform parent = this.modelRenderer.transform.parent;
        if (this.modelTransform == null) {
            if (parent.parent != null) this.modelTransform = parent;
            else this.modelTransform = modelRenderer.transform;
        }
    }*/
    void OnDrawGizmos() {
        Bounds bounds;
        if (this.modelRenderer != null) {
            bounds = this.modelRenderer.bounds;
            Gizmos.DrawWireCube(bounds.center, 2 * bounds.extents);
        }
    }
    private IEnumerable<OffsetOption> GetOffsetOptions() {
        yield return this.x;
        yield return this.y;
        yield return this.z;
    }
}
[CustomEditor(typeof(ModelPivotCompensator))]
class ModelPivotCompensatorEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var _target = target as ModelPivotCompensator;
        /*if (GUILayout.Button("SearchModel")) {
            _target.SearchModel();
        }*/
        if (GUILayout.Button("Compensate")) {
            _target.Compensate();
        }
    }
}