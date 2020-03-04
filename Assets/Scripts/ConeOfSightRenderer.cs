using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConeOfSightRenderer : MonoBehaviour
{
    public Camera ViewCamera;

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
        
        ViewCamera.SetTargetBuffers(mColorTexture.colorBuffer, mDepthTexture.depthBuffer);
        
    }

    void Update()
    {
        ViewCamera.Render();
        mMaterial.SetTexture("_ViewDepthTexture", mDepthTexture);
        mMaterial.SetMatrix("_ViewSpaceMatrix", ViewCamera.projectionMatrix * ViewCamera.worldToCameraMatrix);
    }

}
