using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target to follow")]
    public Transform target; // The player

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 5, -10); // Camera offset
    public float followSpeed = 5f; // How fast the camera follows

    void LateUpdate()
    {
        if (target == null) return;

        // Desired position
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move camera
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Optional: make camera look at the player
        transform.LookAt(target);
    }
}
