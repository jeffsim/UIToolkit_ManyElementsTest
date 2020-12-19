using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ManyElementsTest
{
    public class BaseGridElement : VisualElement
    {
        Label label;
        string labelText;

        public BaseGridElement()
        {
            AddToClassList("gridElement");
        }
    }
}