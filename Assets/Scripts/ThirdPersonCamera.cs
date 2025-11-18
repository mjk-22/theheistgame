using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2f, -4f);
    public float distance = 4f;
    public float mouseSensitivity = 3f;
    public float minPitch = -20f;
    public float maxPitch = 60f;

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

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPos = target.position + rotation * (offset.normalized * distance);

        transform.position = desiredPos;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}