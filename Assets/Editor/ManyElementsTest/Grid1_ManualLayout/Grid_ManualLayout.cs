using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace ManyElementsTest
{
    public class Grid_ManualLayout : BaseGrid
    {
        public override string ToString() => "ManualLayout";
      
        public new class UxmlFactory : UxmlFactory<Grid_ManualLayout, UxmlTraits> { }

        List<GridElement_ManualLayout> gridElements = new List<GridElement_ManualLayout>();

        public Grid_ManualLayout()
        {
            Add(ScrollView = new ScrollView());

            // When the user scrolls the scrollview, ensure all items have valid tiles
            ScrollView.verticalScroller.valueChanged += val => relayoutElements();

            // this one is needed for window resize
            RegisterCallback<GeometryChangedEvent>(evt => relayoutElements());
        }

        public override void PopulateWithTestElements()
        {
            for (int i = 0; i < ManyElementsTestWindow.NumElementsToAddToGrid; i++)
            {
                var el = GridElement_ManualLayout.GetFromPool(ManyElementsTestWindow.TestTexture, "Item " + i);
                ScrollView.contentContainer.Add(el);
                gridElements.Add(el);
            }
            relayoutElements();
        }

        protected override void relayoutElements()
        {
            float gridWidth = ScrollView.contentContainer.layout.width;
            float paddedTileSize = gridElementSize + 6;
            int maxTilesPerRow = Math.Max(1, (int)(gridWidth / paddedTileSize));

            // Calculate vert coord of the scrolled visible rect
            float viewY1 = ScrollView.scrollOffset.y;
            float viewY2 = viewY1 + ScrollView.layout.height;

            for (int i = 0; i < gridElements.Count; i++)
            {
                float tileX1 = (i % maxTilesPerRow) * paddedTileSize;
                float tileY1 = (i / maxTilesPerRow) * paddedTileSize;
                float tileY2 = tileY1 + gridElementSize;

                // Only have to check for vert bounds
                bool isInViewport = tileY1 < viewY2 && tileY2 >= viewY1;
                gridElements[i].SetBounds(tileX1, tileY1, gridElementSize, isInViewport);
            }
            ScrollView.contentContainer.style.height = ((gridElements.Count - 1) / maxTilesPerRow + 1) * paddedTileSize;
        }
    }
}