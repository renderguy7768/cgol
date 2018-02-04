using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts
{
    public class GridGenerator : MonoBehaviour
    {
        [Range(3, 150)]
        [Tooltip("Grid Width (Number of Columns)")]
        public int Width;

        [Range(3, 150)]
        [Tooltip("Grid Height (Number of Rows)")]
        public int Height;

        [Range(0.1f, 1.0f)]
        [Tooltip("Delay in seconds between display of two generations")]
        public float GenerationGap;

        private static readonly Vector3 CellScale = Vector3.one * 0.8f;

        private Cell[,] _cells;
        private int _xOffset;
        private int _yOffset;

        private Coroutine _runCoroutine;

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
                Manager.GameState = GameStateEnum.Wait;
            }
        }

        private void Start()
        {
            // Calculate cell offsets
            if (Manager.GameState == GameStateEnum.Wait)
            {
                _cells = new Cell[Height, Width];
                _xOffset = Width - Mathf.FloorToInt(0.5f * (Width - 1) + 1.0f);
                _yOffset = Height - Mathf.FloorToInt(0.5f * (Height - 1) + 1.0f);
                PopulateGrid();
                CameraController.SetupCamera.Invoke(Width, Height);
            }
        }

        private void PopulateGrid()
        {
            for (var row = 0; row < Height; row++)
            {
                for (var column = 0; column < Width; column++)
                {
                    _cells[row, column] = Instantiate(Manager.CellPrefab, transform).GetComponent<Cell>();

#if UNITY_EDITOR
                    _cells[row, column].gameObject.name = "Cell (" + row + "," + column + ")";
#endif

                    var cellTransform = _cells[row, column].transform;
                    cellTransform.position = new Vector3(column - _xOffset, row - _yOffset, 0.0f);
                    cellTransform.rotation = Quaternion.identity;
                    cellTransform.localScale = CellScale;

                    _cells[row, column].Initialize(row, column, Width, Height);
                }
            }

            Manager.GameState = GameStateEnum.AcceptInput;
        }

        private void UpdateCells()
        {
            for (var row = 0; row < Height; row++)
            {
                for (var column = 0; column < Width; column++)
                {
                    var sum = CalculateCellSum(_cells[row, column]);
                    switch (sum)
                    {
                        case 3:
                            _cells[row, column].NextCellState = _cells[row, column].IsAlive
                                ? NextCellStateEnum.NoChange
                                : NextCellStateEnum.MakeAlive;
                            break;
                        case 4:
                            _cells[row, column].NextCellState = NextCellStateEnum.NoChange;
                            break;
                        default:
                            _cells[row, column].NextCellState = _cells[row, column].IsAlive
                                ? NextCellStateEnum.MakeDead
                                : NextCellStateEnum.NoChange;
                            break;
                    }
                }
            }
        }

        private void ApplyCellUpdates()
        {
            for (var row = 0; row < Height; row++)
            {
                for (var column = 0; column < Width; column++)
                {
                    if (_cells[row, column].NextCellState == NextCellStateEnum.MakeDead)
                    {
                        _cells[row, column].IsAlive = false;
                    }
                    else if (_cells[row, column].NextCellState == NextCellStateEnum.MakeAlive)
                    {
                        _cells[row, column].IsAlive = true;
                    }
                }
            }
        }

        private int CalculateCellSum(Cell cell)
        {
            return cell.CellState + cell.MyNeighbors.Sum(neighbor => _cells[neighbor.R, neighbor.C].CellState);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (Manager.GameState == GameStateEnum.AcceptInput)
                {
                    Manager.GameState = GameStateEnum.Run;
                    _runCoroutine = StartCoroutine(Run());
                }
                else if (Manager.GameState == GameStateEnum.Run)
                {
                    Manager.GameState = GameStateEnum.AcceptInput;
                    if (_runCoroutine != null)
                    {
                        StopCoroutine(_runCoroutine);
                    }
                }
            }
        }
        private IEnumerator Run()
        {
            while (Manager.GameState == GameStateEnum.Run)
            {
                UpdateCells();
                ApplyCellUpdates();
                yield return new WaitForSeconds(GenerationGap);
            }
        }
    }
}
