﻿using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ManyElementsTest
{
    public class ManyElementsTestWindow : EditorWindow
    {
        public static int NumElementsToAddToGrid = 10000;
        public static int NumTestPassesToRun = 3;
        public static Texture2D TestTexture;

        [MenuItem("Window/ManyElementsTest %T")]
        static void Init() => GetWindow<ManyElementsTestWindow>();

        void OnEnable()
        {
            // Disallow resizing to ensure consistent results across runs
            minSize = new Vector2(800, 1000);
            maxSize = new Vector2(800, 1000);

            rootVisualElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/ManyElementsTest/resources/style.uss"));
            var xmlAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ManyElementsTest/resources/TestWindow.uxml");
            xmlAsset.CloneTree(rootVisualElement);

            TestTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/ManyElementsTest/resources/testImage.png");

            var gridContainer = rootVisualElement.Q("testGridContainer");
            var sizeSlider = rootVisualElement.Q<Slider>("sizeSlider");

            // Hook up run tests button
            rootVisualElement.Q<Button>("RunTests").clicked += () => EditorCoroutines.Execute(TestRunner.RunTests(sizeSlider, gridContainer));
            rootVisualElement.Q<Button>("ShowOne").clicked += () => showOneGrid(gridContainer, sizeSlider);

            var prepopulate = false;
            if (prepopulate)
                showOneGrid(gridContainer, sizeSlider);
        }

        void showOneGrid(VisualElement gridContainer, Slider sizeSlider)
        {
            BaseGrid grid;

            // Uncomment the one you want to view:
            //gridContainer.Add(grid = new Grid_ListWithWrapping());
            gridContainer.Add(grid = new Grid_ManualLayout_WithTransform());
            //gridContainer.Add(grid = new Grid_ManualLayout());
            grid.PopulateWithTestElements();

            // Setup slider
            EventCallback<ChangeEvent<float>> sliderCB;
            sizeSlider.RegisterValueChangedCallback(sliderCB = (val) => grid.SetElementSize(val.newValue));
            sizeSlider.value = (sizeSlider.highValue - sizeSlider.lowValue) / 2;
        }
    }
}
