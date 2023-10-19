using System.Collections.Generic;
using Tiles;

namespace Utils
{
    public static class SortTiles
    {
        private static int _leftID, _rightID;
    
        public static List<TileController> SortTilesControllers(List<TileController> tileControllers)
        {
            return StableSort(tileControllers);
        }
    
        private static List<TileController> StableSort(List<TileController> tiles)
        {
            if (tiles.Count <= 1)
                return tiles;
        
            int mid = tiles.Count / 2;
            List<TileController> left = new List<TileController>();
            List<TileController> right = new List<TileController>();
        
            for (int i = 0; i < mid; i++)
                left.Add(tiles[i]);
        
            for (int i = mid; i < tiles.Count; i++)
                right.Add(tiles[i]);
        
            left = StableSort(left);
            right = StableSort(right);
        
            return Merge(left, right);
        }
    
        private static List<TileController> Merge(List<TileController> left, List<TileController> right)
        {
            List<TileController> result = new List<TileController>();
            int i = 0, j = 0;

            while (i < left.Count && j < right.Count)
            {
                _leftID = GetTileID(left[i]);
                _rightID = GetTileID(right[j]);
                if (_leftID <= _rightID)
                {
                    result.Add(left[i]);
                    i++;
                }
                else
                {
                    result.Add(right[j]);
                    j++;
                }
            }
        
            while (i < left.Count)
            {
                result.Add(left[i]);
                i++;
            }
        
            while (j < right.Count)
            {
                result.Add(right[j]);
                j++;
            }
        
            return result;
        }
    
        private static int GetTileID(TileController tile)
        {
            return tile.GetView().GetTileID();
        }
    }
}
