using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ManyElementsTest
{
    public class ManyElementsTestWindow : EditorWindow
    {
        public static int NumElementsToAddToGrid = 20000;
        public static int NumTestPassesToRun = 3;
        public static Texture2D TestTexture;

        [MenuItem("Window/ManyElementsTest %T")]
        static void Init() => GetWindow<ManyElementsTestWindow>();

        void OnEnable()
        {
            // Disallow resizing to ensure consistent results across runs
            minSize = maxSize = new Vector2(1000, 800);

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
            BaseGrid grid = gridContainer.Q<BaseGrid>();
            if (grid != null)
                gridContainer.Remove(grid);

            // Uncomment the one you want to view:
            //gridContainer.Add(grid = new Grid_ListWithWrapping());
            // gridContainer.Add(grid = new Grid_ManualLayout_WithTransform());
            // gridContainer.Add(grid = new Grid_ManualLayout_NoDisplayChanges());
            // gridContainer.Add(grid = new Grid_ManualLayout_NoLabel());
            // gridContainer.Add(grid = new Grid_ManualLayout());
            // gridContainer.Add(grid = new Grid_OnlyRepositionOnResize());
            // gridContainer.Add(grid = new Grid_ManualLayout_RemoveOutOfViewportElementFromParent_WithDetach());
            // gridContainer.Add(grid = new Grid_ManualLayout_RemoveOutOfViewportElementFromParent_SetDisabled());

            //== following *should* be faster..

            // Interesting - I'd've thought the following would avoid a relayout and thus be faster, but it's slower
            // than Grid_ManualLayout_RemoveOutOfViewportElementFromParent_WithTransform...
            // gridContainer.Add(grid = new Grid_ManualLayout_RemoveOutOfViewportElementFromParent_SetLabelOpacity());

            // Try this one on PC; see if it feesl better
            // gridContainer.Add(grid = new Grid_ManualLayout_RemoveOutOfViewportElementFromParent_MinimizeSizeSet());


            // BEST SO FAR:  Odd; some of the above *should* be faster.
            gridContainer.Add(grid = new Grid_ManualLayout_RemoveOutOfViewportElementFromParent_WithTransform());




            grid.PopulateWithTestElements();

            // Setup slider
            EventCallback<ChangeEvent<float>> sliderCB;
            sizeSlider.RegisterValueChangedCallback(sliderCB = (val) => grid.SetElementSize(val.newValue));
            sizeSlider.value = (sizeSlider.highValue - sizeSlider.lowValue) / 2;
        }
    }
}
