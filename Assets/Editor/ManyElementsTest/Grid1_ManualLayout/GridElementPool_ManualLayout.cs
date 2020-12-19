using System.Collections.Generic;

namespace ManyElementsTest
{
    public static class GridElementPool_ManualLayout
    {
        static Stack<GridElement_ManualLayout> pool = new Stack<GridElement_ManualLayout>();

        public static GridElement_ManualLayout GetFromPool()
        {
            if (pool.Count > 0)
                return pool.Pop();
            return new GridElement_ManualLayout();
        }

        public static void ReturnToPool(GridElement_ManualLayout assetTile) => pool.Push(assetTile);
        public static void ClearPool() => pool.Clear();
    }
}