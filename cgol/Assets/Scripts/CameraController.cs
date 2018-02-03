using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        public class SetupCameraEvent : UnityEvent<int, int> { }

        public static SetupCameraEvent SetupCamera;
        private Camera _camera;
        private Vector3 _originalPostion;
        private float _maxOrthographicSize;
        private float _minOrthographicSize;

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
        }
        private void OnEnable()
        {
            SetupCamera.AddListener(Setup);
        }
        private void OnDisable()
        {
            SetupCamera.RemoveListener(Setup);
        }

        private void Setup(int gridWidth, int gridHeight)
        {
            var isWidthEven = (gridWidth & 0x1) == 0;
            var isHeightEven = (gridHeight & 0x1) == 0;

            _originalPostion = transform.position =
                new Vector3(isWidthEven ? -0.5f : 0.0f, isHeightEven ? -0.5f : 0.0f, -10.0f);

            var aspectRatioMultiplier = _camera.aspect >= 1.0f ? 1.0f : 1.0f / _camera.aspect;
            _maxOrthographicSize = _camera.orthographicSize = aspectRatioMultiplier * 0.5f * Mathf.Max(gridWidth, gridHeight);
            _minOrthographicSize = aspectRatioMultiplier - 0.5f;
        }

        private void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            _zoomInputDelta = -Input.GetAxis("Mouse ScrollWheel");
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount == 2)
        {
            var touchZero = Input.GetTouch(0);
            var touchOne = Input.GetTouch(1);

            var touchZeroPreviousPosition = touchZero.position - touchZero.deltaPosition;
            var touchOnePreviousPosition = touchOne.position - touchOne.deltaPosition;

            var previousTouchDeltaMagnitude = (touchZeroPreviousPosition - touchOnePreviousPosition).magnitude;
            var currentTouchDeltaMagnitude = (touchZero.position - touchOne.position).magnitude;

            _zoomInputDelta = previousTouchDeltaMagnitude - currentTouchDeltaMagnitude;
        }
#endif

            if (Input.GetKeyDown(KeyCode.R))
            {
                transform.position = _originalPostion;
                transform.rotation = Quaternion.identity;
                _camera.orthographicSize = _maxOrthographicSize;
            }
        }

        private void LateUpdate()
        {
            if (Mathf.Abs(_zoomInputDelta) > 0.0f)
            {
                var zoomDeltaTimeIndependent = _zoomInputDelta * Time.smoothDeltaTime * ZoomSpeed;
                _camera.orthographicSize += zoomDeltaTimeIndependent;
                _camera.orthographicSize =
                    Mathf.Clamp(_camera.orthographicSize, _minOrthographicSize, _maxOrthographicSize);
                _zoomInputDelta = 0.0f;
            }
        }
    }
}
