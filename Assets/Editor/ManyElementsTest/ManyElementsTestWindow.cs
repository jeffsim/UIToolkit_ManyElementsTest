using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ManyElementsTest
{
    public class ManyElementsTestWindow : EditorWindow
    {
        [MenuItem("Window/ManyElementsTest %T")]
        static void Init() => GetWindow<ManyElementsTestWindow>();

        public static int NumElementsToAddToGrid = 20000;

        public static Texture2D TestTexture;

        void OnEnable()
        {
            rootVisualElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/ManyElementsTest/resources/style.uss"));
            var xmlAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ManyElementsTest/resources/TestWindow.uxml");
            xmlAsset.CloneTree(rootVisualElement);

            TestTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/ManyElementsTest/resources/testImage.png");

            var gridContainer = rootVisualElement.Q("testGridContainer");
            var sizeSlider = rootVisualElement.Q<Slider>("sizeSlider");

            // Hook up run tests button
            rootVisualElement.Q<Button>().clicked += () => EditorCoroutines.Execute(TestRunner.RunTests(sizeSlider, gridContainer));

            var prepopulate = false;
            if (prepopulate)
                EditorCoroutines.Execute(TestRunner.RunTestWithGridType(sizeSlider, gridContainer, new Grid_ListWithWrapping(), true));
        }
    }
}
