using UnityEngine;
using UnityEngine.UIElements;

namespace ManyElementsTest
{
    /* 
     * This test is the same as Grid_ManualLayout_RemoveOutOfViewportElementFromParent_WithTransform except this one 
     * tries to avoid the alloc on the label by disabling it instead of setting text
     */
    public class Grid_ManualLayout_RemoveOutOfViewportElementFromParent_SetLabelDisabled : Grid_ManualLayout_RemoveOutOfViewportElementFromParent
    {
        public override void PopulateWithTestElements()
        {
            Instance = this;
            gridElements.Clear();
            DetachFromParent(); // this makes a LOT of difference when adding elements!
            for (int i = 0; i < ManyElementsTestWindow.NumElementsToAddToGrid; i++)
            {
                var el = new GridElement_ManualLayout_RemoveOutOfViewportElementFromParent_SetLabelDisabled(ManyElementsTestWindow.TestTexture, "Item " + i, ScrollView);
                ScrollView.contentContainer.Add(el);
                gridElements.Add(el);
            }
            ReattachToParent();
            relayoutElements();
        }
    }

    public class GridElement_ManualLayout_RemoveOutOfViewportElementFromParent_SetLabelDisabled : GridElement_ManualLayout
    {
        public GridElement_ManualLayout_RemoveOutOfViewportElementFromParent_SetLabelDisabled(Texture2D image, string labelText, ScrollView scrollView) :
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
                var wasVisible = parent != null;
                if (!wasVisible)
                    scrollView.contentContainer.Add(this);

                if (!wasVisible || x != lastX || y != lastY)
                    transform.position = new Vector3(x, y, transform.position.z);

                if (!wasVisible || size != lastSizeSet)
                {
                    style.width = size;
                    style.height = size;
                    lastSizeSet = size;

                    if (label != null)
                    {
                        var labelIsVisible = size > 100;
                        labelWasVisible = label.style.opacity == 1;
                        if (labelIsVisible != labelWasVisible)
                            label.style.opacity = labelIsVisible ? 1 : 0;
                    }
                }
            }
        }
    }
}