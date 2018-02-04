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

        [Range(3, 150)]
        [Tooltip("Grid Depth")]
        public int Depth;

        [Range(0.1f, 1.0f)]
        [Tooltip("Delay in seconds between display of two generations")]
        public float GenerationGap;

        [Tooltip("Toggle to set 3d on and off. If off depth will be one even if it shows 3")]
        public bool Is3D;

        [Range(1, 10)]
        [Tooltip("Multiplier for distance between all cells in 3D mode")]
        public int DistanceMultiplier;

        private static readonly Vector3 CellScale = Vector3.one * 0.8f;

        private Cell[,,] _cells;
        private Vector3Int _offset;

        private Coroutine _runCoroutine;

        private void Awake()
        {
            Assert.IsTrue(Width > 2, "Width should be greater than 2 for proper simulation to occur");
            Assert.IsTrue(Height > 2, "Height should be greater than 2 for proper simulation to occur");
            Assert.IsTrue(Depth > 2, "Depth should be greater than 2 for proper simulation to occur");

            if (Width < 3 || Height < 3 || Depth < 3)
            {
                Manager.GameState = GameStateEnum.Invalid;
                Debug.LogError("Invalid width or height or depth");
                return;
            }

            if (!Is3D)
            {
                Depth = 1;
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
                _cells = new Cell[Depth, Height, Width];
                _offset.x = Width - Mathf.FloorToInt(0.5f * (Width - 1) + 1.0f);
                _offset.y = Height - Mathf.FloorToInt(0.5f * (Height - 1) + 1.0f);
                _offset.z = Depth - Mathf.FloorToInt(0.5f * (Depth - 1) + 1.0f);
                PopulateGrid();
                CameraController.SetupCamera.Invoke(Width, Height, Depth, DistanceMultiplier);
            }
        }

        private void PopulateGrid()
        {
            for (var d = 0; d < Depth; d++)
            {
                for (var h = 0; h < Height; h++)
                {
                    for (var w = 0; w < Width; w++)
                    {

                        _cells[d, h, w] = Instantiate(Manager.CellPrefab, transform).GetComponent<Cell>();

#if UNITY_EDITOR
                        _cells[d, h, w].gameObject.name = "Cell (" + h + "," + w + "," + d + ")";
#endif

                        var cellTransform = _cells[d, h, w].transform;
                        cellTransform.position = new Vector3(w - _offset.x, h - _offset.y, d - _offset.z) * (Is3D
                            ? DistanceMultiplier
                            : 1.0f);
                        cellTransform.rotation = Quaternion.identity;
                        cellTransform.localScale = CellScale;

                        _cells[d, h, w].Initialize(h, w, Width, Height);
                    }
                }
            }

            Manager.GameState = GameStateEnum.AcceptInput;
        }

        private void UpdateCells()
        {
            for (var d = 0; d < Depth; d++)
            {
                for (var h = 0; h < Height; h++)
                {
                    for (var w = 0; w < Width; w++)
                    {
                        var sum = CalculateCellSum(_cells[d, h, w]);
                        switch (sum)
                        {
                            case 3:
                                _cells[d, h, w].NextCellState = _cells[d, h, w].IsAlive
                                    ? NextCellStateEnum.NoChange
                                    : NextCellStateEnum.MakeAlive;
                                break;
                            case 4:
                                _cells[d, h, w].NextCellState = NextCellStateEnum.NoChange;
                                break;
                            default:
                                _cells[d, h, w].NextCellState = _cells[d, h, w].IsAlive
                                    ? NextCellStateEnum.MakeDead
                                    : NextCellStateEnum.NoChange;
                                break;
                        }
                    }
                }
            }
        }

        private void ApplyCellUpdates()
        {
            for (var d = 0; d < Depth; d++)
            {
                for (var h = 0; h < Height; h++)
                {
                    for (var w = 0; w < Width; w++)
                    {
                        if (_cells[d, h, w].NextCellState == NextCellStateEnum.MakeDead)
                        {
                            _cells[d, h, w].IsAlive = false;
                        }
                        else if (_cells[d, h, w].NextCellState == NextCellStateEnum.MakeAlive)
                        {
                            _cells[d, h, w].IsAlive = true;
                        }
                    }
                }
            }
        }

        private int CalculateCellSum(Cell cell)
        {
            return cell.CellState + cell.MyNeighbors.Sum(neighbor => _cells[neighbor.D, neighbor.H, neighbor.W].CellState);
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
