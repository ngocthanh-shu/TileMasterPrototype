using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SO;
using Tiles;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        [TableList]
        public List<LevelInformation> levels;
        
        public TileSO tileSo;
        
        private GameManager _gameManager;

        public void Initialize(GameManager gameManager)
        {
            _gameManager = gameManager;
        }
        
        public LevelInformation GetLevel(int level)
        {
            LevelInformation levelInformation = SaveLoadData.LoadLevel(level);
            if (levelInformation != null)
            {
                levels.Add(levelInformation);
                return levelInformation;
            }
            _gameManager.ResetLevel();
            levelInformation = SaveLoadData.LoadLevel(1);
            return levelInformation;
        }

        [Button("Generate Levels")]
        public void GenerateLevels()
        {
            SaveLoadData.SaveLevels(levels);
        }
        
        public TileView GetTileView(string key)
        {
            foreach (var tileRef in tileSo.tileReferences)
            {
                if (tileRef.key == key)
                {
                    return tileRef.tile;
                }
            }

            return null;
        }
        
    }

    [Serializable]
    public class TileInformation
    {
        public string tile;
        
        [Header("1 Set = 3 Tiles")]
        public int quantitySet;
    }
    
    [Serializable]
    public class LevelInformation
    {
        public List<TileInformation> tiles;
        
        [Header("Time in seconds")]
        public int time;
    }
}

