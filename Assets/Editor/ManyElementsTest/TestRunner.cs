using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ManyElementsTest
{
    public class TestResult
    {
        public string TestName;
        public float PopulateTime;
        public float ScrollTime;
        public float ResizeTime;
        public int NumTimesRun = 0;

        internal void OutputResults()
        {
            Debug.Log("Average results for " + TestName + " over " + NumTimesRun + " run(s):");
            Debug.Log("  Populate time:" + PopulateTime / NumTimesRun + " sec");
            Debug.Log("  Scroll time:" + ScrollTime / NumTimesRun + " sec");
            Debug.Log("  Resize time:" + ResizeTime / NumTimesRun + " sec");
        }
    }

    public class Test
    {
        public Func<BaseGrid> CreateFunc;
        public TestResult Results;
        public string Name;

        public Test(string testName, Func<BaseGrid> createFunc)
        {
            Name = testName;
            CreateFunc = createFunc;
            Results = new TestResult() { TestName = testName };
        }
    }

    public static class TestRunner
    {
        static internal IEnumerator RunTests(Slider sizeSlider, VisualElement gridContainer)
        {
            var testsToRun = new List<Test>();
            testsToRun.Add(new Test("ManualLayout", () => new Grid_ManualLayout()));
            testsToRun.Add(new Test("ManualLayout_WithTransform", () => new Grid_ManualLayout_WithTransform()));
            testsToRun.Add(new Test("Grid_ManualLayout_NoDisplayChanges", () => new Grid_ManualLayout_NoDisplayChanges()));
            testsToRun.Add(new Test("Grid_ManualLayout_NoLabel", () => new Grid_ManualLayout_NoLabel()));

            // Following are too slow
            //testsToRun.Add(new Test("ListWithWrapping", () => new Grid_ListWithWrapping()));

            for (int i = 0; i < ManyElementsTestWindow.NumTestPassesToRun; i++)
            {
                Debug.Log("Running pass #" + i);
                foreach (var test in testsToRun)
                    yield return RunTest(sizeSlider, gridContainer, test, false);
            }

            Debug.Log("== TESTS COMPLETE ==");
            Debug.Log("  Viewport size: " + gridContainer.parent.parent.parent.layout.size);
            testsToRun.ForEach(test => test.Results.OutputResults());
        }

        public static IEnumerator RunTest(Slider sizeSlider, VisualElement gridContainer, Test test, bool justDoSetup = false)
        {
            var grid = test.CreateFunc();
            gridContainer.Add(grid);

            // Setup slider
            EventCallback<ChangeEvent<float>> sliderCB;
            sizeSlider.RegisterValueChangedCallback(sliderCB = (val) => grid.SetElementSize(val.newValue));
            sizeSlider.value = (sizeSlider.highValue - sizeSlider.lowValue) / 2;

            if (!justDoSetup)
                Debug.Log("----- running tests for " + test.Name);
            yield return null;

            TestPopulateElements(grid, test, justDoSetup);

            if (justDoSetup)
                yield break;

            yield return TestScrollSpeed(sizeSlider, grid, test);
            yield return TestResizeSpeed(sizeSlider, test);

            test.Results.NumTimesRun++;

            // cleanup
            sizeSlider.UnregisterValueChangedCallback(sliderCB);
            gridContainer.Remove(grid);
        }

        static void TestPopulateElements(BaseGrid grid, Test test, bool justDoSetup)
        {
            startTestTimer();
            grid.PopulateWithTestElements();
            if (!justDoSetup)
                test.Results.PopulateTime += endTestTimer();
        }

        static internal IEnumerator TestScrollSpeed(Slider sizeSlider, BaseGrid grid, Test test)
        {
            sizeSlider.value = sizeSlider.lowValue;
            yield return null;

            startTestTimer();
            var scrollSpeed = new Vector2(0, 150);
            int steps = 0, maxSteps = 40;
            var scrollView = grid.ScrollView;
            var elementContainer = grid.ScrollView.contentContainer;
            while (scrollView.scrollOffset.y < elementContainer.worldBound.height - scrollView.worldBound.height && steps < maxSteps)
            {
                scrollView.scrollOffset += scrollSpeed;
                steps++;
                yield return null;
            }
            steps = 0;
            while (scrollView.scrollOffset.y > 0 && steps < maxSteps)
            {
                scrollView.scrollOffset -= scrollSpeed;
                steps++;
                yield return null;
            }
            test.Results.ScrollTime += endTestTimer();
        }

        static internal IEnumerator TestResizeSpeed(Slider sizeSlider, Test test)
        {
            sizeSlider.value = sizeSlider.lowValue;
            yield return null;

            startTestTimer();

            var resizeSpeed = 10;
            for (int i = 0; i < 3; i++)
            {
                while (sizeSlider.value < sizeSlider.highValue / 2)
                {
                    sizeSlider.value += resizeSpeed;
                    yield return null;
                }
                while (sizeSlider.value > sizeSlider.lowValue)
                {
                    sizeSlider.value -= resizeSpeed;
                    yield return null;
                }
            }
            test.Results.ResizeTime += endTestTimer();
        }

        static long startTime;
        static private void startTestTimer() => startTime = DateTime.Now.Ticks;
        static private float endTestTimer() => (DateTime.Now.Ticks - startTime) / 10000000f;
    }
}