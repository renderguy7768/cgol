using System;
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
        private MinMax<float> _orthographicSize;
        private Bounds _bounds;

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
            _orthographicSize.Max = _camera.orthographicSize = aspectRatioMultiplier * 0.5f * Mathf.Max(gridWidth, gridHeight);
            _orthographicSize.Min = aspectRatioMultiplier - 0.5f;
            _bounds = new Bounds(_camera, _orthographicSize.Max);
            _bounds.Update(_camera);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                transform.position = _originalPostion;
                transform.rotation = Quaternion.identity;
                _camera.orthographicSize = _orthographicSize.Max;
                _bounds.Update(_camera);
            }

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
        }

        private void LateUpdate()
        {
            if (Mathf.Abs(_zoomInputDelta) > 0.0f)
            {
                var zoomDeltaTimeIndependent = _zoomInputDelta * Time.smoothDeltaTime * ZoomSpeed;
                _camera.orthographicSize += zoomDeltaTimeIndependent;
                _camera.orthographicSize =
                    Mathf.Clamp(_camera.orthographicSize, _orthographicSize.Min, _orthographicSize.Max);
                _bounds.Update(_camera);
                _zoomInputDelta = 0.0f;
            }

            var position = transform.position;
            position.x = Mathf.Clamp(position.x, _bounds.Left, _bounds.Right);
            position.y = Mathf.Clamp(position.y, _bounds.Bottom, _bounds.Top);
            transform.position = position;
        }
    }

    public class Bounds
    {
        private readonly float _hExtent;
        private readonly float _vExtent;
        private readonly float _maxOrthographicSize;

        private Vector3 _bottomLeft;
        private Vector3 _topRight;

        public float Left { get; private set; }
        public float Right { get; private set; }
        public float Top { get; private set; }
        public float Bottom { get; private set; }

        public Bounds(Camera camera, float max)
        {
            _vExtent = camera.orthographicSize;
            _hExtent = camera.aspect * _vExtent;

            _bottomLeft = camera.ViewportToWorldPoint(Vector3.zero);
            _topRight = camera.ViewportToWorldPoint(Vector3.one);

            _maxOrthographicSize = max;
        }
        public void Update(Camera camera)
        {
            var deltaOrthographicSize = _maxOrthographicSize - camera.orthographicSize;

            var vDeltaExtent = deltaOrthographicSize;
            var hDeltaExtent = camera.aspect * vDeltaExtent;

            Left = _bottomLeft.x + _hExtent - hDeltaExtent;
            Right = _topRight.x - _hExtent + hDeltaExtent;
            Bottom = _bottomLeft.y + _vExtent - vDeltaExtent;
            Top = _topRight.y - _vExtent + vDeltaExtent;
        }
    }

    public struct MinMax<T>
    {
        public T Min;
        public T Max;
    }
}
