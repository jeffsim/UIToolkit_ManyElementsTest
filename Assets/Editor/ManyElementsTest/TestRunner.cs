using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace ManyElementsTest
{
    public static class TestRunner
    {
        static internal IEnumerator RunTests(Slider sizeSlider, VisualElement gridContainer)
        {
            yield return RunTestWithGridType(sizeSlider, gridContainer, new Grid_ListWithWrapping());
            yield return RunTestWithGridType(sizeSlider, gridContainer, new Grid_ManualLayout());
        }

        public static IEnumerator RunTestWithGridType(Slider sizeSlider, VisualElement gridContainer, BaseGrid grid, bool justDoSetup = false)
        {
            gridContainer.Add(grid);

            // Setup slider
            EventCallback<ChangeEvent<float>> sliderCB;
            sizeSlider.RegisterValueChangedCallback(sliderCB = (val) => grid.SetElementSize(val.newValue));
            sizeSlider.value = (sizeSlider.highValue - sizeSlider.lowValue) / 2;

            Debug.Log("----- running tests for " + grid);
            yield return null;

            TestPopulateElements(grid);

            if (justDoSetup)
                yield break;

            yield return TestScrollSpeed(sizeSlider, grid);
            yield return TestResizeSpeed(sizeSlider);

            // cleanup
            sizeSlider.UnregisterValueChangedCallback(sliderCB);
            gridContainer.Remove(grid);
        }

        static void TestPopulateElements(BaseGrid grid)
        {
            startTestTimer();
            grid.PopulateWithTestElements();
            endTestTimer("populating");
        }

        static internal IEnumerator TestScrollSpeed(Slider sizeSlider, BaseGrid grid)
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
            endTestTimer("Scrolling");
        }

        static internal IEnumerator TestResizeSpeed(Slider sizeSlider)
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
            endTestTimer("Resize");
        }

        static long startTime;

        static private void endTestTimer(string timerName)
        {
            Debug.Log(timerName + ": " + (DateTime.Now.Ticks - startTime) / 10000000f + " sec");
        }

        static private void startTestTimer()
        {
            startTime = DateTime.Now.Ticks;
        }
    }
}