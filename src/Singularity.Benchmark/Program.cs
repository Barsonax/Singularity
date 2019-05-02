using BenchmarkDotNet.Running;
using System;
using System.Linq.Expressions;
using Singularity.FastExpressionCompiler;
using Singularity.TestClasses.Benchmark;

namespace Singularity.Benchmark
{
    class Program 
    {
        static void Main()
        {
            var constant = Expression.Constant(new Program());

            var del1 = Expression.Lambda<Func<Program>>(constant).CompileFast<Func<Program>>();
            //var del = Expression.Lambda<Func<Program>>(constant).TryCompileWithoutClosure<Func<Program>>();

            var del = Expression.Lambda<Func<Program>>(constant).TryCompileWithPreCreatedClosure<Func<Program>>(ExpressionCompiler.Closure.Create(constant.Value), constant);

            var value = del.Invoke();
            Console.WriteLine(value);
            //AdvancedSingularityContainerBenchmark _benchmark = new AdvancedSingularityContainerBenchmark();

            //while (true)
            //{
            //    _benchmark.AspNetCore();
            //}
            //BenchmarkRunner.Run<ImmutableDictionaryBenchmarks>();
            //BenchmarkRunner.Run<SimpleContainerBenchmark>();
            //BenchmarkRunner.Run()
            //BenchmarkSwitcher.FromTypes(new[] {typeof(SimpleContainerBenchmark), typeof(AdvancedContainerBenchmark)}).RunAllJoined();

            //BenchmarkRunner.Run<AdvancedContainerBenchmark>();
            //BenchmarkRunner.Run<DisposeListBenchmarks>();
            Console.ReadKey();
        }
    }
}
