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
        [Category("Math")]
        [Priority(1)]
        [Author("Admin")]
        public void MultiplyFailTest()
        {
            var result = calculator.Multiply(3, 4);

            Assert.Less(result, 5);
        }

        [TestMethod]
        [Category("Dynamic")]
        [Priority(2)]
        [DynamicTestCase(nameof(GetDivideTestCases))]
        public void DivideDynamicTest(int a, int b, int expected)
        {
            var result = calculator.Divide(a, b);
            Assert.AreEqual(expected, result);
        }

        public static IEnumerable<object[]> GetDivideTestCases()
        {
            yield return new object[] { 10, 2, 5 };
            yield return new object[] { 9, 3, 3 };
            yield return new object[] { 20, 4, 5 };
            yield return new object[] { -10, 2, -5 };
        }
    }
}