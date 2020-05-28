using UnityEngine;

public class PhysicalBody : MonoBehaviour
{
    private Rigidbody rb;

    private Vector3 rotatedTranslation = new Vector3(0, 0, 0);

    public Vector3 RotatedTranslation { get => rotatedTranslation; set => rotatedTranslation = value; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 translation = rotatedTranslation * Time.fixedDeltaTime;

        rb.MovePosition(transform.position + translation);

        rb.velocity = new Vector3(0, 0, 0);
        rb.angularVelocity = new Vector3(0, 0, 0);
    }

    public void Teleport(Vector3 position)
    {
        rotatedTranslation.x = 0;
        rotatedTranslation.y = 0;
        rotatedTranslation.z = 0;
        rb.velocity = new Vector3(0, 0, 0);
        rb.angularVelocity = new Vector3(0, 0, 0);

        transform.position = position;
    }

    public Vector3 TargetPosition()
    {
        return rb.transform.position;
    }
}