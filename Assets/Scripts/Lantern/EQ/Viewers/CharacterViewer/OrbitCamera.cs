using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lantern.Legacy.CharacterViewer
{
    public class OrbitCamera : MonoBehaviour
    {
        public Vector3 _targetPosition;
        public float CurrentDistance = 5.0f;
        public float xSpeed = 5000.0f;
        public float ySpeed = 5000.0f;

        [SerializeField]
        private RectTransform _uiElement;

        [SerializeField]
        private float yMinLimit = -30f;

        [SerializeField]
        private float yMaxLimit = 90f;

        [SerializeField]
        private float _distanceMin = 0.25f;

        [SerializeField]
        private float _distanceMax = 5f;
        public float maxSpeed = 20f;

        private float _zoomSpeed;

        private float _defaultDistance = 10f;


        private Rigidbody rigidbody;

        private Vector2 _inputVelocity = new Vector2();

        private Vector2 _rotation;

        [SerializeField] private float damping;

        private Vector2 _defaultRotation;

        public bool _inputActive = true;

        private float _previousPinchDistance = -1.0f;

        private bool _rotateTouchAvailable = true;

        private void Start()
        {
            Vector3 angles = transform.eulerAngles;
            _rotation = new Vector2(angles.y, angles.x);
            _defaultRotation = _rotation;
            float ratio = _uiElement.sizeDelta.x * FindObjectOfType<Canvas>().scaleFactor / Screen.width;
            var rect = GetComponent<Camera>().rect;
            rect.x = ratio;
            GetComponent<Camera>().rect = rect;
        }

        public void SetNewTarget(float defaultDistance, float followOffset)
        {
            _targetPosition = new Vector3(0f, followOffset, 0f);
            CurrentDistance = defaultDistance;
            _defaultDistance = defaultDistance;
            _zoomSpeed = defaultDistance / 10.0f;
        }

        public void ResetCamera()
        {
            CurrentDistance = _defaultDistance;
            _inputVelocity = Vector2.zero;
            _rotation = _defaultRotation;
        }

        void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetCamera();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                var speed = Input.GetKey(KeyCode.LeftShift) ? 1.0f : 0.1f;
                var targetPos = _targetPosition;
                targetPos.y += speed;
                _targetPosition = targetPos;
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                var speed = Input.GetKey(KeyCode.LeftShift) ? 1.0f : 0.1f;
                var targetPos = _targetPosition;
                targetPos.y -= speed;
                _targetPosition = targetPos;
            }

            if (_inputActive && Input.touchCount == 2 && !IsPointerOverUiObject())
            {
                float pinchDist = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);

                if (_previousPinchDistance != -1.0f)
                {
                    float newDistance = CurrentDistance - (pinchDist - _previousPinchDistance) * _zoomSpeed * Time.deltaTime;
                    CurrentDistance =
                        Mathf.Clamp(newDistance, _distanceMin * _defaultDistance, _distanceMax * _defaultDistance);
                }

                _previousPinchDistance = pinchDist;
            }
            else
            {
                if (_previousPinchDistance != -1f)
                {
                    StartCoroutine(SetPostPinchTimeout());
                }

                _previousPinchDistance = -1f;
            }

            if (_inputActive && Input.touchCount == 1 && !IsTouchOverUiObject() && _rotateTouchAvailable)
            {
                var touch = Input.GetTouch(0);
                _inputVelocity.x = touch.deltaPosition.x * 50f * Time.deltaTime;
                _inputVelocity.y = -touch.deltaPosition.y * 50f * Time.deltaTime;
            }

            /*if (Input.GetMouseButton(0) && !IsPointerOverUiObject())
            {
                _inputVeloity.x = Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
                _inputVeloity.y = -Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;
            }*/

            _rotation.x += _inputVelocity.x;
            _rotation.y += _inputVelocity.y;

            _rotation.y = Mathf.Clamp(_rotation.y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(_rotation.y, _rotation.x, 0);

            if (!IsPointerOverUiObject() && _inputActive)
            {
                float newDistance = CurrentDistance - Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed;
                CurrentDistance =
                    Mathf.Clamp(newDistance, _distanceMin * _defaultDistance, _distanceMax * _defaultDistance);
            }

            PositionCamera(rotation, CurrentDistance);

            // Damping
            _inputVelocity.x = Mathf.MoveTowards(_inputVelocity.x, 0.0f, damping * Time.deltaTime);
            _inputVelocity.y = Mathf.MoveTowards(_inputVelocity.y, 0.0f, damping * Time.deltaTime);

            // Velocity limit
            _inputVelocity.x = Mathf.Clamp(_inputVelocity.x, -maxSpeed, maxSpeed);
            _inputVelocity.y = Mathf.Clamp(_inputVelocity.y, -maxSpeed, maxSpeed);
        }

        private IEnumerator SetPostPinchTimeout()
        {
            _rotateTouchAvailable = false;
            yield return new WaitForSeconds(0.1f);
            _rotateTouchAvailable = true;
        }

        private void PositionCamera(Quaternion rotation, float distance)
        {
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -CurrentDistance);
            Vector3 position = rotation * negDistance + _targetPosition;

            transform.rotation = rotation;
            transform.position = position;
        }

        private static bool IsPointerOverUiObject()
        {
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        private static bool IsTouchOverUiObject()
        {
            if (Input.touchCount == 0)
            {
                return false;
            }

            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            for (int i = 0; i < Input.touchCount; ++i)
            {
                var touch = Input.GetTouch(i);

                eventDataCurrentPosition.position = new Vector2(touch.position.x, touch.position.y);
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

                if (results.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
