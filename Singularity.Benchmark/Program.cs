using BenchmarkDotNet.Running;
using System;

namespace Singularity.Benchmark
{
    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<SingularityContainerBenchmark>();
            Console.ReadKey();
        }
    }
}
