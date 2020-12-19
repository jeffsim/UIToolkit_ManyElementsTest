using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ManyElementsTest
{
    /* 
     * Same as manual layout but with no label.  curious how much is going to that.
     */
    public class Grid_ManualLayout_NoLabel : Grid_ManualLayout
    {
        public override void PopulateWithTestElements()
        {
            gridElements.Clear();
            DetachFromParent(); // this makes a LOT of difference when adding elements!
            for (int i = 0; i < ManyElementsTestWindow.NumElementsToAddToGrid; i++)
            {
                var el = new GridElement_ManualLayout_NoLabel(ManyElementsTestWindow.TestTexture);
                ScrollView.contentContainer.Add(el);
                gridElements.Add(el);
            }
            ReattachToParent();
            relayoutElements();
        }
    }

    public class GridElement_ManualLayout_NoLabel : GridElement_ManualLayout
    {
        public GridElement_ManualLayout_NoLabel(Texture2D image) : base(image, null)
        {
            AddToClassList("manualLayout");

            // Reset variables that are used to minimize style changes
            lastSizeSet = float.MaxValue;
            lastDisplayStyle = (DisplayStyle)(-1);
            lastX = float.MaxValue;
            lastY = float.MaxValue;
            wasVisible = false;
            style.backgroundImage = image;
        }

        public override void SetBounds(float x, float y, float size, bool isVisible)
        {
            if (!isVisible)
            {
                if (lastDisplayStyle != DisplayStyle.None)
                {
                    style.display = DisplayStyle.None;
                    lastDisplayStyle = DisplayStyle.None;
                }
            }
            else
            {
                if (!wasVisible || lastDisplayStyle != DisplayStyle.Flex)
                {
                    style.display = DisplayStyle.Flex;
                    lastDisplayStyle = DisplayStyle.Flex;
                }

                if (!wasVisible || x != lastX || y != lastY)
                {
                    //this.transform.position = new Vector3(x, y, transform.position.z);
                    style.left = lastX = x;
                    style.top = lastY = y;
                }

                if (!wasVisible || size != lastSizeSet)
                {
                    style.width = size;
                    style.height = size;
                }
            }
            wasVisible = isVisible;
        }
    }
}