using Singularity;
using System;
using System.Diagnostics;

namespace ExpressionTrees
{
	class Program
	{
		static void Main(string[] args)
		{
			var container = new Container();
			using (var builder = container.StartBuilding())
			{
				builder.Bind<Foo_0>().To<Foo_0>().SetLifetime(Lifetime.PerContainer);
				builder.Bind<Foo_1>().To<Foo_1>();
				builder.Bind<Foo_2>().To<Foo_2>();
			}

			var value = container.GetInstance<Foo_0>();

			for (int j = 0; j < 10; j++)
			{
				var itterations = 10000000;
				var list = new TestClass[itterations];
				for (int i = 0; i < itterations; i++)
				{
					list[i] = new TestClass();
				}
				GC.Collect();
				var watch = Stopwatch.StartNew();

				foreach (var instance in list)
				{
					container.Inject(instance);
				}

				Console.WriteLine(watch.ElapsedMilliseconds);
			}
			Console.Read();
		}
	}

	public class TestClass
	{
		[Inject]
		public void Init(Foo_1 foo_2)
		{

		}
	}

	public class Foo_0
	{
		public Foo_0()
		{

		}
	}

	public class Foo_1
	{
		public Foo_0 Foo_0;
		public Foo_1(Foo_0 foo_0)
		{
			Foo_0 = foo_0;
		}
	}

	public class Foo_2
	{
		public Foo_1 Foo_1;
		public Foo_2(Foo_1 foo_1)
		{
			Foo_1 = foo_1;
		}
	}
}