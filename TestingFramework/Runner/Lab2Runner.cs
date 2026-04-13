using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestingFramework.Attributes;

namespace TestingFramework.Runner
{
    public class Lab2Runner
    {
        public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;

        public async Task RunTests(Assembly assembly)
        {
            var results = new ConcurrentQueue<string>();
            var semaphore = new SemaphoreSlim(MaxDegreeOfParallelism);

            var testTasks = new List<Task>();

            var testClasses = assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<TestClassAttribute>() != null);

            foreach (var testClass in testClasses)
            {
                var instance = Activator.CreateInstance(testClass);

                var beforeMethod = testClass.GetMethods()
                    .FirstOrDefault(m => m.GetCustomAttribute<BeforeAttribute>() != null);

                var afterMethod = testClass.GetMethods()
                    .FirstOrDefault(m => m.GetCustomAttribute<AfterAttribute>() != null);

                var testMethods = testClass.GetMethods()
                    .Where(m => m.GetCustomAttribute<TestMethodAttribute>() != null);

                foreach (var method in testMethods)
                {
                    var testCases = method.GetCustomAttributes<TestCaseAttribute>();

                    if (!testCases.Any())
                    {
                        testTasks.Add(RunTest   (instance, method, beforeMethod, afterMethod, null, results, semaphore));
                    }
                    else
                    {
                        foreach (var testCase in testCases)
                        {
                            testTasks.Add(RunTest(instance, method, beforeMethod, afterMethod, testCase.Parameters, results, semaphore));
                        }
                    }
                }
            }

            await Task.WhenAll(testTasks);
            Console.WriteLine("=== PARALLEL RESULTS ===");
            foreach (var res in results)
                Console.WriteLine(res);

        }

        private async Task RunTest(
            object instance,
            MethodInfo method,
            MethodInfo before,
            MethodInfo after,
            object[] parameters,
            ConcurrentQueue<string> results,
            SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();

            try
            {
                var timeoutAttr = method.GetCustomAttribute<TimeoutAttribute>();
                int timeout = timeoutAttr?.Milliseconds ?? Timeout.Infinite;

                var testTask = Task.Run(() =>
                {
                    before?.Invoke(instance, null);

                    var result = method.Invoke(instance, parameters);

                    if (result is Task task)
                        task.GetAwaiter().GetResult();

                    after?.Invoke(instance, null);
                });

                if (await Task.WhenAny(testTask, Task.Delay(timeout)) == testTask)
                {
                    results.Enqueue($"PASS: {method.Name}");
                }
                else
                {
                    results.Enqueue($"TIMEOUT: {method.Name}");
                }
            }
            catch (Exception ex)
            {
                results.Enqueue($"FAIL: {method.Name} -> {ex.InnerException?.Message ?? ex.Message}");
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
