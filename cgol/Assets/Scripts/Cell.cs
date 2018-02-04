using System.Linq;
using UnityEngine;

/**
 * Make neighbors property
 * Change access modifier of marked fields and maybe make them props
 * Make things private
 */

namespace Assets.Scripts
{
    public enum NextCellStateEnum : byte { NoChange, MakeDead, MakeAlive }
    public class Cell : MonoBehaviour
    {
        private const int NumberOfNeighbors = 8;
        private Renderer _renderer;
        public int CellState; //{ get; private set; } // Make private

        //////////////////////////////////////////////////

        public Index[] MyNeighbors; //{ get; private set; } // Make private
        public Index Me;
        public Index FrontOfMe;
        public Index BackOfMe;

        public bool IsSumSet; //{ get; set; }

        private int _sum;
        public int Sum
        {
            get
            {
                if (IsSumSet)
                {
                    return _sum;
                }

                Debug.LogErrorFormat("Trying to get a not set sum. Index: {0}", Me);
                Manager.GameState = GameStateEnum.Invalid;
                return -1;
            }
            private set
            {
                IsSumSet = true;
                _sum = value;
            }
        }
        //////////////////////////////////////////////////

        public NextCellStateEnum NextCellState { get; set; }
        private bool _isAlive;
        public bool IsAlive
        {
            get { return _isAlive; }
            set
            {
                _isAlive = value;
                CellState = _isAlive ? 1 : 0;
                _renderer.sharedMaterial = Manager.CellMaterials[CellState];
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
            MyNeighbors = new Index[NumberOfNeighbors];
            Me = new Index { W = w, H = h, D = d };

            if (is3D)
            {
                FrontOfMe = new Index { W = w, H = h, D = dMinusOne < 0 ? gridDepthMinusOne : dMinusOne };
                BackOfMe = new Index { W = w, H = h, D = dPlusOne > gridDepthMinusOne ? 0 : dPlusOne };
            }

            MyNeighbors[0] = new Index
            {
                H = h,
                W = wMinusOne < 0 ? gridWidthMinusOne : wMinusOne,
                D = is3D ? d : 0
            };

            MyNeighbors[1] = new Index
            {
                H = hPlusOne > gridHeightMinusOne ? 0 : hPlusOne,
                W = wMinusOne < 0 ? gridWidthMinusOne : wMinusOne,
                D = is3D ? d : 0
            };

            MyNeighbors[2] = new Index
            {
                H = hPlusOne > gridHeightMinusOne ? 0 : hPlusOne,
                W = w,
                D = is3D ? d : 0
            };

            MyNeighbors[3] = new Index
            {
                H = hPlusOne > gridHeightMinusOne ? 0 : hPlusOne,
                W = wPlusOne > gridWidthMinusOne ? 0 : wPlusOne,
                D = is3D ? d : 0
            };

            MyNeighbors[4] = new Index
            {
                H = h,
                W = wPlusOne > gridWidthMinusOne ? 0 : wPlusOne,
                D = is3D ? d : 0
            };

            MyNeighbors[5] = new Index
            {
                H = hMinusOne < 0 ? gridHeightMinusOne : hMinusOne,
                W = wPlusOne > gridWidthMinusOne ? 0 : wPlusOne,
                D = is3D ? d : 0
            };

            MyNeighbors[6] = new Index
            {
                H = hMinusOne < 0 ? gridHeightMinusOne : hMinusOne,
                W = w,
                D = is3D ? d : 0
            };

            MyNeighbors[7] = new Index
            {
                H = hMinusOne < 0 ? gridHeightMinusOne : hMinusOne,
                W = wMinusOne < 0 ? gridWidthMinusOne : wMinusOne,
                D = is3D ? d : 0
            };

        }

        public void CalculateCellSum(Cell[,,] cells)
        {
            Sum = CellState + MyNeighbors.Sum(neighbor => cells[neighbor.D, neighbor.H, neighbor.W].CellState);
        }

        private void OnMouseDown()
        {
            if (Manager.GameState == GameStateEnum.AcceptInput)
            {
                IsAlive = !IsAlive;
            }
        }
    }

    [System.Serializable]
    public struct Index
    {
        public int W;
        public int H;
        public int D;
        public override string ToString()
        {
            return "Cell (" + D + "," + H + "," + W + ")";
        }
    }
}
