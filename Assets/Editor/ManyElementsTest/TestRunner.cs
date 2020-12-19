using System;
using System.Collections;
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
            NumTimesRun--; // remove prepass
            Debug.Log("Average results for " + TestName + " over " + NumTimesRun + " runs:");
            Debug.Log("  Populate time:" + PopulateTime / NumTimesRun + " sec");
            Debug.Log("  Scroll time:" + ScrollTime / NumTimesRun + " sec");
            Debug.Log("  Resize time:" + ResizeTime / NumTimesRun + " sec");
        }
    }

    public static class TestRunner
    {
        static bool ignoreTestResults;

        static internal IEnumerator RunTests(Slider sizeSlider, VisualElement gridContainer)
        {
            var results_ManualLayout = new TestResult() { TestName = "ManualLayout" };
            var results_ListWithWrapping = new TestResult() { TestName = "ListWithWrapping" };

            // Run twice; all once to cache (results = ignored) and then once for the 'real' run
            for (int i = 0; i < 3; i++)
            {
                ignoreTestResults = i == 0;
                if (ignoreTestResults)
                    Debug.Log("Running prepass (results ignored)");
                else
                    Debug.Log("Running pass #" + i);
                yield return RunTestWithGridType(sizeSlider, gridContainer, new Grid_ManualLayout(), false, results_ManualLayout);
                yield return RunTestWithGridType(sizeSlider, gridContainer, new Grid_ListWithWrapping(), false, results_ListWithWrapping);
            }

            results_ManualLayout.OutputResults();
            results_ListWithWrapping.OutputResults();
        }

        public static IEnumerator RunTestWithGridType(Slider sizeSlider, VisualElement gridContainer, BaseGrid grid, bool justDoSetup = false, TestResult testResult = null)
        {
            gridContainer.Add(grid);

            // Setup slider
            EventCallback<ChangeEvent<float>> sliderCB;
            sizeSlider.RegisterValueChangedCallback(sliderCB = (val) => grid.SetElementSize(val.newValue));
            sizeSlider.value = (sizeSlider.highValue - sizeSlider.lowValue) / 2;

            if (!ignoreTestResults)
                Debug.Log("----- running tests for " + grid);
            yield return null;

            TestPopulateElements(grid, testResult);

            if (justDoSetup)
                yield break;

            yield return TestScrollSpeed(sizeSlider, grid, testResult);
            yield return TestResizeSpeed(sizeSlider, testResult);

            testResult.NumTimesRun++;

            // cleanup
            sizeSlider.UnregisterValueChangedCallback(sliderCB);
            gridContainer.Remove(grid);
        }

        static void TestPopulateElements(BaseGrid grid, TestResult testResult)
        {
            startTestTimer();
            grid.PopulateWithTestElements();
            if (!ignoreTestResults)
                testResult.PopulateTime += endTestTimer();
        }

        static internal IEnumerator TestScrollSpeed(Slider sizeSlider, BaseGrid grid, TestResult testResult)
        {
            sizeSlider.value = 25;
            yield return null;

            startTestTimer();
            var scrollSpeed = new Vector2(0, 20);
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
            if (!ignoreTestResults)
                testResult.ScrollTime += endTestTimer();
        }

        static internal IEnumerator TestResizeSpeed(Slider sizeSlider, TestResult testResult)
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
            if (!ignoreTestResults)
                testResult.ResizeTime += endTestTimer();
        }

        static long startTime;
        static private void startTestTimer() => startTime = DateTime.Now.Ticks;
        static private float endTestTimer() => (DateTime.Now.Ticks - startTime) / 10000000f;
    }
}