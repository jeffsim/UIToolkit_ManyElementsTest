using UnityEngine.UIElements;

namespace ManyElementsTest
{
    public abstract class BaseGrid : VisualElement
    {
        protected float gridElementSize = 128;
        public ScrollView ScrollView;

        protected abstract void relayoutElements();
        public abstract void PopulateWithTestElements();

        public void SetElementSize(float newValue)
        {
            gridElementSize = newValue;
            relayoutElements();
        }
    }
}