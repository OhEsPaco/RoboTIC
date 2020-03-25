using UnityEngine;

public class PhysicalCameraController : MonoBehaviour
{
    private Vector3 newPosition;

    private float yaw;
    private float pitch;
    private float roll;

    public float TargetYaw { get; set; }
    public float TargetPitch { get; set; }
    public float TargetRoll { get; set; }
    public float Yaw { get => yaw; }
    public float Pitch { get => pitch; }
    public float Roll { get => roll; }

    // Start is called before the first frame update
    private void Start()
    {
        pitch = transform.eulerAngles.x;
        yaw = transform.eulerAngles.y;
        roll = transform.eulerAngles.z;

        TargetPitch = transform.eulerAngles.x;
        TargetYaw = transform.eulerAngles.y;
        TargetRoll = transform.eulerAngles.z;

        newPosition = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        transform.eulerAngles = new Vector3(pitch, yaw, roll);
        transform.position = newPosition;
    }

    public void LerpTowards(in Vector3 targetPosition, in float positionLerpPct, in float rotationLerpPct)
    {
        yaw = Mathf.Lerp(Yaw, TargetYaw, rotationLerpPct);
        pitch = Mathf.Lerp(Pitch, TargetPitch, rotationLerpPct);
        roll = Mathf.Lerp(Roll, TargetRoll, rotationLerpPct);

        newPosition = Vector3.Lerp(transform.position, targetPosition, positionLerpPct);
    }
}