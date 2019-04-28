using BenchmarkDotNet.Running;
using System;
using Singularity.TestClasses.Benchmark;

namespace Singularity.Benchmark
{
    class Program
    {
        static void Main()
        {
            AdvancedSingularityContainerBenchmark _benchmark = new AdvancedSingularityContainerBenchmark();

            while (true)
            {
                _benchmark.Register();
            }
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
