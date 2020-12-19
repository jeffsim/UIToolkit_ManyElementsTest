using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ManyElementsTest
{
    /* 
     * This test is the same as Grid_ManualLayout_RemoveOutOfViewportElementFromParent except this one 
     * updates transform rather than style
     */
    public class Grid_ManualLayout_RemoveOutOfViewportElementFromParent_WithTransform : Grid_ManualLayout_RemoveOutOfViewportElementFromParent
    {
        public static Grid_ManualLayout_RemoveOutOfViewportElementFromParent_WithTransform Instance;
        public override void PopulateWithTestElements()
        {
            Instance = this;
            gridElements.Clear();
            DetachFromParent(); // this makes a LOT of difference when adding elements!
            for (int i = 0; i < ManyElementsTestWindow.NumElementsToAddToGrid; i++)
            {
                var el = new GridElement_ManualLayout_RemoveOutOfViewportElementFromParent_WithTransform(ManyElementsTestWindow.TestTexture, "Item " + i, ScrollView);
                ScrollView.contentContainer.Add(el);
                gridElements.Add(el);
            }
            ReattachToParent();
            relayoutElements();
        }
    }

    public class GridElement_ManualLayout_RemoveOutOfViewportElementFromParent_WithTransform : GridElement_ManualLayout
    {
        public GridElement_ManualLayout_RemoveOutOfViewportElementFromParent_WithTransform(Texture2D image, string labelText, ScrollView scrollView) :
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
                    this.transform.position = new Vector3(x, y, transform.position.z);
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