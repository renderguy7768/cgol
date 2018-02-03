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

            _maxOrthographicSize = _camera.orthographicSize = 0.5f * Mathf.Max(gridWidth, gridHeight);
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.R)) return;
            transform.position = _originalPostion;
            transform.rotation = Quaternion.identity;
            _camera.orthographicSize = _maxOrthographicSize;
        }
    }
}
