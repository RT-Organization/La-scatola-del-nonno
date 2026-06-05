using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Camera rotation sensitivity and player reference
    public float mouseSensitivity = 2.5f;
    [SerializeField] private Transform playerBody;

    // Tracks vertical rotation to clamp view angle
    private float xRotation = 0f;

    private void Start()
    {
        // Lock cursor to center of screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Handle camera rotation every frame
        HandleCameraRotation();
    }

    /// <summary>
    /// Handles both horizontal and vertical camera rotation
    /// </summary>
    private void HandleCameraRotation()
    {
        // Get mouse input and apply sensitivity
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Calculate vertical rotation with clamping
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -50f, 50f);

        // Apply rotations
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // Vertical (camera)
        playerBody.Rotate(Vector3.up * mouseX); // Horizontal (player body)
    }
}