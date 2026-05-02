using System.Diagnostics;
using System.Reflection;
using TestingFramework.Attributes;
using TestingFramework.Runner;

Assembly assembly = Assembly.LoadFrom("C:/Users/user/Desktop/spp_new/spp_new/TestProject_Tests/bin/Debug/net8.0/TestProject_Tests.dll");




Console.WriteLine("======================================\n==========LABA 4======================\n======================================");
using (Lab4Runner r4 = new Lab4Runner(minThreads: 2, maxThreads: 5, idleTimeoutMs: 3000))
{
    // Subscribe to events
    r4.ThreadPool.StateChanged += (sender, msg) => Console.WriteLine($"[Event: StateChanged] {msg}");
    r4.ThreadPool.ErrorOccurred += (sender, msg) => Console.WriteLine($"[Event: ErrorOccurred] {msg}");

    Assembly assembly4 = Assembly.LoadFrom("C:/Users/user/Desktop/spp_new/spp_new/TestProject_Tests/bin/Debug/net8.0/TestProject_Tests.dll");
    var allTestClasses4 = assembly4.GetTypes().Where(t => t.GetCustomAttribute<TestClassAttribute>() != null).ToList();

    Console.WriteLine("Running tests with Category 'Dynamic' or 'Math':");
    Func<MethodInfo, bool> filter = m => 
    {
        var category = m.GetCustomAttribute<CategoryAttribute>();
        return category != null && (category.Name == "Dynamic" || category.Name == "Math");
    };

    var watch = Stopwatch.StartNew();
    await r4.RunTests(allTestClasses4, filter);
    watch.Stop();
    Console.WriteLine($"Время выполнения: {watch.ElapsedMilliseconds} мс");
}