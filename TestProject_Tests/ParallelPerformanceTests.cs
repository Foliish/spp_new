using System.Threading.Tasks;
using TestingFramework.Attributes;

namespace TestedProject.Tests
{
    [TestClass]
    public class ParallelPerformanceTests
    {
        [TestMethod]
        public async Task Test1()
        {
            await Task.Delay(500);
        }

        [TestMethod]
        public async Task Test2()
        {
            await Task.Delay(500);
        }

        [TestMethod]
        public async Task Test3()
        {
            await Task.Delay(500);
        }

        [TestMethod]
        public async Task Test4()
        {
            await Task.Delay(500);
        }

        [TestMethod]
        [Timeout(200)]
        public async Task TimeoutTest()
        {
            await Task.Delay(1000);
        }
    }
}
