using UnityEngine;
using UnityEngine.UIElements;

namespace ManyElementsTest
{
    /* 
     * This test is the same as Grid_ManualLayout_RemoveOutOfViewportElementFromParent except this one 
     * updates transform rather than style
     */
    public class Grid_ManualLayout_RemoveOutOfViewportElementFromParent_SetDisabled : Grid_ManualLayout_RemoveOutOfViewportElementFromParent
    {
        public override void PopulateWithTestElements()
        {
            Instance = this;
            gridElements.Clear();
            DetachFromParent(); // this makes a LOT of difference when adding elements!
            for (int i = 0; i < ManyElementsTestWindow.NumElementsToAddToGrid; i++)
            {
                var el = new GridElement_ManualLayout_RemoveOutOfViewportElementFromParent_SetDisabled(ManyElementsTestWindow.TestTexture, "Item " + i);
                ScrollView.contentContainer.Add(el);
                gridElements.Add(el);
            }
            ReattachToParent();
            relayoutElements();
        }
    }

    public class GridElement_ManualLayout_RemoveOutOfViewportElementFromParent_SetDisabled : GridElement_ManualLayout
    {
        public GridElement_ManualLayout_RemoveOutOfViewportElementFromParent_SetDisabled(Texture2D image, string labelText) :
            base(image, labelText)
        {
        }

        public override void SetBounds(float x, float y, float size, bool isVisible)
        {
            if (!isVisible)
            {
                if (enabledSelf)
                    SetEnabled(false);
            }
            else
            {
                if (!enabledSelf)
                    SetEnabled(true);

                if (!wasVisible || x != lastX || y != lastY)
                    transform.position = new Vector3(lastX = x, lastY = y, transform.position.z);

                if (!wasVisible || size != lastSizeSet)
                {
                    style.width = size;
                    style.height = size;
                    lastSizeSet = size;

                    if (label != null)
                    {
                        var labelIsVisible = size > 100;
                        labelWasVisible = label.text.Length > 0;
                        if (labelIsVisible != labelWasVisible)
                        {
                            label.text = labelIsVisible ? labelText : "";
                        }
                    }
                }
            }
        }
    }
}