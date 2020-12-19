using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ManyElementsTest
{
    public class BaseGridElement : VisualElement
    {
        public BaseGridElement()
        {
            AddToClassList("gridElement");
        }
    }
}