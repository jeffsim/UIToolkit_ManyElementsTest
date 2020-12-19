using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ManyElementsTest
{
    /* 
     * This test is the same as Grid_ManualLayout except this one completely removes elements from parent when out of viewport
     */
    public class Grid_ManualLayout_RemoveOutOfViewportElementFromParent : Grid_ManualLayout
    {
        public static Grid_ManualLayout_RemoveOutOfViewportElementFromParent Instance;
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
            // DetachFromParent(); 

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

    public class GridElement_ManualLayout_RemoveOutOfViewportElementFromParent : GridElement_ManualLayout
    {
        public GridElement_ManualLayout_RemoveOutOfViewportElementFromParent(Texture2D image, string labelText, ScrollView scrollView) :
            base(image, labelText)
        {
            this.scrollView = scrollView;
        }

        ScrollView scrollView;

        public override void SetBounds(float x, float y, float size, bool isVisible)
        {
            if (!isVisible)
            {
                if (parent != null)
                    scrollView.contentContainer.Remove(this);
            }
            else
            {
                if (parent == null)
                    scrollView.contentContainer.Add(this);

                if (!wasVisible || x != lastX || y != lastY)
                {
                    style.left = lastX = x;
                    style.top = lastY = y;
                }

                if (!wasVisible || size != lastSizeSet)
                {
                    style.width = size;
                    style.height = size;
                    if (label != null)
                    {
                        var labelIsVisible = size > 100;
                        var labelWasVisible1 = label.text.Length > 0;
                        if (labelIsVisible != labelWasVisible1)
                        {
                            label.text = labelIsVisible ? labelText : "";
                        }
                    }
                    lastSizeSet = size;
                }
            }
            wasVisible = isVisible;
        }
    }
}