using System;
using UnityEngine;

namespace Tiles
{
    public class TileView : MonoBehaviour, ITile
    {
        [SerializeField]
        private int tileID;
        
        private Collider _collider;
        private Rigidbody _rigidbody;
        
        public Func<TileController> OnTileClicked;

        public void Initialize()
        {
            _collider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
        }
        
        public Collider GetCollider()
        {
            return _collider;
        }
        
        public Rigidbody GetRigidbody()
        {
            return _rigidbody;
        }
    
        public int GetTileID()
        {
            return tileID;
        }
        
        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }
}
