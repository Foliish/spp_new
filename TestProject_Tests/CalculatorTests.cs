using TestProject;
using TestingFramework.Attributes;
using TestingFramework.Assertions;

namespace TestProject.Tests
{
    [TestClass]
    public class CalculatorTests
    {
        private Calculator calculator;

        [Before]
        public void Setup()
        {
            calculator = new TestProject.Calculator();
        }

        [After]
        public void Cleanup()
        {
            calculator = null;
        }

        [TestMethod]
        public void AddTest()
        {
            var result = calculator.Add(2, 3);
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        [TestCase(4)]
        [TestCase(5)]
        public void SquareTest(int value)
        {
            var result = calculator.Square(value);
            Assert.Greater(result, value);
        }

        [TestMethod]
        public void MultiplyTest()
        {
            var result = calculator.Multiply(3, 4);
            Assert.AreNotEqual(5, result);
            Assert.Greater(result, 10);
        }

        [TestMethod]
        public void MultiplyFailTest()
        {
            var result = calculator.Multiply(3, 4);

            Assert.Less(result, 5);
        }
    }
}