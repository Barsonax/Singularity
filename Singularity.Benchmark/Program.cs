using BenchmarkDotNet.Running;
using System;

namespace Singularity.Benchmark
{
    class Program
    {
        static void Main()
        {
            //BenchmarkRunner.Run<ImmutableDictionaryBenchmarks>();
            BenchmarkRunner.Run<SimpleContainerBenchmark>();
            //BenchmarkRunner.Run<DisposeListBenchmarks>();
            Console.ReadKey();
        }
    }
}
