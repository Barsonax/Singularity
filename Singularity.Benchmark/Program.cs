using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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
