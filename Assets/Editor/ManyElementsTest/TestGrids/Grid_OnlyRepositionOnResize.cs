using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ManyElementsTest
{
    /* 
     * In this test, grid elements are absolutely positioned once.  Unlike Grid_ManualLayout, we don't try
     * to modify display; thus, we don't need to relayout on scroll.
     */
    public class Grid_OnlyRepositionOnResize : BaseGrid
    {
        protected List<GridElement_OnlyRepositionOnResize> gridElements = new List<GridElement_OnlyRepositionOnResize>();

        public Grid_OnlyRepositionOnResize()
        {
            Add(ScrollView = new ScrollView());

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
                var el = new GridElement_OnlyRepositionOnResize(ManyElementsTestWindow.TestTexture, "Item " + i);
                ScrollView.contentContainer.Add(el);
                gridElements.Add(el);
            }
            ReattachToParent();
            relayoutElements();
        }

        protected override void relayoutElements()
        {
            // NOTE: The following doesn't make much different on relayouts; only on bulk-adding/removing.
            // hml; therefore this might make a diff. on the grids that do a display none/flex change
            // similarly: try a version of those that instead of display none/flex, dodse a removefromparent
            //  when out of viewport?
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
                gridElements[i].OnGridResized(tileX1, tileY1, gridElementSize);
            }
            ScrollView.contentContainer.style.height = ((gridElements.Count - 1) / maxTilesPerRow + 1) * paddedTileSize;
        }
    }

    public class GridElement_OnlyRepositionOnResize : BaseGridElement
    {
        protected Label label;
        protected string labelText;

        // Fields which are used to minimize style sets unless changed
        protected float lastSizeSet = float.MaxValue;
        protected DisplayStyle lastDisplayStyle = (DisplayStyle)(-1);
        protected float lastX = float.MaxValue;
        protected float lastY = float.MaxValue;
        protected bool labelWasVisible = false;

        public GridElement_OnlyRepositionOnResize(Texture2D image, string labelText)
        {
            AddToClassList("manualLayout");
            style.backgroundImage = image;
            Add(label = new Label());
            this.labelText = labelText;
            label.text = labelText;
        }

        public virtual void OnGridResized(float x, float y, float size)
        {
            if (x != lastX || y != lastY)
            {
                //this.transform.position = new Vector3(x, y, transform.position.z);
                style.left = lastX = x;
                style.top = lastY = y;
            }

            if (size != lastSizeSet)
            {
                style.width = size;
                style.height = size;
                var labelIsVisible = size > 100;
                if (labelIsVisible != labelWasVisible)
                {
                    labelWasVisible = labelIsVisible;
                    label.text = labelIsVisible ? labelText : "";
                }
                lastSizeSet = size;
            }
        }
    }
}