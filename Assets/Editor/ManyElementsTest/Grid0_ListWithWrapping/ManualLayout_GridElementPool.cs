using System.Collections.Generic;

namespace ManyElementsTest
{
    public static class GridElementPool_ListWithWrapping
    {
        static Stack<GridElement_ListWithWrapping> pool = new Stack<GridElement_ListWithWrapping>();

        public static GridElement_ListWithWrapping GetFromPool()
        {
            if (pool.Count > 0)
                return pool.Pop();
            return new GridElement_ListWithWrapping();
        }

        public static void ReturnToPool(GridElement_ListWithWrapping assetTile) => pool.Push(assetTile);
        public static void ClearPool() => pool.Clear();
    }
}