using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TestingFramework.Attributes;

namespace TestingFramework.Runner
{
    public class Runner
    {
        public void RunTests(Assembly assembly)
        {
            var testClasses = assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<TestClassAttribute>() != null);

            foreach (var testClass in testClasses)
            {
                Console.WriteLine($"Running tests in {testClass.Name}");

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
                        RunTest(instance, method, beforeMethod, afterMethod, null);
                    }
                    else
                    {
                        foreach (var testCase in testCases)
                        {
                            RunTest(instance, method, beforeMethod, afterMethod, testCase.Parameters);
                        }
                    }
                }
            }
        }

        private void RunTest(object instance, MethodInfo method, MethodInfo before, MethodInfo after, object[] parameters)
        {
            try
            {
                before?.Invoke(instance, null);

                var result = method.Invoke(instance, parameters);

                if (result is Task task)
                    task.GetAwaiter().GetResult();

                Console.WriteLine($"PASS: {method.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FAIL: {method.Name} -> {ex.InnerException?.Message ?? ex.Message}");
            }
            finally
            {
                after?.Invoke(instance, null);
            }
        }
    }
}