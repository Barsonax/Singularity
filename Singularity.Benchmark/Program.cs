using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;

namespace Singularity.Benchmark
{
    class Program
    {
        static void Main()
        {
			BenchmarkRunner.Run<InjectorBenchmark>();
			Console.ReadKey();
        }
    }

	public class InjectorBenchmark
	{
		private readonly Container _singulairtyContainer;

		public InjectorBenchmark()
		{
		    var config = new BindingConfig();
		    config.Bind<IInjectorTest>().To(() => new InjectorTest()).SetLifetime(Lifetime.PerContainer);
            _singulairtyContainer = new Container(config);
		}

        [Benchmark]
		public IInjectorTest Singularity()
		{
			var value = _singulairtyContainer.GetInstance<IInjectorTest>();
			return value;
		}
	}

	public class InjectorTest : IInjectorTest
	{

	}

	public interface IInjectorTest
	{

	}
}
