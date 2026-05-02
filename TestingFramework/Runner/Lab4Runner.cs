using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TestingFramework.Attributes;

namespace TestingFramework.Runner
{
    public class Lab4Runner : IDisposable
    {
        private readonly CustomThreadPool _threadPool;
        public CustomThreadPool ThreadPool => _threadPool;
        public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;

        public Lab4Runner(int minThreads = 2, int maxThreads = 5, int idleTimeoutMs = 3000)
        {
            _threadPool = new CustomThreadPool(minThreads, maxThreads, idleTimeoutMs);
        }

        public async Task RunTests(IEnumerable<Type> testClasses, Func<MethodInfo, bool> filter = null)
        {
            var results = new ConcurrentQueue<string>();
            var runTasks = new List<Task>();

            foreach (var testClass in testClasses)
            {
                var instance = Activator.CreateInstance(testClass);
                var beforeMethod = testClass.GetMethods().FirstOrDefault(m => m.GetCustomAttribute<BeforeAttribute>() != null);
                var afterMethod = testClass.GetMethods().FirstOrDefault(m => m.GetCustomAttribute<AfterAttribute>() != null);
                var testMethods = testClass.GetMethods().Where(m => m.GetCustomAttribute<TestMethodAttribute>() != null);

                foreach (var method in testMethods)
                {
                    if (filter != null && !filter(method))
                        continue;

                    var testCases = method.GetCustomAttributes<TestCaseAttribute>();
                    var dynamicTestCases = method.GetCustomAttributes<DynamicTestCaseAttribute>();
                    
                    bool hasParameters = testCases.Any() || dynamicTestCases.Any();

                    if (!hasParameters)
                    {
                        runTasks.Add(EnqueueTest(instance, method, beforeMethod, afterMethod, null, results));
                    }
                    else
                    {
                        foreach (var testCase in testCases)
                        {
                            runTasks.Add(EnqueueTest(instance, method, beforeMethod, afterMethod, testCase.Parameters, results));
                        }

                        foreach (var dynAttr in dynamicTestCases)
                        {
                            var sourceMethod = testClass.GetMethod(dynAttr.SourceMethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                            if (sourceMethod != null)
                            {
                                var parametersList = (IEnumerable<object[]>)sourceMethod.Invoke(sourceMethod.IsStatic ? null : instance, null);
                                foreach (var p in parametersList)
                                {
                                    runTasks.Add(EnqueueTest(instance, method, beforeMethod, afterMethod, p, results));
                                }
                            }
                        }
                    }
                }
            }

            await Task.WhenAll(runTasks);

            Console.WriteLine($"--- Wave Completed. Results processed: {results.Count} ---");
            foreach (var res in results)
            {
                Console.WriteLine(res);
            }
        }

        private Task EnqueueTest(object inst, MethodInfo m, MethodInfo b, MethodInfo a, object[] p, ConcurrentQueue<string> res)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var timeout = m.GetCustomAttribute<TimeoutAttribute>()?.Milliseconds ?? 50000;

            _threadPool.Enqueue(() =>
            {
                try
                {
                    var testTask = Task.Run(() => {
                        b?.Invoke(inst, null);
                        var r = m.Invoke(inst, p);
                        if (r is Task t) t.GetAwaiter().GetResult();
                        a?.Invoke(inst, null);
                    });

                    if (testTask.Wait(timeout)) res.Enqueue($"PASS: {m.Name}");
                    else res.Enqueue($"TIMEOUT: {m.Name}");
                }
                catch (Exception ex) { res.Enqueue($"FAIL: {m.Name} -> {ex.InnerException?.Message ?? ex.Message}"); }
                finally { tcs.SetResult(true); }
            });
            return tcs.Task;
        }

        public void Dispose() => _threadPool.Dispose();
    }
}
