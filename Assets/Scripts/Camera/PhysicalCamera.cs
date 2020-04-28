using UnityEngine;

public class PhysicalCamera : MonoBehaviour
{
    [SerializeField] private PhysicalBody physicalBody;
    [SerializeField] private PhysicalCameraController cameraController;
    [SerializeField] private CamCage cage;

    [Header("Rotation Settings")]
    [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
    public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

    [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
    public bool invertY = false;

    [Header("Movement Settings")]
    [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
    public float boost = 3.5f;

    [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
    public float positionLerpTime = 0.2f;

    [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
    public float rotationLerpTime = 0.01f;

    // Update is called once per frame
    private void Update()
    {
        // Exit Sample
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        // Hide and lock cursor when right mouse button pressed
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        // Unlock and show cursor when right mouse button released
        if (Input.GetMouseButtonUp(1))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        // Rotation
        if (Input.GetMouseButton(1))
        {
            Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * (invertY ? 1 : -1));

            float mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

            cameraController.TargetYaw += mouseMovement.x * mouseSensitivityFactor;
            cameraController.TargetPitch += mouseMovement.y * mouseSensitivityFactor;
        }

        Vector3 translation = GetInputTranslationDirection();

        // Speed up movement when shift key held
        if (Input.GetKey(KeyCode.LeftShift))
        {
            translation *= 10.0f;
        }

        // Modify movement by a boost factor (defined in Inspector and modified in play mode through the mouse scroll wheel)
        boost += Input.mouseScrollDelta.y * 0.2f;
        translation *= Mathf.Pow(2.0f, boost);

        // Framerate-independent interpolation
        // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
        float positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
        float rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);

       

        Vector3 rotatedTranslation = Quaternion.Euler(cameraController.TargetPitch, cameraController.TargetYaw, cameraController.TargetRoll) * translation;

        Vector3 adjTrans;
        Vector3 adjPos;
        bool x;
        bool y;
        bool z;

        cage.AdjustTranslation(physicalBody.transform.position,rotatedTranslation, out adjTrans,out adjPos,out x, out y, out z);

        if (x)
        {
            rotatedTranslation.x = 0;
          
        }

        if (y)
        {
            rotatedTranslation.y = 0;
           
        }

        if (z)
        {
            rotatedTranslation.z = 0;
           
        }
        physicalBody.RotatedTranslation = rotatedTranslation;
        cameraController.LerpTowards(physicalBody.TargetPosition(), positionLerpPct, rotationLerpPct);


       /* if (x || y || z)
        {
            //cameraController.transform.position=adjPos;
            physicalBody.Teleport(adjPos);
            cameraController.LerpTowards(adjPos, positionLerpPct, rotationLerpPct);
        }
        else
        {
            cameraController.LerpTowards(physicalBody.TargetPosition(), positionLerpPct, rotationLerpPct);
        }*/

       
    }

    private Vector3 GetInputTranslationDirection()
    {
        Vector3 direction = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector3.right;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            direction += Vector3.down;
        }
        if (Input.GetKey(KeyCode.E))
        {
            direction += Vector3.up;
        }
        return direction;
    }
}