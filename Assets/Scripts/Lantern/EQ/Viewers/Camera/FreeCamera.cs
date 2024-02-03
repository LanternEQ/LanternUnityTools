using UnityEngine;

namespace Lantern.EQ.Viewers
{
    public class FreeCamera : CameraBase
    {
        [SerializeField]
        private float movementSpeed = 5f;

        [SerializeField, Range(1f, 1040f)]
        private float rotationSpeed = 360f;

        [SerializeField, Range(-90f, 90f)]
        private float minVerticalAngle = -90f;

        [SerializeField, Range(-90f, 90f)]
        private float maxVerticalAngle = 90f;

        private Vector2 rotationAngles;

        private void Update()
        {
            HandleMovementInput();
            HandleRotationInput();
            ConstrainAngles();
        }

        private void HandleMovementInput()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(horizontal, 0f, vertical) * movementSpeed * Time.deltaTime;
            transform.Translate(movement);
        }

        private void HandleRotationInput()
        {
            Vector2 mouseInput = new Vector2(
                -Input.GetAxis("Mouse Y"),
                Input.GetAxis("Mouse X")
            );

            rotationAngles.x += rotationSpeed * Time.unscaledDeltaTime * mouseInput.x;
            rotationAngles.y += rotationSpeed * Time.unscaledDeltaTime * mouseInput.y;

            transform.rotation = Quaternion.Euler(rotationAngles.x, rotationAngles.y, 0f);
        }

        private void ConstrainAngles()
        {
            rotationAngles.x = Mathf.Clamp(rotationAngles.x, minVerticalAngle, maxVerticalAngle);
            rotationAngles.y %= 360;
        }
    }
}
