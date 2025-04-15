using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; } // Singleton instance
    public float moveSpeed = 10f; // Speed of movement
    public float mouseSensitivity = 100f; // Sensitivity of mouse movement

    private float pitch = 0f; // Rotation around the X-axis
    private float yaw = 0f; // Rotation around the Y-axis
    private Rigidbody rb; // Reference to the Rigidbody component

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
        {
            Instance = this; // Set the singleton instance
        }

        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        rb.useGravity = false; // Disable gravity for the camera
        rb.freezeRotation = true; // Prevent the Rigidbody from rotating
    }

    public void SetCameraStartLocation(Vector3 startLocation)
    {
        transform.position = startLocation; // Set the camera's position to the specified start location
    }

    // Update is called once per frame
    void Update()
    {
        // Handle movement
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxis("Vertical"); // W/S or Up/Down
        Vector3 movement = transform.right * horizontal + transform.forward * vertical;
        rb.linearVelocity = movement * moveSpeed; // Set the Rigidbody's velocity

        // Handle rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f); // Limit pitch to avoid flipping

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
