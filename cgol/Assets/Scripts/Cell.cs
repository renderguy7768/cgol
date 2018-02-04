using UnityEngine;

namespace Assets.Scripts
{
    public enum NextCellStateEnum { NoChange, MakeDead, MakeAlive }
    public class Cell : MonoBehaviour
    {
        private Renderer _renderer;
        public int CellState { get; private set; }   
        [SerializeField]
        internal Index[] MyNeighbors { get; private set; }
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

        public void Initialize(int row, int column, int gridWidth, int gridHeight)
        {
            var gridWidthMinusOne = gridWidth - 1;
            var gridHeightMinusOne = gridHeight - 1;

            var rowPlusOne = row + 1;
            var rowMinusOne = row - 1;
            var columnPlusOne = column + 1;
            var columnMinusOne = column - 1;

            _renderer = GetComponent<Renderer>();
            IsAlive = (Random.Range(0, int.MaxValue) & 0x1) == 0;
            NextCellState = NextCellStateEnum.NoChange;
            MyNeighbors = new Index[8];

            MyNeighbors[0].H = row;
            MyNeighbors[0].W = columnMinusOne < 0 ? gridWidthMinusOne : columnMinusOne;

            MyNeighbors[1].H = rowPlusOne > gridHeightMinusOne ? 0 : rowPlusOne;
            MyNeighbors[1].W = columnMinusOne < 0 ? gridWidthMinusOne : columnMinusOne;

            MyNeighbors[2].H = rowPlusOne > gridHeightMinusOne ? 0 : rowPlusOne;
            MyNeighbors[2].W = column;

            MyNeighbors[3].H = rowPlusOne > gridHeightMinusOne ? 0 : rowPlusOne;
            MyNeighbors[3].W = columnPlusOne > gridWidthMinusOne ? 0 : columnPlusOne;

            MyNeighbors[4].H = row;
            MyNeighbors[4].W = columnPlusOne > gridWidthMinusOne ? 0 : columnPlusOne;

            MyNeighbors[5].H = rowMinusOne < 0 ? gridHeightMinusOne : rowMinusOne;
            MyNeighbors[5].W = columnPlusOne > gridWidthMinusOne ? 0 : columnPlusOne;

            MyNeighbors[6].H = rowMinusOne < 0 ? gridHeightMinusOne : rowMinusOne;
            MyNeighbors[6].W = column;

            MyNeighbors[7].H = rowMinusOne < 0 ? gridHeightMinusOne : rowMinusOne;
            MyNeighbors[7].W = columnMinusOne < 0 ? gridWidthMinusOne : columnMinusOne;
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
    internal struct Index
    {
        internal int W;
        internal int H;
        internal int D;
    }

}
