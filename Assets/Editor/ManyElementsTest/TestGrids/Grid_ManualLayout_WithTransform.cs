using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ManyElementsTest
{
    /* 
     * 
     * This test is the same as Grid_ManualLayout except this one modifies transform rather than style.left/style.right
     * 
     * 
     */
    public class Grid_ManualLayout_WithTransform : BaseGrid
    {
        public new class UxmlFactory : UxmlFactory<Grid_ManualLayout_WithTransform, UxmlTraits> { }

        List<GridElement_ManualLayout_WithTransform> gridElements = new List<GridElement_ManualLayout_WithTransform>();

        public Grid_ManualLayout_WithTransform()
        {
            Add(ScrollView = new ScrollView());

            // When the user scrolls the scrollview, ensure all items have valid tiles
            ScrollView.verticalScroller.valueChanged += val => relayoutElements();

            // this one is needed for window resize
            RegisterCallback<GeometryChangedEvent>(evt => relayoutElements());
        }

        protected override void OnResizeAllElements()
        {
            relayoutElements();
        }

        public override void PopulateWithTestElements()
        {
            for (int i = 0; i < ManyElementsTestWindow.NumElementsToAddToGrid; i++)
            {
                var el = new GridElement_ManualLayout_WithTransform(ManyElementsTestWindow.TestTexture, "Item " + i);
                ScrollView.contentContainer.Add(el);
                gridElements.Add(el);
            }
            relayoutElements();
        }

        protected override void relayoutElements()
        {
            float paddedTileSize = gridElementSize + 6;
            int maxTilesPerRow = Math.Max(1, (int)(ScrollView.contentContainer.layout.width / paddedTileSize));

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

    public class GridElement_ManualLayout_WithTransform : BaseGridElement
    {
        Label label;
        string labelText;

        // Fields which are used to minimize style sets unless changed
        float lastSizeSet;
        DisplayStyle lastDisplayStyle;
        float lastX;
        float lastY;
        bool wasVisible;

        public GridElement_ManualLayout_WithTransform(Texture2D image, string labelText)
        {
            Add(label = new Label());
            AddToClassList("manualLayout");

            // Reset variables that are used to minimize style changes
            lastSizeSet = float.MaxValue;
            lastDisplayStyle = (DisplayStyle)(-1);
            lastX = float.MaxValue;
            lastY = float.MaxValue;
            wasVisible = false;
            style.backgroundImage = image;
            this.labelText = labelText;
            label.text = labelText;
        }

        public void SetBounds(float x, float y, float size, bool isVisible)
        {
            if (!isVisible)
            {
                // TODO (PERF): Note to self: commenting out this and the inverse below makes scrolling super fast...
                // but it also breaks when resizing.
                if (lastDisplayStyle != DisplayStyle.None)
                {
                    style.display = DisplayStyle.None;
                    lastDisplayStyle = DisplayStyle.None;
                }
            }
            else
            {
                // We're visible.  If we weren't visible before, then force update everything since we may not have updated
                // The following may be overkill; I'm unclear on whether or not setting a style to its existing value is expensive.
                // Update display (if changed)
                if (!wasVisible || lastDisplayStyle != DisplayStyle.Flex)
                {
                    style.display = DisplayStyle.Flex;
                    lastDisplayStyle = DisplayStyle.Flex;
                }

                // Update location (if changed).  I'm assuming changing one of [left,top] is as expensive as changing both
                if (!wasVisible || x != lastX || y != lastY)
                {
                    this.transform.position = new Vector3(x, y, transform.position.z);
                    //style.left = lastX = x;
                    //style.top = lastY = y;
                }

                // Update size (if changed)
                if (!wasVisible || size != lastSizeSet)
                {
                    style.width = size;
                    style.height = size;
                    label.text = size > 100 ? labelText : "";
                    lastSizeSet = size;
                }
            }
            wasVisible = isVisible;
        }
    }
}