using System;
using System.Linq.Expressions;
using BenchmarkDotNet.Running;
using Singularity.FastExpressionCompiler;

namespace Singularity.Benchmark
{
    class Program 
    {
        static void Main()
        {
            //var _benchmark = new SimpleContainerBenchmark();
            //_benchmark.Setup();
            //_benchmark.Complex();
            //BenchmarkRunner.Run<ImmutableDictionaryBenchmarks>();
            //BenchmarkRunner.Run<SimpleContainerBenchmark>();
            BenchmarkRunner.Run<AdvancedContainerBenchmark>();
            //BenchmarkRunner.Run()
            //BenchmarkSwitcher.FromTypes(new[] {typeof(SimpleContainerBenchmark), typeof(AdvancedContainerBenchmark)}).RunAllJoined();

            //BenchmarkRunner.Run<DisposeListBenchmarks>();
            Console.ReadKey();
        }
    }
}
