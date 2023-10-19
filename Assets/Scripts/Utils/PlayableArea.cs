using Managers;
using UnityEngine;

namespace Utils
{
    public class PlayableArea : MonoBehaviour
    {
        private GameManager _gameManager;
    
        public void Initialize(GameManager gameManager)
        {
            _gameManager = gameManager;
        }
    
        public Collider GetPlayableAreaCollider()
        {
            return GetComponent<Collider>();
        }

        private void OnTriggerExit(Collider other)
        {
            _gameManager.RefreshTilePositionAction?.Invoke(other.gameObject);
        }
    }
}
