using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public enum NextCellStateEnum : byte { NoChange, MakeDead, MakeAlive }
    public class Cell : MonoBehaviour
    {
        private const int NumberOfNeighbors = 8;
        private Renderer _renderer;
        private int _cellState;

        private Index[] _myNeighbors;
        private Index _me;
        private Index _frontOfMe;
        private Index _backOfMe;

        public bool IsSumSet { get; set; }

        private int _sum;
        private int Sum
        {
            get
            {
                if (IsSumSet)
                {
                    return _sum;
                }

                Debug.LogErrorFormat("Trying to get a not set sum. Index: {0}", _me);
                Manager.GameState = GameStateEnum.Invalid;
                return -1;
            }
            set
            {
                IsSumSet = true;
                _sum = value;
            }
        }

        public NextCellStateEnum NextCellState { get; set; }

        private bool _isAlive;
        public bool IsAlive
        {
            get { return _isAlive; }
            set
            {
                _isAlive = value;
                _cellState = _isAlive ? 1 : 0;
                _renderer.sharedMaterial = Manager.CellMaterials[_cellState];
            }
        }

        public void Initialize(int d, int h, int w, int gridWidth, int gridHeight, int gridDepth)
        {
            var gridWidthMinusOne = gridWidth - 1;
            var gridHeightMinusOne = gridHeight - 1;
            var gridDepthMinusOne = gridDepth - 1;

            var wPlusOne = w + 1;
            var wMinusOne = w - 1;
            var hPlusOne = h + 1;
            var hMinusOne = h - 1;
            var dPlusOne = d + 1;
            var dMinusOne = d - 1;

            var is3D = gridDepth != 1;

            _renderer = GetComponent<Renderer>();
            IsAlive = (Random.Range(0, int.MaxValue) & 0x1) == 0;
            NextCellState = NextCellStateEnum.NoChange;
            _myNeighbors = new Index[NumberOfNeighbors];
            _me = new Index { W = w, H = h, D = d };

            if (is3D)
            {
                _frontOfMe = new Index { W = w, H = h, D = dMinusOne < 0 ? gridDepthMinusOne : dMinusOne };
                _backOfMe = new Index { W = w, H = h, D = dPlusOne > gridDepthMinusOne ? 0 : dPlusOne };
            }

            _myNeighbors[0] = new Index
            {
                H = h,
                W = wMinusOne < 0 ? gridWidthMinusOne : wMinusOne,
                D = is3D ? d : 0
            };

            _myNeighbors[1] = new Index
            {
                H = hPlusOne > gridHeightMinusOne ? 0 : hPlusOne,
                W = wMinusOne < 0 ? gridWidthMinusOne : wMinusOne,
                D = is3D ? d : 0
            };

            _myNeighbors[2] = new Index
            {
                H = hPlusOne > gridHeightMinusOne ? 0 : hPlusOne,
                W = w,
                D = is3D ? d : 0
            };

            _myNeighbors[3] = new Index
            {
                H = hPlusOne > gridHeightMinusOne ? 0 : hPlusOne,
                W = wPlusOne > gridWidthMinusOne ? 0 : wPlusOne,
                D = is3D ? d : 0
            };

            _myNeighbors[4] = new Index
            {
                H = h,
                W = wPlusOne > gridWidthMinusOne ? 0 : wPlusOne,
                D = is3D ? d : 0
            };

            _myNeighbors[5] = new Index
            {
                H = hMinusOne < 0 ? gridHeightMinusOne : hMinusOne,
                W = wPlusOne > gridWidthMinusOne ? 0 : wPlusOne,
                D = is3D ? d : 0
            };

            _myNeighbors[6] = new Index
            {
                H = hMinusOne < 0 ? gridHeightMinusOne : hMinusOne,
                W = w,
                D = is3D ? d : 0
            };

            _myNeighbors[7] = new Index
            {
                H = hMinusOne < 0 ? gridHeightMinusOne : hMinusOne,
                W = wMinusOne < 0 ? gridWidthMinusOne : wMinusOne,
                D = is3D ? d : 0
            };

        }

        public int CalculateCellSum(Cell[,,] cells)
        {
            if (!IsSumSet)
            {
                Sum = _cellState + _myNeighbors.Sum(neighbor => cells[neighbor.D, neighbor.H, neighbor.W]._cellState);
            }
            return Sum;
        }

        public int CalculateCellSum3D(Cell[,,] cells)
        {
            var result = CalculateCellSum(cells);

            var backCell = cells[_backOfMe.D, _backOfMe.H, _backOfMe.W];
            result += backCell.IsSumSet ? backCell.Sum : backCell.CalculateCellSum(cells);

            var frontCell = cells[_frontOfMe.D, _frontOfMe.H, _frontOfMe.W];
            result += frontCell.IsSumSet ? frontCell.Sum : frontCell.CalculateCellSum(cells);

            return result;
        }

        private void OnMouseDown()
        {
            if (Manager.GameState == GameStateEnum.AcceptInput)
            {
                IsAlive = !IsAlive;
            }
        }
    }

    internal struct Index
    {
        internal int W;
        internal int H;
        internal int D;
        public override string ToString()
        {
            return "Cell (" + D + "," + H + "," + W + ")";
        }
    }
}
