using UnityEngine;

public class ConeOfSightRenderer : MonoBehaviour
{
    private static readonly int sViewDepthTexturedID = Shader.PropertyToID("_ViewDepthTexture");
    private static readonly int sViewSpaceMatrixID = Shader.PropertyToID("_ViewSpaceMatrix");

    public Camera ViewCamera;
    public float ViewDistance;
    public float ViewAngle;
    private Material mMaterial;
    
    private void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        mMaterial = renderer.material;  // This generates a copy of the material
        renderer.material = mMaterial;

        RenderTexture depthTexture = new RenderTexture(ViewCamera.pixelWidth, ViewCamera.pixelHeight, 24, RenderTextureFormat.Depth);
        ViewCamera.targetTexture = depthTexture;
        ViewCamera.farClipPlane = ViewDistance;
        ViewCamera.fieldOfView = ViewAngle;

        transform.localScale = new Vector3(ViewDistance * 2, transform.localScale.y, ViewDistance * 2);

        mMaterial.SetTexture(sViewDepthTexturedID, ViewCamera.targetTexture);
        mMaterial.SetFloat("_ViewAngle", ViewAngle);
    }

    private void Update()
    {
        ViewCamera.Render();
        mMaterial.SetMatrix(sViewSpaceMatrixID, ViewCamera.projectionMatrix * ViewCamera.worldToCameraMatrix);
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(1f, 0f, 1f));
        Gizmos.DrawWireSphere(Vector3.zero, ViewDistance);
        Gizmos.matrix = Matrix4x4.identity;
    }

#endif
}