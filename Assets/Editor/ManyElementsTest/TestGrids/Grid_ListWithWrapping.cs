using UnityEngine;
using UnityEngine.UIElements;

namespace ManyElementsTest
{
    /* 
     * 
     * This test uses flex-wrap and lets uielements handle all positioning
     * 
     * 
     */
    public class Grid_ListWithWrapping : BaseGrid
    {
        public new class UxmlFactory : UxmlFactory<Grid_ListWithWrapping, UxmlTraits> { }

        public Grid_ListWithWrapping()
        {
            Add(ScrollView = new ScrollView());
            ScrollView.contentContainer.AddToClassList("listWithWrapping");
        }

        protected override void OnResizeAllElements()
        {
            // Note: unparenting and reparenting is no faster.
            // var p = parent;
            // RemoveFromHierarchy();
            
            var children = ScrollView.contentContainer.Children();
            foreach (var el in children)
                ((GridElement_ListWithWrapping)el).SetSize(gridElementSize);
            
            // p.Add(this);
        }

        public override void PopulateWithTestElements()
        {
            for (int i = 0; i < ManyElementsTestWindow.NumElementsToAddToGrid; i++)
                ScrollView.contentContainer.Add(new GridElement_ListWithWrapping(ManyElementsTestWindow.TestTexture, "Item " + i));
            OnResizeAllElements();
        }
    }

    public class GridElement_ListWithWrapping : BaseGridElement
    {
        Label label;
        string labelText;

        public GridElement_ListWithWrapping(Texture2D image, string text)
        {
            Add(label = new Label());
            AddToClassList("listWithWrapping");

            style.backgroundImage = image;
            label.text = labelText = text;
        }

        public void SetSize(float size)
        {
            style.width = size;
            style.height = size;

            label.text = size > 100 ? labelText : "";
        }
    }
}