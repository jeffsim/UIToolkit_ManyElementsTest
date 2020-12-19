using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ManyElementsTest
{
    /* 
     * In this test, grid elements are absolutely positioned, and set to display=none when out of the viewport
     * The latter requires us to relayout on every scroll
     */
    public class Grid_ManualLayout : BaseGrid
    {
        protected List<GridElement_ManualLayout> gridElements = new List<GridElement_ManualLayout>();

        public Grid_ManualLayout()
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
            gridElements.Clear();

            ScrollView.contentContainer.Clear();

            DetachFromParent(); // this makes a LOT of difference when adding elements!

            for (int i = 0; i < ManyElementsTestWindow.NumElementsToAddToGrid; i++)
            {
                var el = new GridElement_ManualLayout(ManyElementsTestWindow.TestTexture, "Item " + i);
                ScrollView.contentContainer.Add(el);
                gridElements.Add(el);
            }
            ReattachToParent();
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

    public class GridElement_ManualLayout : BaseGridElement
    {
        protected Label label;
        protected string labelText;

        // Fields which are used to minimize style sets unless changed
        protected float lastSizeSet = float.MaxValue;
        protected DisplayStyle lastDisplayStyle = (DisplayStyle)(-1);
        protected float lastX = float.MaxValue;
        protected float lastY = float.MaxValue;
        protected bool wasVisible = false;
        protected bool labelWasVisible = false;

        public GridElement_ManualLayout(Texture2D image, string labelText)
        {
            AddToClassList("manualLayout");
            style.backgroundImage = image;
            
            if (labelText != null)
            {
                Add(label = new Label());
                this.labelText = labelText;
                label.text = labelText;
            }
        }

        public virtual void SetBounds(float x, float y, float size, bool isVisible)
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
                    style.left = lastX = x;
                    style.top = lastY = y;
                }

                // Update size (if changed)
                if (!wasVisible || size != lastSizeSet)
                {
                    style.width = size;
                    style.height = size;
                    if (label != null)
                    {
                        var labelIsVisible = size > 100;
                        if (labelIsVisible != labelWasVisible)
                        {
                            labelWasVisible = labelIsVisible;
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