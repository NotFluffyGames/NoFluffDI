using System.Collections.Generic;
using NUnit.Framework;

namespace NotFluffy.NoFluffDI.Editor.DebuggingWindow
{
    class TreeModelTests
    {
        [Test]
        public static void TestTreeModelCanAddElements()
        {
            var root = new TreeElement {Name = "Root", Depth = -1};
            var listOfElements = new List<TreeElement>();
            listOfElements.Add(root);

            var model = new TreeModel<TreeElement>(listOfElements);
            model.AddElement(new TreeElement {Name = "Element"}, root, 0);
            model.AddElement(new TreeElement {Name = "Element " + root.Children.Count}, root, 0);
            model.AddElement(new TreeElement {Name = "Element " + root.Children.Count}, root, 0);
            model.AddElement(new TreeElement {Name = "Sub Element"}, root.Children[1], 0);

            // Assert order is correct
            string[] namesInCorrectOrder = {"Root", "Element 2", "Element 1", "Sub Element", "Element"};
            Assert.AreEqual(namesInCorrectOrder.Length, listOfElements.Count, "Result count does not match");
            for (int i = 0; i < namesInCorrectOrder.Length; ++i)
            {
                Assert.AreEqual(namesInCorrectOrder[i], listOfElements[i].Name);
            }

            // Assert depths are valid
            TreeElementUtility.ValidateDepthValues(listOfElements);
        }

        [Test]
        public static void TestTreeModelCanRemoveElements()
        {
            var root = new TreeElement {Name = "Root", Depth = -1};
            var listOfElements = new List<TreeElement>();
            listOfElements.Add(root);

            var model = new TreeModel<TreeElement>(listOfElements);
            model.AddElement(new TreeElement {Name = "Element"}, root, 0);
            model.AddElement(new TreeElement {Name = "Element " + root.Children.Count}, root, 0);
            model.AddElement(new TreeElement {Name = "Element " + root.Children.Count}, root, 0);
            model.AddElement(new TreeElement {Name = "Sub Element"}, root.Children[1], 0);

            model.RemoveElements(new[] {root.Children[1].Children[0], root.Children[1]});

            // Assert order is correct
            string[] namesInCorrectOrder = {"Root", "Element 2", "Element"};
            Assert.AreEqual(namesInCorrectOrder.Length, listOfElements.Count, "Result count does not match");
            for (int i = 0; i < namesInCorrectOrder.Length; ++i)
            {
                Assert.AreEqual(namesInCorrectOrder[i], listOfElements[i].Name);
            }

            // Assert depths are valid
            TreeElementUtility.ValidateDepthValues(listOfElements);
        }
    }
}