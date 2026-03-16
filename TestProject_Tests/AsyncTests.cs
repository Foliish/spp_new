using TestProject;
using TestingFramework.Attributes;
using TestingFramework.Assertions;

namespace TestedProject.Tests
{
    [TestClass]
    public class AsyncTests
    {
        private AsyncService service;

        [Before]
        public void Setup()
        {
            service = new AsyncService();
        }

        [TestMethod]
        public async Task GetNumberAsyncTest()
        {
            var result = await service.GetNumberAsync();

            Assert.AreEqual(42, result);
            Assert.Greater(result, 10);
        }

        [TestMethod]
        public async Task GetTextAsyncTest()
        {
            var result = await service.GetTextAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual("hello", result);
        }
        [TestMethod]
        public async Task GetNumberAsyncFailTest()
        {
            var result = await service.GetNumberAsync();

            Assert.AreEqual(100, result);
        }
    }
}