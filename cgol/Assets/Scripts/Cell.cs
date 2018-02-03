using UnityEngine;

namespace Assets.Scripts
{
    public class Cell : MonoBehaviour
    {
        private int _cellState;
        private Renderer _renderer;
        private bool _isAlive;

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

        public void Initialize()
        {
            _renderer = GetComponent<Renderer>();
            IsAlive = false;
           
        }
        private void OnMouseDown()
        {
            if (Manager.GameState == GameStateEnum.AcceptInput)
            {
                IsAlive = !IsAlive;
            }
        }
    }
}
