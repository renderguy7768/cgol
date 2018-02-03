using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts
{
    public class GridGenerator : MonoBehaviour
    {
        [Tooltip("Grid Width (Number of Columns)")]
        public int Width;

        [Tooltip("Grid Height (Number of Rows)")]
        public int Height;

        private const byte NumberOfCellMaterials = 2;
        private static readonly Vector3 CellScale = Vector3.one * 0.8f;

        private GameObject _cellPrefab;
        private Material[] _cellMaterials;
        private Transform[,] _cells;
        private int _xOffset;
        private int _yOffset;

        private enum GameState : byte { Invalid, Wait, Run }

        private GameState _gameState;

        private void Awake()
        {
            Assert.IsTrue(Width > 2, "Width should be greater than 2 for proper simulation to occur");
            Assert.IsTrue(Height > 2, "Height should be greater than 2 for proper simulation to occur");

            if (Width < 3 || Height < 3)
            {
                _gameState = GameState.Invalid;
                Debug.LogError("Invalid width or height");
                return;
            }

            _cellPrefab = Resources.Load<GameObject>("Prefabs/Cell");
            Assert.IsNotNull(_cellPrefab, "Cell prefab not found");
            if (_cellPrefab == null)
            {
                _gameState = GameState.Invalid;
                Debug.LogError("Cell prefab not found");
                return;
            }
            _cellMaterials = new Material[NumberOfCellMaterials];
            _cells = new Transform[Height, Width];
            _gameState = GameState.Wait;
        }

        private void Start()
        {
            // Calculate cell offsets
            if (_gameState == GameState.Wait)
            {
                _xOffset = Width - Mathf.FloorToInt(0.5f * (Width - 1) + 1.0f);
                _yOffset = Height - Mathf.FloorToInt(0.5f * (Height - 1) + 1.0f);
                StartCoroutine(PopulateGrid());
                CameraController.SetupCamera.Invoke(Width, Height);
            }
        }

        private IEnumerator PopulateGrid()
        {
            for (var row = 0; row < Height; row++)
            {
                for (var column = 0; column < Width; column++)
                {
                    _cells[row, column] = Instantiate(_cellPrefab, transform).GetComponent<Transform>();

                    _cells[row, column].position = new Vector3(column - _xOffset, row - _yOffset, 0.0f);
                    _cells[row, column].rotation = Quaternion.identity;
                    _cells[row, column].localScale = CellScale;

                    yield return null;
                }
            }
        }
    }
}
