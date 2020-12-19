using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ManyElementsTest
{
    /* 
     * This test is the same as Grid_ManualLayout except this one modifies transform rather than style.left/style.right
     */
    public class Grid_ManualLayout_WithTransform : Grid_ManualLayout
    {
        public override void PopulateWithTestElements()
        {
            gridElements.Clear();
            DetachFromParent(); // this makes a LOT of difference when adding elements!
            for (int i = 0; i < ManyElementsTestWindow.NumElementsToAddToGrid; i++)
            {
                var el = new GridElement_ManualLayout_WithTransform(ManyElementsTestWindow.TestTexture, "Item " + i);
                ScrollView.contentContainer.Add(el);
                gridElements.Add(el);
            }
            ReattachToParent();
            relayoutElements();
        }
    }

    public class GridElement_ManualLayout_WithTransform : GridElement_ManualLayout
    {
        public GridElement_ManualLayout_WithTransform(Texture2D image, string labelText) :
            base(image, labelText)
        {
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
                            label.text = labelIsVisible ? labelText : "";
                    }
                    lastSizeSet = size;
                }
            }
            wasVisible = isVisible;
        }
    }
}