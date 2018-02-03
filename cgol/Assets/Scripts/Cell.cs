using System;
using UnityEngine;

/**
 * Make index internal and remove serialization
 */

namespace Assets.Scripts
{
    public class Cell : MonoBehaviour
    {
        private int _cellState;
        private Renderer _renderer;
        private bool _isAlive;
        [SerializeField]
        private Index _me;
        [SerializeField]
        private Index[] _myNeighbors;

        public bool IsAlive
        {
            get { return _isAlive; }
            private set
            {
                _isAlive = value;
                _cellState = _isAlive ? 1 : 0;
                _renderer.sharedMaterial = Manager.CellMaterials[_cellState];
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
            IsAlive = false;
            _me.R = row;
            _me.C = column;
            _myNeighbors = new Index[8];

            _myNeighbors[0].R = row;
            _myNeighbors[0].C = columnMinusOne < 0 ? gridWidthMinusOne : columnMinusOne;

            _myNeighbors[1].R = rowPlusOne > gridHeightMinusOne ? 0 : rowPlusOne;
            _myNeighbors[1].C = columnMinusOne < 0 ? gridWidthMinusOne : columnMinusOne;

            _myNeighbors[2].R = rowPlusOne > gridHeightMinusOne ? 0 : rowPlusOne;
            _myNeighbors[2].C = column;

            _myNeighbors[3].R = rowPlusOne > gridHeightMinusOne ? 0 : rowPlusOne;
            _myNeighbors[3].C = columnPlusOne > gridWidthMinusOne ? 0 : columnPlusOne;

            _myNeighbors[4].R = row;
            _myNeighbors[4].C = columnPlusOne > gridWidthMinusOne ? 0 : columnPlusOne;

            _myNeighbors[5].R = rowMinusOne < 0 ? gridHeightMinusOne : rowMinusOne;
            _myNeighbors[5].C = columnPlusOne > gridWidthMinusOne ? 0 : columnPlusOne;

            _myNeighbors[6].R = rowMinusOne < 0 ? gridHeightMinusOne : rowMinusOne;
            _myNeighbors[6].C = column;

            _myNeighbors[7].R = rowMinusOne < 0 ? gridHeightMinusOne : rowMinusOne;
            _myNeighbors[7].C = columnMinusOne < 0 ? gridWidthMinusOne : columnMinusOne;
        }


        private void OnMouseDown()
        {
            if (Manager.GameState == GameStateEnum.AcceptInput)
            {
                IsAlive = !IsAlive;
            }
        }
    }
    [Serializable]
    public struct Index
    {
        public int R;
        public int C;
    }

}
