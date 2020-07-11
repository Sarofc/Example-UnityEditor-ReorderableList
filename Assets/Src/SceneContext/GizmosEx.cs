using UnityEngine;
using UnityEditor;

public class GizmosEx
{
    static bool IsZoomed(Transform t)
    {
        return (Camera.current.transform.position - t.position).magnitude <= 20;
    }

    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
    static void DrawGizmos(PrefabExample i, GizmoType type)
    {
        if (IsZoomed(i.transform))
        {

        }
        else
        {
            Gizmos.DrawIcon(i.transform.position, "cm_logo_lg");
        }
    }
}