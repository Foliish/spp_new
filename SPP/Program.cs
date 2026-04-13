using System.Reflection;
using TestingFramework.Attributes;
using TestingFramework.Runner;

Assembly assembly = Assembly.LoadFrom("C:/Users/user/Desktop/spp/spp_new/TestProject_Tests/bin/Debug/net8.0/TestProject_Tests.dll");

Runner r = new Runner();
r.RunTests(assembly);

Console.WriteLine("======================================\n==========LABA 2======================\n======================================");
Lab2Runner r2 = new Lab2Runner();
r2.MaxDegreeOfParallelism = 3;
Assembly assembly2 = Assembly.LoadFrom("C:/Users/user/Desktop/spp/spp_new/TestProject_Tests/bin/Debug/net8.0/TestProject_Tests.dll");
await r2.RunTests(assembly2);

Console.WriteLine("======================================\n==========LABA 3======================\n======================================");

Lab3Runner r3 = new Lab3Runner();
r3.MaxDegreeOfParallelism = 5;
Assembly assembly3 = Assembly.LoadFrom("C:/Users/user/Desktop/spp/spp_new/TestProject_Tests/bin/Debug/net8.0/TestProject_Tests.dll");
var testClasses = assembly3.GetTypes()
                .Where(t => t.GetCustomAttribute<TestClassAttribute>() != null);
await r3.RunTests(testClasses);

await Simulation();
static async Task Simulation()
{
    Lab3Runner runner = new Lab3Runner(minThreads: 2, maxThreads: 8, idleTimeoutMs: 4000);

    Assembly assembly = Assembly.LoadFrom("C:/Users/user/Desktop/spp/spp_new/TestProject_Tests/bin/Debug/net8.0/TestProject_Tests.dll");
    var allTestClasses = assembly.GetTypes()
        .Where(t => t.GetCustomAttribute<TestClassAttribute>() != null)
        .ToList();

    Console.WriteLine(">>> STARTING LOAD SIMULATION <<<\n");

    Console.WriteLine("\n[SCENARIO 1] один");
    var singleBatch = allTestClasses.Take(1);
    await runner.RunTests(singleBatch);

    Console.WriteLine("\n[SCENARIO 2] Ждем");
    await Task.Delay(2000);

    Console.WriteLine("\n[SCENARIO 3] Горит дедлайн");
    var peakBatch = Enumerable.Repeat(allTestClasses, 5).SelectMany(x => x);
    await runner.RunTests(peakBatch);

    Console.WriteLine("\n[SCENARIO 4] Ждем");
    await Task.Delay(6000);

    Console.WriteLine("\n[SCENARIO 5] Ещё чуть");
    await runner.RunTests(allTestClasses.Take(2));

    Console.WriteLine("\n>>> SIMULATION FINISHED <<<");
    runner.Dispose();
}