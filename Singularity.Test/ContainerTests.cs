using Xunit;
using Singularity.Exceptions;

namespace Singularity.Test
{
	public class ContainerTests
	{
		[Fact]
		public void GetInstance_NoInternalDependencies()
		{
			var container = new Container();
			using (var builder = container.StartBuilding())
			{
				builder.Bind<IFoo_0>().To<Foo_0>();
			}
			var value = container.GetInstance<IFoo_0>();
			Assert.Equal(typeof(Foo_0), value.GetType());
		}

		[Fact]
		public void GetInstance_WithInternalDependencies1Deep_PerCallLifetime()
		{
			var container = new Container();
			using (var builder = container.StartBuilding())
			{
				builder.Bind<IFoo_0>().To<Foo_0>();
				builder.Bind<IFoo_1>().To<Foo_1>();
			}
			var value1 = container.GetInstance<IFoo_1>();
			var value2 = container.GetInstance<IFoo_1>();
			Assert.NotNull(value1);
			Assert.NotNull(value2);
			Assert.NotNull(value1.Foo_0);
			Assert.NotNull(value2.Foo_0);
			Assert.Equal(typeof(Foo_1), value1.GetType());
			Assert.Equal(typeof(Foo_1), value2.GetType());
			Assert.NotEqual(value1.Foo_0, value2.Foo_0);
		}

		[Fact]
		public void GetInstance_WithInternalDependencies1Deep_PerContainerLifetime()
		{
			var container = new Container();
			using (var builder = container.StartBuilding())
			{
				builder.Bind<IFoo_0>().To<Foo_0>().SetLifetime(Lifetime.PerContainer);
				builder.Bind<IFoo_1>().To<Foo_1>();
			}
			var value1 = container.GetInstance<IFoo_1>();
			var value2 = container.GetInstance<IFoo_1>();
			Assert.NotNull(value1);
			Assert.NotNull(value2);
			Assert.NotNull(value1.Foo_0);
			Assert.NotNull(value2.Foo_0);
			Assert.Equal(typeof(Foo_1), value1.GetType());
			Assert.Equal(typeof(Foo_1), value2.GetType());
			Assert.Equal(value1.Foo_0, value2.Foo_0);
		}

		[Fact]
		public void GetInstance_WithInternalDependencies2Deep()
		{
			var container = new Container();
			using (var builder = container.StartBuilding())
			{
				builder.Bind<IFoo_0>().To<Foo_0>();
				builder.Bind<IFoo_1>().To<Foo_1>();
				builder.Bind<IFoo_2>().To<Foo_2>();
			}
			var value = container.GetInstance<IFoo_2>();
			Assert.Equal(typeof(Foo_2), value.GetType());
			Assert.NotNull(value.Foo_1);
			Assert.NotNull(value.Foo_1.Foo_0);
		}

		[Fact]
		public void GetInstance_MissingDependency_Throws()
		{
			var container = new Container();
			using (var builder = container.StartBuilding())
			{

			}
			Assert.Throws<DependencyNotFoundException>(() =>
			{
				container.GetInstance<IFoo_0>();
			});
		}

		[Fact]
		public void GetInstance_MissingInternalDependency_Throws()
		{
			var container = new Container();

			Assert.Throws<CannotResolveDependencyException>(() =>
			{
				using (var builder = container.StartBuilding())
				{
					builder.Bind<IFoo_1>().To<Foo_1>();
				}
			});
		}

		[Fact]
		public void GetInstance_WithPerContainerLifetime()
		{
			var container = new Container();

			using (var builder = container.StartBuilding())
			{
				builder.Bind<IFoo_0>().To<Foo_0>().SetLifetime(Lifetime.PerContainer);
			}
			var value1 = container.GetInstance<IFoo_0>();
			var value2 = container.GetInstance<IFoo_0>();
			Assert.NotNull(value1);
			Assert.NotNull(value2);
			Assert.Equal(value1, value2);
		}

		[Fact]
		public void GetInstance_WithPerCallLifetime()
		{
			var container = new Container();

			using (var builder = container.StartBuilding())
			{
				builder.Bind<IFoo_0>().To<Foo_0>();
			}
			var value1 = container.GetInstance<IFoo_0>();
			var value2 = container.GetInstance<IFoo_0>();
			Assert.NotNull(value1);
			Assert.NotNull(value2);
			Assert.NotEqual(value1, value2);
		}
	}

	public interface IFoo_0
	{

	}

	public interface IFoo_1
	{
		IFoo_0 Foo_0 { get; }
	}

	public interface IFoo_2
	{
		IFoo_1 Foo_1 { get; }
	}

	public class Foo_0 : IFoo_0
	{
		//public Foo_0()
		//{

		//}
	}

	public class Foo_1 : IFoo_1
	{
		public IFoo_0 Foo_0 { get; }
		public Foo_1(IFoo_0 foo_0)
		{
			Foo_0 = foo_0;
		}
	}

	public class Foo_2 : IFoo_2
	{
		public IFoo_1 Foo_1 { get; }
		public Foo_2(IFoo_1 foo_1)
		{
			Foo_1 = foo_1;
		}
	}
}
