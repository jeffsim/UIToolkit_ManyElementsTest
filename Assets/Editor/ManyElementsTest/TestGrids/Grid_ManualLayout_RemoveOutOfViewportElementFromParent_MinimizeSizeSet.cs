using UnityEngine;
using UnityEngine.UIElements;

namespace ManyElementsTest
{
    /* 
     * This test is the same as Grid_ManualLayout_RemoveOutOfViewportElementFromParent_WithTransform except this one 
     * avoids some unnecessary size sets
     */
    public class Grid_ManualLayout_RemoveOutOfViewportElementFromParent_MinimizeSizeSet : Grid_ManualLayout_RemoveOutOfViewportElementFromParent
    {
        public override void PopulateWithTestElements()
        {
            Instance = this;
            gridElements.Clear();
            DetachFromParent(); // this makes a LOT of difference when adding elements!
            for (int i = 0; i < ManyElementsTestWindow.NumElementsToAddToGrid; i++)
            {
                var el = new GridElement_ManualLayout_RemoveOutOfViewportElementFromParent_MinimizeSizeSet(ManyElementsTestWindow.TestTexture, "Item " + i, ScrollView);
                ScrollView.contentContainer.Add(el);
                gridElements.Add(el);
            }
            ReattachToParent();
            relayoutElements();
        }

        protected override void OnResizeAllElements()
        {
            for (int i = 0; i < gridElements.Count; i++)
                ((GridElement_ManualLayout_RemoveOutOfViewportElementFromParent_MinimizeSizeSet)gridElements[i]).SetSize(gridElementSize);
            relayoutElements();
        }
    }

    public class GridElement_ManualLayout_RemoveOutOfViewportElementFromParent_MinimizeSizeSet : GridElement_ManualLayout
    {
        public GridElement_ManualLayout_RemoveOutOfViewportElementFromParent_MinimizeSizeSet(Texture2D image, string labelText, ScrollView scrollView) :
            base(image, labelText)
        {
            this.scrollView = scrollView;
        }

        ScrollView scrollView;

        public void SetSize(float size)
        {
            style.width = size;
            style.height = size;
        }

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
                    transform.position = new Vector3(lastX = x, lastY = y, transform.position.z);

                var labelIsVisible = size > 100;
                labelWasVisible = label.text.Length > 0;
                if (labelIsVisible != labelWasVisible)
                    label.text = labelIsVisible ? labelText : "";
            }
        }
    }
}