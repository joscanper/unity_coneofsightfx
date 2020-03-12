using UnityEngine;

public class ConeOfSightRenderer : MonoBehaviour
{
    private static readonly int sViewDepthTexturedID = Shader.PropertyToID("_ViewDepthTexture");
    private static readonly int sViewSpaceMatrixID = Shader.PropertyToID("_ViewSpaceMatrix");

    public Camera ViewCamera;
    public float ViewDistance;
    public float ViewAngle;

    private Material mMaterial;

    void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        mMaterial = renderer.material;  // This generates a copy of the material
        renderer.material = mMaterial;
        
        RenderTexture depthTexture = new RenderTexture(ViewCamera.pixelWidth, ViewCamera.pixelHeight, 32, RenderTextureFormat.Depth);

        ViewCamera.depthTextureMode = DepthTextureMode.Depth;
        ViewCamera.farClipPlane = ViewDistance;
        ViewCamera.SetTargetBuffers(depthTexture.colorBuffer, depthTexture.depthBuffer);
        ViewCamera.fieldOfView = ViewAngle;

        transform.localScale = new Vector3(ViewDistance * 2, transform.localScale.y, ViewDistance *2);

        mMaterial.SetTexture(sViewDepthTexturedID, depthTexture);
        mMaterial.SetFloat("_ViewAngle", ViewAngle);
    }

    void Update()
    {
        ViewCamera.Render();
        
        mMaterial.SetMatrix(sViewSpaceMatrixID, ViewCamera.projectionMatrix * ViewCamera.worldToCameraMatrix);
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, ViewDistance);
    }

#endif

}
