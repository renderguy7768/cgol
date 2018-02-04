using System;
using UnityEngine;
using Random = UnityEngine.Random;

/**
 * Make index internal and remove serialization
 */

namespace Assets.Scripts
{
    public enum NextCellStateEnum { NoChange, MakeDead, MakeAlive }
    public class Cell : MonoBehaviour
    {
        public int CellState { get; private set; }
        private Renderer _renderer;
        //private Index _me;
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
            IsAlive = Random.Range(0, int.MaxValue) % 2 == 0;
            NextCellState = NextCellStateEnum.NoChange;
            //_me.R = row;
            //_me.C = column;
            MyNeighbors = new Index[8];

            MyNeighbors[0].R = row;
            MyNeighbors[0].C = columnMinusOne < 0 ? gridWidthMinusOne : columnMinusOne;

            MyNeighbors[1].R = rowPlusOne > gridHeightMinusOne ? 0 : rowPlusOne;
            MyNeighbors[1].C = columnMinusOne < 0 ? gridWidthMinusOne : columnMinusOne;

            MyNeighbors[2].R = rowPlusOne > gridHeightMinusOne ? 0 : rowPlusOne;
            MyNeighbors[2].C = column;

            MyNeighbors[3].R = rowPlusOne > gridHeightMinusOne ? 0 : rowPlusOne;
            MyNeighbors[3].C = columnPlusOne > gridWidthMinusOne ? 0 : columnPlusOne;

            MyNeighbors[4].R = row;
            MyNeighbors[4].C = columnPlusOne > gridWidthMinusOne ? 0 : columnPlusOne;

            MyNeighbors[5].R = rowMinusOne < 0 ? gridHeightMinusOne : rowMinusOne;
            MyNeighbors[5].C = columnPlusOne > gridWidthMinusOne ? 0 : columnPlusOne;

            MyNeighbors[6].R = rowMinusOne < 0 ? gridHeightMinusOne : rowMinusOne;
            MyNeighbors[6].C = column;

            MyNeighbors[7].R = rowMinusOne < 0 ? gridHeightMinusOne : rowMinusOne;
            MyNeighbors[7].C = columnMinusOne < 0 ? gridWidthMinusOne : columnMinusOne;
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
        internal int R;
        internal int C;
    }

}
