using UnityEngine;
using UnityEditor;
using Unity.Jobs;
using Unity.Collections;

public class ConeOfSightRaycaster : MonoBehaviour
{

    private static readonly int sSightDepthBufferID = Shader.PropertyToID("_SightDepthBuffer");
    private static readonly int sBufferSize = 256;  // This has to match the shader buffer size

    public GameObject ConeOfSight;
	public float SightAngle;
	public float MaxDistance;
    public LayerMask Layermask;
	public bool DrawDebug;
    public bool UseJobSystem;

	private float[] m_aDepthBuffer;
	private Material m_ConeOfSightMat;


    // ----------------------------------------------------------

    void Start ()
    {
		Renderer renderer = ConeOfSight.GetComponent<Renderer>();
		m_ConeOfSightMat = new Material(renderer.material);
        renderer.material = m_ConeOfSightMat;

        m_aDepthBuffer = new float[sBufferSize];
	}

    // ----------------------------------------------------------

	private void Update()
    {
        UpdateViewDepthBuffer();
    }

    // ----------------------------------------------------------

    void OnDrawGizmos()
    {
		Handles.DrawWireArc(this.transform.localPosition,this.transform.up,Vector3.right,360,MaxDistance);

		float halfAngle = SightAngle/2 * Mathf.PI / 180;
		float viewAngle = this.transform.rotation.eulerAngles.y * Mathf.PI / 180;

		Vector3 p1 = GetRayEndPosition(-halfAngle - viewAngle, MaxDistance);
		Vector3 p2 = GetRayEndPosition(halfAngle - viewAngle, MaxDistance);

		Debug.DrawRay(this.transform.position, p1);
		Debug.DrawRay(this.transform.position, p2);

	}

    // ----------------------------------------------------------

    void UpdateViewDepthBuffer()
    {
        if (UseJobSystem)
        {
            UpdateViewDepthBufferWithJob();
        }
        else
        {
            UpdateViewDepthBufferImmediate();
        }
    }

    // ----------------------------------------------------------

    private void UpdateViewDepthBufferImmediate()
    {
        float angleStep = SightAngle / sBufferSize;
        float viewAngle = this.transform.rotation.eulerAngles.y;
        int bufferIndex = 0;

        for (int i = 0; i < sBufferSize; i++)
        {
            float angle = angleStep * i + (viewAngle - SightAngle / 2);

            Vector3 dest = GetRayEndPosition(-angle * Mathf.PI / 180, MaxDistance);
            Ray ray = new Ray(this.transform.position, dest);

            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit, MaxDistance, Layermask, QueryTriggerInteraction.Ignore))
            {
                m_aDepthBuffer[bufferIndex] = (hit.distance / MaxDistance);
            }
            else
            {
                m_aDepthBuffer[bufferIndex] = -1;
                if (DrawDebug)
                    Debug.DrawRay(this.transform.position, dest);
            }
            bufferIndex++;
        }

        m_ConeOfSightMat.SetFloatArray(sSightDepthBufferID, m_aDepthBuffer);
    }

    // ----------------------------------------------------------

    private void UpdateViewDepthBufferWithJob()
    {
        // Setup the command and result buffers
        var results = new NativeArray<RaycastHit>(sBufferSize, Allocator.TempJob);
        var commands = new NativeArray<RaycastCommand>(sBufferSize, Allocator.TempJob);

        float angleStep = SightAngle / sBufferSize;
        float viewAngle = this.transform.rotation.eulerAngles.y;

        for (int i = 0; i < sBufferSize; i++)
        {
            float angle = angleStep * i + (viewAngle - SightAngle / 2);

            Vector3 dest = GetRayEndPosition(-angle * Mathf.PI / 180, MaxDistance);
            Ray ray = new Ray(this.transform.position, dest);

            commands[i] = new RaycastCommand(ray.origin, ray.direction, MaxDistance, Layermask);
        }

        // Schedule the batch of raycasts
        JobHandle handle = RaycastCommand.ScheduleBatch(commands, results, 32, default(JobHandle));

        // Wait for the batch processing job to complete
        handle.Complete();

        // Convert result to float buffer
        for (int i = 0; i < sBufferSize; i++)
            m_aDepthBuffer[i] = results[i].distance / MaxDistance;

        m_ConeOfSightMat.SetFloatArray(sSightDepthBufferID, m_aDepthBuffer);

        // Dispose the buffers
        results.Dispose();
        commands.Dispose();
    }

    // ----------------------------------------------------------

    Vector3 GetRayEndPosition(float angle, float dist)
    {
		float x = Mathf.Cos(angle) * dist;
		float z = Mathf.Sin(angle) * dist;
		return new Vector3(x, 0, z);
	}

}
