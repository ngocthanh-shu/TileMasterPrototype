using Managers;
using Unity.VisualScripting;
using UnityEngine;

namespace Tiles
{
    public class TileController : IGameController
    {
        private TileView _view;
        private Outline _outline;
        private GameManager _gameManager;

        public void Initialize(IData data = null)
        {
            if(data == null) return;
            TileData tileData = (TileData) data;
            _view = tileData.View;
            _view.Initialize();
            _outline = _view.GameObject().GetComponent<Outline>();
            _outline.enabled = false;
            _gameManager = tileData.GameManager;
            
            InitializeAction();
        }
        
        private void InitializeAction()
        {
            _view.OnTileClicked += OnTileClicked;
        }

        private TileController OnTileClicked()
        {
            return this;
        }


        public TileView GetView()
        {
            return _view;
        }
        
        public void SetOutline(bool value)
        {
            _outline.enabled = value;
        }
        
        public void DisableColliderTrigger()
        {
            Collider tileCollider = GetView().GetCollider();
            Rigidbody tileRigidbody = GetView().GetRigidbody();
            if(tileCollider != null)
                tileCollider.isTrigger = true;
            if (tileRigidbody != null)
            {
                tileRigidbody.useGravity = false;
                RigidbodyConstraints constraints;
                constraints = RigidbodyConstraints.FreezePosition;
                constraints = RigidbodyConstraints.FreezeRotation;
                tileRigidbody.constraints = constraints;
            }
        }
        
        public void EnableColliderTrigger()
        {
            Collider tileCollider = GetView().GetCollider();
            Rigidbody tileRigidbody = GetView().GetRigidbody();
            if(tileCollider != null)
                tileCollider.isTrigger = false;
            if (tileRigidbody != null)
            {
                tileRigidbody.useGravity = true;
                RigidbodyConstraints constraints;
                constraints = RigidbodyConstraints.None;
                constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                tileRigidbody.constraints = constraints;
            }
        }

    }
    
    public class TileData : IData
    {
        public TileView View;
        public GameManager GameManager;
    }
}
