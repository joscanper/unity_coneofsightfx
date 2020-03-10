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
        /*
        
        mMaterial = new Material(renderer.material);
        renderer.material = mMaterial;
        mMaterial = renderer.sharedMaterial;
        */



        RenderTexture colorTexture = new RenderTexture(ViewCamera.pixelWidth, ViewCamera.pixelHeight, 0, RenderTextureFormat.ARGB32);
        RenderTexture depthTexture = new RenderTexture(ViewCamera.pixelWidth, ViewCamera.pixelHeight, 32, RenderTextureFormat.Depth);
        
        ViewCamera.depthTextureMode = DepthTextureMode.Depth;
        ViewCamera.farClipPlane = ViewDistance;
        ViewCamera.SetTargetBuffers(colorTexture.colorBuffer, depthTexture.depthBuffer);
        ViewCamera.fieldOfView = ViewAngle;

        transform.localScale = new Vector3(ViewDistance, transform.localScale.y, ViewDistance);

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
