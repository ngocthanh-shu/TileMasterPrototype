using Tiles;
using UnityEngine;

namespace Managers
{
    public class TouchableManager : Singleton<TouchableManager>
    {
        private Vector2 _startPositionTouch;
        private Vector2 _currentPositionTouch;
        
        private GameManager _gameManager;
        private Camera _mainCamera;
        
        private TileController _selectedTile;
        
        
        
        private TouchState touchState;
    
        public void Initialize(GameManager gameManager)
        {
            _gameManager = gameManager;
            touchState = TouchState.None;
            _mainCamera = Camera.main;
        }

        public void OnUpdate()
        {
            if (touchState == TouchState.Start)
            {
                _currentPositionTouch = _gameManager.inputManager.GetTouchPosition();
                if (_startPositionTouch != _currentPositionTouch)
                {
                    GetObjectTouched(_currentPositionTouch);
                }
            }
        }
    
        public void StartTouch(Vector2 position, float time)
        {
            _startPositionTouch = position;
            _currentPositionTouch = _startPositionTouch;
            touchState = TouchState.Start;
            GetObjectTouched(position);
        }
        
        public void EndTouch(Vector2 position, float time)
        {
            touchState = TouchState.End;
            GetObjectTouched(position);
        }

        private void GetObjectTouched(Vector2 position)
        {
            if(_gameManager.GetGameState() != GameState.Game) return;
            Ray ray = _mainCamera.ScreenPointToRay(position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.collider.gameObject;
                TileView tileView = hitObject.GetComponent<TileView>();
                if (tileView != null)
                {
                    if (touchState == TouchState.End)
                    {
                        TileController tileController = tileView.OnTileClicked();
                        if (_selectedTile != null)
                        {
                            _selectedTile.SetOutline(false);
                            _selectedTile = null;
                        }
                        tileController.SetOutline(false);
                        _gameManager.SelectedTileAction?.Invoke(tileController);
                    }
                        
                    else
                    {
                        if (_selectedTile != null)
                        {
                            _selectedTile.SetOutline(false);
                        }
                        _selectedTile = tileView.OnTileClicked();
                        _selectedTile.SetOutline(true);
                    }
                }
            }
        }
        
        public TouchState GetTouchState()
        {
            return touchState;
        }
        
    }
    
    public enum TouchState
    {
        Start,
        End,
        None
    }
}
