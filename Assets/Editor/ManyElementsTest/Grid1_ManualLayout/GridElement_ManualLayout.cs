using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ManyElementsTest
{
    public class GridElement_ManualLayout : BaseGridElement
    {
        Label label;
        string labelText;

        // Fields which are used to minimize style sets unless changed
        float lastSizeSet;
        DisplayStyle lastDisplayStyle;
        float lastX;
        float lastY;
        bool wasVisible;

        public static GridElement_ManualLayout GetFromPool(Texture2D image, string labelText)
        {
            var el = GridElementPool_ManualLayout.GetFromPool();
            el.initialize(image, labelText);
            return el;
        }

        public void ReturnToPool() => GridElementPool_ManualLayout.ReturnToPool(this);

        public GridElement_ManualLayout()
        {
#if USE_UXML_FOR_GRID_ELEMENT
            // Load our elements from uxml
            var gridElementUxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ManyElementsTest/resources/GridElement.uxml"));
            Add(UI = gridElementUxml.CloneTree());
            Label = UI.Q<Label>();
#else
            // Create our elements manually
            Add(label = new Label());
#endif
        }

        void initialize(Texture2D image, string labelText)
        {
            // Reset variables that are used to minimize style changes
            lastSizeSet = -1000;
            lastDisplayStyle = (DisplayStyle)(-1);
            lastX = float.MaxValue;
            lastY = float.MaxValue;
            wasVisible = false;

            style.backgroundImage = image;

            this.labelText = labelText;
            label.text = labelText;
        }

        public void SetBounds(float x, float y, float size, bool isVisible)
        {
            if (!isVisible)
            {
                // TODO (PERF): Note to self: commenting out this and the inverse below makes scrolling super fast...
                // but it also breaks when resizing.
                if (lastDisplayStyle != DisplayStyle.None)
                {
                    style.display = DisplayStyle.None;
                    lastDisplayStyle = DisplayStyle.None;
                }
            }
            else
            {
                // We're visible.  If we weren't visible before, then force update everything since we may not have updated
                // The following may be overkill; I'm unclear on whether or not setting a style to its existing value is expensive.
                // Update display (if changed)
                if (!wasVisible || lastDisplayStyle != DisplayStyle.Flex)
                {
                    style.display = DisplayStyle.Flex;
                    lastDisplayStyle = DisplayStyle.Flex;
                }

                // Update location (if changed).  I'm assuming changing one of [left,top] is as expensive as changing both
                if (!wasVisible || x != lastX || y != lastY)
                {
                    //this.transform.position = new Vector3(x, y, transform.position.z);
                    style.left = lastX = x;
                    style.top = lastY = y;
                }

                // Update size (if changed)
                if (!wasVisible || size != lastSizeSet)
                {
                    style.width = size;
                    style.height = size;
                    label.text = size > 100 ? labelText : "";
                    lastSizeSet = size;
                }
            }
            wasVisible = isVisible;
        }
    }
}