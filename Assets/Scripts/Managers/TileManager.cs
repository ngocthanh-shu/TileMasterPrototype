using System.Collections;
using System.Collections.Generic;
using Tiles;
using UnityEngine;
using Utils;

namespace Managers
{
    public class TileManager : Singleton<TileManager>
    {
        private GameManager _gameManager;
        private Collider _background;
        private Collider _playableArea;
        private Collider _stackArea;
        private Collider _tileDefaultCollider;
        private int _stackNumber;
        

        private List<TileController> _tiles;
        private List<Vector2> _tilePositions;
        private int _currentTileActive;

        private GameObject _tile;
    
        public void Initialize(GameManager gameManager, Collider background, PlayableArea playableArea, Collider stackArea, Collider tileDefaultCollider, int stackNumber)
        {
            _gameManager = gameManager;
            _background = background;
            _stackArea = stackArea;
            _tileDefaultCollider = tileDefaultCollider;
            _stackNumber = stackNumber;
            _playableArea = playableArea.GetPlayableAreaCollider();
            _tiles = new List<TileController>();
            _tilePositions = new List<Vector2>();
            SetupTilePositionIntoStack();
        }

        public void GenerateTiles(List<TileInformation> tiles)
        {
            _currentTileActive = 0;
            foreach (var tile in tiles)
            {
                int quantity = tile.quantitySet * 3;
                for (int i = 0; i < quantity; i++)
                {
                    _currentTileActive += GenerateTile(_gameManager.levelManager.GetTileView(tile.tile).GetGameObject());
                }
            }
        }

        private int GenerateTile(GameObject tilePrefab)
        {
            Vector3 randomPosition = GenerateRandomPosition(_playableArea);
            _tile = _gameManager.poolManager.GetPooledObject(tilePrefab, randomPosition, Quaternion.identity);
            _tile.transform.rotation = GenerateRandomRotation();
            var tC = new TileController();
            tC.Initialize(new TileData
            {
                View = _tile.GetComponent<TileView>(),
                GameManager = _gameManager
            });
            return 1;
        }

        private Vector3 GenerateRandomPosition(Collider pArea)
        {
            Bounds bounds = pArea.bounds;
            Vector3 randomPosition = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                _background.bounds.max.y + Random.Range(0.1f, 5f),
                Random.Range(bounds.min.z, bounds.max.z)
            );
            return randomPosition;
        }
    
        private Quaternion GenerateRandomRotation()
        {
            Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            return randomRotation;
        }

        public void RefreshTilePosition(GameObject tile)
        {
            if (!CheckTileExist(tile))
            {
                tile.transform.position = GenerateRandomPosition(_playableArea);
            }
        }

        private void SetTileRotation(GameObject tile, float rotationY) 
        {
            tile.transform.rotation = Quaternion.Euler(0, rotationY, 0);
        }

        public int GetCurrentTileActive()
        {
            return _currentTileActive;
        }
        
        public int GetTileCount()
        {
            return _tiles.Count;
        }

        public void ResetStack()
        {
            _tiles.Clear();
        }
        
        public void AddTileIntoWait(TileController tile)
        {
            _gameManager.audioManager.PlayClickTile();
            if (CheckTileToAddWait(tile))
            {
                for (int i = 0; i < _tiles.Count; i++)
                {
                    if(GetID(tile) < GetID(_tiles[i]))
                    {
                        InsertTile(tile, i);
                        _gameManager.OnGameStateChanged?.Invoke();
                        break;
                    }
                    else if(GetID(tile) == GetID(_tiles[i]))
                    {
                        if (_tiles.Count > i + 1)
                        {
                            if (GetID(tile) == GetID(_tiles[i + 1]))
                            {
                                InsertTile(tile, i + 2);
                                StartCoroutine(RemoveTileFromWait(tile, _tiles[i], _tiles[i + 1]));
                                _currentTileActive -= 3;
                                _gameManager.OnStarScoreChanged?.Invoke();
                            }
                            else
                            {
                                InsertTile(tile, i + 1);
                                _gameManager.OnGameStateChanged?.Invoke();
                            }
                            break;
                        }
                        else
                        {
                            AddTile(tile);
                            _gameManager.OnGameStateChanged?.Invoke();
                            break;
                        }
                    }
                    else
                    {
                        if (_tiles.Count == i + 1)
                        {
                            AddTile(tile);
                            _gameManager.OnGameStateChanged?.Invoke();
                            break;
                        }
                    }
                }
            }
        }
        
        private bool CheckTileExist(GameObject tile)
        {
            TileView tileView = tile.GetComponent<TileView>();
            if(tileView == null)
                return false;
            TileController tileController = tileView.OnTileClicked();
            if(tileController == null)
                return false;
            return _tiles.Contains(tileController);
        }

        private int GetID(TileController tile)
        {
            return tile.GetView().GetTileID();
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator RemoveTileFromWait(TileController tile1, TileController tile2, TileController tile3)
        {
            yield return new WaitForSeconds(0.2f);
            tile1.EnableColliderTrigger();
            tile2.EnableColliderTrigger();
            tile3.EnableColliderTrigger();
            GetGo(tile1).SetActive(false);
            GetGo(tile3).SetActive(false);
            GetGo(tile2).SetActive(false);
            RemoveTile(tile1);
            RemoveTile(tile3);
            RemoveTile(tile2);
            _gameManager.audioManager.PlayAudioOneShot(_gameManager.audioManager.GetAudioSource(), _gameManager.resourceManager.GetDictionaryAudio()["AddStar"]);
            _gameManager.OnGameStateChanged?.Invoke();
        }
        
        private GameObject GetGo(TileController tile)
        {
            return tile.GetView().GetGameObject();
        }

        private bool CheckTileToAddWait(TileController tile)
        {
            if(tile == null)
                return false;
            if (_tiles.Contains(tile))
                return false;
            if (_tiles.Count == 0)
            {
                AddTile(tile);
                return false;
            }
            return true;
        }
        
        private void AddTile(TileController tile)
        {
            _tiles.Add(tile);
            SetTileRotation(GetGo(tile), 90);
            tile.DisableColliderTrigger();
            UpdateTilePosition();
        }
        
        private void InsertTile(TileController tile, int index)
        {
            _tiles.Insert(index, tile);
            SetTileRotation(GetGo(tile), 90);
            tile.DisableColliderTrigger();
            UpdateTilePosition();
        }
        
        public void EnableColliderTrigger()
        {
            for (int i = 0; i < _tiles.Count; i++)
            {
                _tiles[i].EnableColliderTrigger();
            }
        }

        private void RemoveTile(TileController tile)
        {
            _tiles.Remove(tile);
            UpdateTilePosition();
        }
        
        private void UpdateTilePosition()
        {
            Vector3 transformPosition;
            if(_tiles.Count <= 0)
                return;
            for (int i = 0; i < _tiles.Count; i++)
            {
                transformPosition = GetGo(_tiles[i]).transform.position;
                transformPosition.x = _tilePositions[i].x;
                transformPosition.y = _background.bounds.max.y;
                transformPosition.z = _tilePositions[i].y;
                GetGo(_tiles[i]).transform.position = transformPosition;
            }
        }

        private void SetupTilePositionIntoStack()
        {
            float emptySpaceX = GetEmptySpaceX();
            float stackPositionZ = GetStackPositionZ();
            Vector2 stackPosition = Vector2.zero;
            float beforeBoundMaxX = _stackArea.bounds.min.x;
            for (int i = 0; i < _stackNumber; i++)
            {
                stackPosition.x = beforeBoundMaxX + emptySpaceX + _tileDefaultCollider.bounds.size.x / 2;
                stackPosition.y = stackPositionZ;
                _tilePositions.Add(stackPosition);
                beforeBoundMaxX = stackPosition.x + _tileDefaultCollider.bounds.size.x / 2;
            }
        }
        
        private float GetEmptySpaceX()
        {
            Bounds stackBounds = _stackArea.bounds;
            Bounds tileBounds = _tileDefaultCollider.bounds;
            float stackAreaLength = stackBounds.max.x - stackBounds.min.x;
            float tileLength = tileBounds.max.x - tileBounds.min.x;
            float emptySpace = stackAreaLength - tileLength * _stackNumber;
            float emptySpaceX = emptySpace / (_stackNumber + 1);
            return emptySpaceX;
        }

        private float GetStackPositionZ()
        {
            Bounds stackBounds = _stackArea.bounds;
            Bounds tileBounds = _tileDefaultCollider.bounds;
            float stackAreaLength = stackBounds.max.z - stackBounds.min.z;
            float tileLength = tileBounds.max.z - tileBounds.min.z;
            float emptySpace = stackAreaLength - tileLength;
            float stackPositionZ = stackBounds.min.z + emptySpace / 2;
            return stackPositionZ;
        }
    }
}
