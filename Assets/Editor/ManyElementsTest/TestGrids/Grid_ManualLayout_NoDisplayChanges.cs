using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ManyElementsTest
{
    /* 
     * This test is the same as Grid_ManualLayout except this one moves out-of-viewport elements far offscreen 
     * rather than setting display=none.
     */
    public class Grid_ManualLayout_NoDisplayChanges : Grid_ManualLayout
    {
        public override void PopulateWithTestElements()
        {
            gridElements.Clear();
            DetachFromParent(); // this makes a LOT of difference!
            for (int i = 0; i < ManyElementsTestWindow.NumElementsToAddToGrid; i++)
            {
                var el = new GridElement_ManualLayout_NoDisplayChanges(ManyElementsTestWindow.TestTexture, "Item " + i);
                ScrollView.contentContainer.Add(el);
                gridElements.Add(el);
            }
            ReattachToParent();
            relayoutElements();
        }
    }

    public class GridElement_ManualLayout_NoDisplayChanges : GridElement_ManualLayout
    {
        public GridElement_ManualLayout_NoDisplayChanges(Texture2D image, string labelText) : base(image, labelText)
        {
        }

        public override void SetBounds(float x, float y, float size, bool isVisible)
        {
            if (!isVisible)
            {
                style.left = lastX = -1000;
            }
            else
            {
                if (!wasVisible || x != lastX || y != lastY)
                {
                    // this.transform.position = new Vector3(x, y, transform.position.z);
                    style.left = lastX = x;
                    style.top = lastY = y;
                }

                // Update size (if changed)
                if (!wasVisible || size != lastSizeSet)
                {
                    style.width = size;
                    style.height = size;
                   
                    var labelIsVisible = size > 100;
                    // if (labelIsVisible != labelWasVisible)
                    {
                        label.text = labelIsVisible ? labelText : "";
                        labelWasVisible = labelIsVisible;
                    }
                    lastSizeSet = size;
                }
            }
            wasVisible = isVisible;
        }
    }
}