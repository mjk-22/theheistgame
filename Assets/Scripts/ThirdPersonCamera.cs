using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Position")]
    public Vector3 offset = new Vector3(0f, 2f, -4f);
    public float distance = 4f;

    [Header("Mouse")]
    public float mouseSensitivity = 3f;
    public float minPitch = -20f;
    public float maxPitch = 60f;

    [Header("Collision")]
    public LayerMask wallLayer;
    public float collisionOffset = 0.3f;
    public float sphereRadius = 0.25f;

    float yaw;
    float pitch;

    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("ThirdPersonCamera: No target assigned!");
            return;
        }

        Vector3 e = transform.eulerAngles;
        yaw = e.y;
        pitch = e.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;
        if (GameManager.IsPaused) return;

        // Mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Desired camera position
        Vector3 desiredPos = target.position + rotation * (offset.normalized * distance);

        // Direction from target to camera
        Vector3 direction = desiredPos - target.position;
        float dist = direction.magnitude;

        if (dist > 0.01f)
        {
            direction.Normalize();

            if (Physics.SphereCast(
                target.position,
                sphereRadius,
                direction,
                out RaycastHit hit,
                dist,
                wallLayer))
            {
                desiredPos = hit.point - direction * collisionOffset;
            }
        }

        // Apply position
        transform.position = desiredPos;

        // Look at player (slightly above center)
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}