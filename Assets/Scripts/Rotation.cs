using UnityEngine;

public class Rotation : MonoBehaviour {

	public float Speed= 3000f;

    private Rigidbody m_Rigidbody;

    // ----------------------------------------------------------

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // ----------------------------------------------------------

    void FixedUpdate()
    {
        if (m_Rigidbody)
            m_Rigidbody.MoveRotation(this.transform.rotation * Quaternion.Euler(Vector3.up * Speed * Time.deltaTime));
        else
            this.transform.Rotate(Vector3.up * Speed * Time.deltaTime);
    }

}
