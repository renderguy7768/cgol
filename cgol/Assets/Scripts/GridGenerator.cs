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

        private static readonly Vector3 CellScale = Vector3.one * 0.8f;

        private Cell[,] _cells;
        private int _xOffset;
        private int _yOffset;

        private void Awake()
        {
            Assert.IsTrue(Width > 2, "Width should be greater than 2 for proper simulation to occur");
            Assert.IsTrue(Height > 2, "Height should be greater than 2 for proper simulation to occur");

            if (Width < 3 || Height < 3)
            {
                Manager.GameState = GameStateEnum.Invalid;
                Debug.LogError("Invalid width or height");
                return;
            }

            if (Manager.Initialize())
            {
                _cells = new Cell[Height, Width];
                Manager.GameState = GameStateEnum.Wait;
            }
        }

        private void Start()
        {
            // Calculate cell offsets
            if (Manager.GameState == GameStateEnum.Wait)
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
                    _cells[row, column] = Instantiate(Manager.CellPrefab, transform).GetComponent<Cell>();
                    _cells[row, column].gameObject.name = "Cell (" + row + "," + column + ")";

                    var cellTransform = _cells[row, column].transform;
                    cellTransform.position = new Vector3(column - _xOffset, row - _yOffset, 0.0f);
                    cellTransform.rotation = Quaternion.identity;
                    cellTransform.localScale = CellScale;

                    _cells[row, column].Initialize();

                    yield return null;
                }
            }

            Manager.GameState = GameStateEnum.AcceptInput;
        }
    }
}
