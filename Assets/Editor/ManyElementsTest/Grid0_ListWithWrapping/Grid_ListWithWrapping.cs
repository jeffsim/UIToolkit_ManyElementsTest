using UnityEngine.UIElements;

namespace ManyElementsTest
{
    public class Grid_ListWithWrapping : BaseGrid
    {
        public override string ToString() => "ListWithWrapping";

        public new class UxmlFactory : UxmlFactory<Grid_ListWithWrapping, UxmlTraits> { }

        public Grid_ListWithWrapping()
        {
            Add(ScrollView = new ScrollView());
            ScrollView.contentContainer.AddToClassList("listWithWrapping");
        }
        
        protected override void OnElementResize()
        {
            foreach (var el in ScrollView.contentContainer.Children())
                ((GridElement_ListWithWrapping)el).SetSize(gridElementSize);
        }

        public override void PopulateWithTestElements()
        {
            for (int i = 0; i < ManyElementsTestWindow.NumElementsToAddToGrid; i++)
            {
                var el = GridElement_ListWithWrapping.GetFromPool(ManyElementsTestWindow.TestTexture, "Item " + i);
                ScrollView.contentContainer.Add(el);
            }
            OnElementResize();
        }
    }
}