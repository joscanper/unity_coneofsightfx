using UnityEngine;

#if UNITY_EDITOR

public class DebugGizmo : MonoBehaviour
{
    public enum GizmoType
    {
        Sphere,
        Cube,
        Cross,
        AxisAlignedCross,
        LocalMatrixCube,
        Collider,
        ForwardArrow
    }

    public float Size = 1f;
    public GizmoType Type = GizmoType.Sphere;
    public Color Color = Color.white;
    public bool OnlySelected = false;
    public bool Wired = true;

    // --------------------------------------------------------------------

    private void OnDrawGizmos()
    {
        if (!OnlySelected)
            DrawGizmo();
    }

    // --------------------------------------------------------------------

    private void OnDrawGizmosSelected()
    {
        if (OnlySelected)
            DrawGizmo();
    }

    // --------------------------------------------------------------------

    private void DrawGizmo()
    {
        if (Size == 0f)
        {
            return;
        }

        Gizmos.color = Color;
        switch (Type)
        {
            case GizmoType.Sphere:
                if (Wired)
                    Gizmos.DrawWireSphere(transform.position, Size * 0.5f);
                else
                    Gizmos.DrawSphere(transform.position, Size * 0.5f);
                break;

            case GizmoType.Cube:
                if (Wired)
                    Gizmos.DrawWireCube(transform.position, Size * Vector3.one);
                else
                    Gizmos.DrawCube(transform.position, Size * Vector3.one);
                break;

            case GizmoType.Cross:

                if (Wired)
                {
                    Gizmos.DrawLine(transform.position - transform.right * Size * 0.5f, transform.position + transform.right * Size * 0.5f);
                    Gizmos.DrawLine(transform.position - transform.forward * Size * 0.5f, transform.position + transform.forward * Size * 0.5f);
                    Gizmos.DrawLine(transform.position - transform.up * Size * 0.5f, transform.position + transform.up * Size * 0.5f);
                }
                else
                {
                    Gizmos.matrix = transform.localToWorldMatrix;
                    float lineW = 0.01f;
                    Gizmos.DrawCube(Vector3.zero, new Vector3(Size, lineW, lineW));
                    Gizmos.DrawCube(Vector3.zero, new Vector3(lineW, Size, lineW));
                    Gizmos.DrawCube(Vector3.zero, new Vector3(lineW, lineW, Size));
                    Gizmos.matrix = Matrix4x4.identity;
                }

                break;

            case GizmoType.AxisAlignedCross:
                if (Wired)
                {
                    Gizmos.DrawLine(transform.position - Vector3.right * Size * 0.5f, transform.position + Vector3.right * Size * 0.5f);
                    Gizmos.DrawLine(transform.position - Vector3.forward * Size * 0.5f, transform.position + Vector3.forward * Size * 0.5f);
                    Gizmos.DrawLine(transform.position - Vector3.up * Size * 0.5f, transform.position + Vector3.up * Size * 0.5f);
                }
                else
                {
                    float lineW = 0.01f;
                    Gizmos.DrawCube(transform.position, new Vector3(Size, lineW, lineW));
                    Gizmos.DrawCube(transform.position, new Vector3(lineW, Size, lineW));
                    Gizmos.DrawCube(transform.position, new Vector3(lineW, lineW, Size));
                }
                break;

            case GizmoType.LocalMatrixCube:
                Gizmos.matrix = transform.localToWorldMatrix;
                if (Wired)
                    Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                else
                    Gizmos.DrawCube(Vector3.zero, Vector3.one);
                Gizmos.matrix = Matrix4x4.identity;
                break;

            case GizmoType.ForwardArrow:
                Gizmos.DrawLine(transform.position, transform.position + transform.forward * Size);

                Vector3 cross = Vector3.Cross(transform.forward, transform.forward == Vector3.forward ? Vector3.up : Vector3.forward).normalized;
                Vector3 perpendicular1 = Vector3.Cross(transform.forward, cross);

                Vector3 perpendicular2 = Vector3.Cross(transform.forward, perpendicular1);

                Gizmos.DrawLine(transform.position + transform.forward * Size, transform.position + (transform.forward * Size * 0.5f - perpendicular1 * Size * 0.25f));
                Gizmos.DrawLine(transform.position + transform.forward * Size, transform.position + (transform.forward * Size * 0.5f + perpendicular1 * Size * 0.25f));
                Gizmos.DrawLine(transform.position + transform.forward * Size, transform.position + (transform.forward * Size * 0.5f - perpendicular2 * Size * 0.25f));
                Gizmos.DrawLine(transform.position + transform.forward * Size, transform.position + (transform.forward * Size * 0.5f + perpendicular2 * Size * 0.25f));

                break;
        }
    }
}

#endif