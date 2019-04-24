using BenchmarkDotNet.Running;
using System;

namespace Singularity.Benchmark
{
    class Program
    {
        static void Main()
        {
            //BenchmarkRunner.Run<ImmutableDictionaryBenchmarks>();
            //BenchmarkRunner.Run<SimpleContainerBenchmark>();
            //BenchmarkRunner.Run()
            //BenchmarkSwitcher.FromTypes(new[] {typeof(SimpleContainerBenchmark), typeof(AdvancedContainerBenchmark)}).RunAllJoined();

            BenchmarkRunner.Run<AdvancedContainerBenchmark>();
            //BenchmarkRunner.Run<DisposeListBenchmarks>();
            Console.ReadKey();
        }
    }
}
