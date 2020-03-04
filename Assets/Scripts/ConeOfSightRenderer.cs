using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConeOfSightRenderer : MonoBehaviour
{
    public Camera ViewCamera;
    public float ViewDistance;

    [Header("---------- Debug ------------")]
    public Texture DebugTexture;

    private RenderTexture mDepthTexture;
    private RenderTexture mColorTexture;
    private Material mMaterial;

    void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        mMaterial = new Material(renderer.material);
        
        renderer.material = mMaterial;

        mColorTexture = new RenderTexture(ViewCamera.pixelWidth, ViewCamera.pixelHeight, 32, RenderTextureFormat.ARGB32);
        mDepthTexture = new RenderTexture(ViewCamera.pixelWidth, ViewCamera.pixelHeight, 32, RenderTextureFormat.Depth);
        
        DebugTexture = mDepthTexture;
        ViewCamera.depthTextureMode = DepthTextureMode.Depth;
        ViewCamera.farClipPlane = ViewDistance;
        ViewCamera.SetTargetBuffers(mColorTexture.colorBuffer, mDepthTexture.depthBuffer);

        transform.localScale = new Vector3(ViewDistance, transform.localScale.y, ViewDistance);
    }

    void Update()
    {
        ViewCamera.Render();
        mMaterial.SetTexture("_ViewDepthTexture", mDepthTexture);
        mMaterial.SetMatrix("_ViewSpaceMatrix", ViewCamera.projectionMatrix * ViewCamera.worldToCameraMatrix);
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, ViewDistance);
    }

#endif

}
