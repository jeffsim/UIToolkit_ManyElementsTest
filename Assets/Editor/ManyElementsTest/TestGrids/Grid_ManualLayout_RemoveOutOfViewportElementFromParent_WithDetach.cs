using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ManyElementsTest
{
    /* 
     * This test is the same as Grid_ManualLayout_RemoveOutOfViewportElementFromParent except this one 
     * detaches from the ui while relaying out
     */
    public class Grid_ManualLayout_RemoveOutOfViewportElementFromParent_WithDetach : Grid_ManualLayout
    {
        public static Grid_ManualLayout_RemoveOutOfViewportElementFromParent_WithDetach Instance;
        public override void PopulateWithTestElements()
        {
            Instance = this;
            gridElements.Clear();
            DetachFromParent(); // this makes a LOT of difference when adding elements!
            for (int i = 0; i < ManyElementsTestWindow.NumElementsToAddToGrid; i++)
            {
                var el = new GridElement_ManualLayout_RemoveOutOfViewportElementFromParent(ManyElementsTestWindow.TestTexture, "Item " + i, ScrollView);
                ScrollView.contentContainer.Add(el);
                gridElements.Add(el);
            }
            ReattachToParent();
            relayoutElements();
        }

        protected override void relayoutElements()
        {
            // DON'T do this here.  Slows it down a lot.  Only helpful on add/remove
            // Leaving here since that's this test
            DetachFromParent();

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

            ReattachToParent();
        }
    }
}