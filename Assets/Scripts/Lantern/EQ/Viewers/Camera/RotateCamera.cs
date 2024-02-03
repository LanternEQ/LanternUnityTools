using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public float rotationSpeed = 5.0f;
    public float pitchRange = 80.0f;

    private float pitch = 0.0f;
    private float yaw = 0.0f;

    public float smoothSpeed = 10.0f;
    private Vector2 currentRotation = Vector2.zero;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y"); // Inverted for natural mouse movement

        yaw += mouseX * rotationSpeed;
        pitch += mouseY * rotationSpeed;

        // Clamp the pitch to prevent looking straight up or down
        pitch = Mathf.Clamp(pitch, -pitchRange, pitchRange);

        // Smooth the rotation
        currentRotation = Vector2.Lerp(currentRotation, new Vector2(pitch, yaw), smoothSpeed * Time.deltaTime);

        transform.eulerAngles = new Vector3(currentRotation.x, currentRotation.y, 0.0f);
    }
}
