using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        public class SetupCameraEvent : UnityEvent<int, int, int, float>
        {
        }

        public static SetupCameraEvent SetupCamera;
        private Coroutine _resetCameraCoroutine;
        private bool _isCameraResetting;
        private Camera _camera;
        private Vector3 _originalPostion;
        private MinMax<float> _orthographicSize;
        private MinMax<float> _resetCameraSpeed;
        private Bounds _bounds;
        private Vector3 _initialMousePosition;
        private Vector3 _targetRotation;

        private const float RotationSpeed = 50.0f;
        private const float MoveSpeed = 50.0f;
        private const float PanSpeed = 2.5f;
        private const float ZoomSpeed =
#if UNITY_EDITOR || UNITY_STANDALONE
            100.0f;
#elif UNITY_ANDROID || UNITY_IOS
        1.0f;
#endif
        private float _zoomInputDelta;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            if (SetupCamera == null)
            {
                SetupCamera = new SetupCameraEvent();
            }

            _resetCameraSpeed.Min = 0.01f;
            _resetCameraSpeed.Max = 5.0f;
        }

        private void OnEnable()
        {
            SetupCamera.AddListener(Setup);
        }

        private void OnDisable()
        {
            SetupCamera.RemoveListener(Setup);
        }

        private void Setup(int gridWidth, int gridHeight, int gridDepth, float distanceMultiplierFor3D)
        {
            var isWidthEven = (gridWidth & 0x1) == 0;
            var isHeightEven = (gridHeight & 0x1) == 0;
            if (gridDepth == 1)
            {
                _camera.orthographic = true;

                _originalPostion = transform.position =
                    new Vector3(isWidthEven ? -0.5f : 0.0f, isHeightEven ? -0.5f : 0.0f, -10.0f);

                var aspectRatioMultiplier = _camera.aspect >= 1.0f ? 1.0f : 1.0f / _camera.aspect;
                _orthographicSize.Max = _camera.orthographicSize =
                    aspectRatioMultiplier * 0.5f * Mathf.Max(gridWidth, gridHeight);
                _orthographicSize.Min = aspectRatioMultiplier - 0.5f;
                _bounds = new Bounds(_camera, _orthographicSize.Max);
                _bounds.Update(_camera);
            }
            else
            {
                _camera.orthographic = false;

                _originalPostion = transform.position =
                    new Vector3(isWidthEven ? -0.5f : 0.0f, isHeightEven ? -0.5f : 0.0f,
                        -distanceMultiplierFor3D * Mathf.Max(gridWidth, gridHeight));
            }
        }

        private void Update()
        {
            if (Manager.GameState != GameStateEnum.AcceptInput && Manager.GameState != GameStateEnum.Run) return;
            if (!_isCameraResetting && Input.GetKeyDown(KeyCode.R))
            {
                _isCameraResetting = true;
                _resetCameraCoroutine = StartCoroutine(ResetCamera());
            }

            if (_camera.orthographic)
            {
                DetermineZoomDelta();
            }
            else
            {
                if (!Input.GetMouseButton(0)) return;
                if (_isCameraResetting && _resetCameraCoroutine != null)
                {
                    StopCoroutine(_resetCameraCoroutine);
                    _isCameraResetting = false;
                }
                CalculateNewRotaion();
            }
        }

        private void LateUpdate()
        {
            if (Manager.GameState != GameStateEnum.AcceptInput && Manager.GameState != GameStateEnum.Run) return;
            if (_camera.orthographic)
            {
                ZoomCamera();
                PanCamera();
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    transform.eulerAngles =
                        Vector3.Lerp(transform.eulerAngles, _targetRotation, RotationSpeed * Time.smoothDeltaTime);
                }

                MoveCamera();
            }
        }

        private void CalculateNewRotaion()
        {
            _targetRotation = new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0.0f);

            _targetRotation += transform.eulerAngles;
        }

        private void MoveCamera()
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                if (_isCameraResetting && _resetCameraCoroutine != null)
                {
                    StopCoroutine(_resetCameraCoroutine);
                    _isCameraResetting = false;
                }
            }
            var time = Time.smoothDeltaTime * MoveSpeed;
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += transform.forward * time;
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.position -= transform.forward * time;
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.position -= transform.right * time;
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position += transform.right * time;
            }
        }

        private IEnumerator ResetCamera()
        {
            var time = 0.0f;
            yield return new WaitUntil(() =>
            {
                time += Time.smoothDeltaTime * Mathf.Lerp(_resetCameraSpeed.Min, _resetCameraSpeed.Max, Time.smoothDeltaTime);
                transform.position = Vector3.Lerp(transform.position, _originalPostion, time);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, time);

                if (_camera.orthographic)
                {
                    _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _orthographicSize.Max, time);
                }
                return time > 1.0f;
            });
            if (_camera.orthographic)
            {
                UpdateBoundsAfterReset();
            }
        }

        private void UpdateBoundsAfterReset()
        {
            _bounds.Update(_camera);
            transform.position = _bounds.ClampCamera(transform.position);
            _isCameraResetting = false;
        }

        private void DetermineZoomDelta()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            _zoomInputDelta = -Input.GetAxis("Mouse ScrollWheel");
#elif UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount != 2) return;
            var touchZero = Input.GetTouch(0);
            var touchOne = Input.GetTouch(1);

            var touchZeroPreviousPosition = touchZero.position - touchZero.deltaPosition;
            var touchOnePreviousPosition = touchOne.position - touchOne.deltaPosition;

            var previousTouchDeltaMagnitude = (touchZeroPreviousPosition - touchOnePreviousPosition).magnitude;
            var currentTouchDeltaMagnitude = (touchZero.position - touchOne.position).magnitude;

            _zoomInputDelta = previousTouchDeltaMagnitude - currentTouchDeltaMagnitude;
#endif
        }

        private void ZoomCamera()
        {
            if (!(Mathf.Abs(_zoomInputDelta) > 0.0f)) return;
            if (_isCameraResetting && _resetCameraCoroutine != null)
            {
                StopCoroutine(_resetCameraCoroutine);
                UpdateBoundsAfterReset();
            }
            _camera.orthographicSize += _zoomInputDelta * Time.smoothDeltaTime * ZoomSpeed;
            _camera.orthographicSize =
                Mathf.Clamp(_camera.orthographicSize, _orthographicSize.Min, _orthographicSize.Max);
            _bounds.Update(_camera);
            _zoomInputDelta = 0.0f;
            transform.position = _bounds.ClampCamera(transform.position);
        }

        private void PanCamera()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_isCameraResetting && _resetCameraCoroutine != null)
                {
                    StopCoroutine(_resetCameraCoroutine);
                    UpdateBoundsAfterReset();
                }
                _initialMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                var direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) -
                                Camera.main.ScreenToWorldPoint(_initialMousePosition);
                direction.z = 0.0f;

                var position = transform.position + direction;

                var newPosition = Vector3.Lerp(transform.position,
                    new Vector3(position.x, position.y, transform.position.z), Time.smoothDeltaTime * PanSpeed);
                transform.position = _bounds.ClampCamera(newPosition);
            }
        }
    }

    public class Bounds
    {
        private readonly float _hExtent;
        private readonly float _vExtent;
        private readonly float _maxOrthographicSize;

        private Vector3 _bottomLeft;
        private Vector3 _topRight;

        private float _left;
        private float _right;
        private float _top;
        private float _bottom;

        public Bounds(Camera camera, float max)
        {
            _vExtent = camera.orthographicSize;
            _hExtent = camera.aspect * _vExtent;

            _bottomLeft = camera.ViewportToWorldPoint(Vector2.zero);
            _topRight = camera.ViewportToWorldPoint(Vector2.one);

            _maxOrthographicSize = max;
        }
        public void Update(Camera camera)
        {
            var deltaOrthographicSize = _maxOrthographicSize - camera.orthographicSize;

            var vDeltaExtent = deltaOrthographicSize;
            var hDeltaExtent = camera.aspect * vDeltaExtent;

            _left = _bottomLeft.x + _hExtent - hDeltaExtent;
            _right = _topRight.x - _hExtent + hDeltaExtent;
            _bottom = _bottomLeft.y + _vExtent - vDeltaExtent;
            _top = _topRight.y - _vExtent + vDeltaExtent;
        }

        public Vector3 ClampCamera(Vector3 position)
        {
            position.x = Mathf.Clamp(position.x, _left, _right);
            position.y = Mathf.Clamp(position.y, _bottom, _top);
            return position;
        }
    }

    public struct MinMax<T>
    {
        public T Min;
        public T Max;
    }
}
