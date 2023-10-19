using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Tiles;
using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "Data/Tiles", fileName = "Tiles")]
    public class TileSO : ScriptableObject
    {
        [TableList] public List<TileReference> tileReferences;
    }
    
    [Serializable]
    public class TileReference
    {
        public string key;
        public TileView tile;
    }
}
