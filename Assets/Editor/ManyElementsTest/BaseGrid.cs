using UnityEngine.UIElements;

namespace ManyElementsTest
{
    public abstract class BaseGrid : VisualElement
    {
        protected float gridElementSize = 128;
        public ScrollView ScrollView;

        protected virtual void relayoutElements()
        {

        }
        public abstract void PopulateWithTestElements();
        protected abstract void OnElementResize();

        public void SetElementSize(float newValue)
        {
            gridElementSize = newValue;
            OnElementResize();
        }
    }
}