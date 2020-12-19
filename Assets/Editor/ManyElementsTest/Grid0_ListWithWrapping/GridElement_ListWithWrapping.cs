using UnityEngine;
using UnityEngine.UIElements;

namespace ManyElementsTest
{
    public class GridElement_ListWithWrapping : BaseGridElement
    {
        Label label;
        string labelText;

        public static GridElement_ListWithWrapping GetFromPool(Texture2D image, string labelText)
        {
            var el = GridElementPool_ListWithWrapping.GetFromPool();
            el.initialize(image, labelText);
            return el;
        }
        public void ReturnToPool() => GridElementPool_ListWithWrapping.ReturnToPool(this);

        public GridElement_ListWithWrapping()
        {
            Add(label = new Label());
            AddToClassList("listWithWrapping");
        }

        void initialize(Texture2D image, string text)
        {
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