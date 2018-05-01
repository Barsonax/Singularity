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
			_singulairtyContainer = new Container();
			using (var builder = _singulairtyContainer.StartBuilding())
			{
				builder.Bind<IInjectorTest>().To(() => new InjectorTest()).SetLifetime(Lifetime.PerContainer);
			}
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
