using Singularity.Bindings;
using Singularity.Test.TestClasses;
using Xunit;

namespace Singularity.Test
{
	public class ContainerDecoratorTests
	{
		[Fact]
		public void GetInstance_Decorate_Simple()
		{
			var config = new BindingConfig();
			config.Decorate<IComponent>().With<Decorator1>();
			config.For<IComponent>().Inject<Component>();

			var container = new Container(config);

			var value = container.GetInstance<IComponent>();

			Assert.NotNull(value);
			Assert.Equal(typeof(Decorator1), value.GetType());
			var decorator1 = (Decorator1)value;
			Assert.Equal(typeof(Component), decorator1.Component.GetType());
		}

		[Fact]
		public void GetInstance_Decorate_NestedContainer()
		{
			var config = new BindingConfig();
			config.Decorate<IComponent>().With<Decorator1>();
			config.For<IComponent>().Inject<Component>();

			using (var container = new Container(config))
			{
				var value = container.GetInstance<IComponent>();
				Assert.NotNull(value);
				Assert.Equal(typeof(Decorator1), value.GetType());
				var decorator1 = (Decorator1)value;
				Assert.Equal(typeof(Component), decorator1.Component.GetType());

				var nestedConfig = new BindingConfig();
				nestedConfig.Decorate<IComponent>().With<Decorator2>();
				using (var nestedContainer = container.GetNestedContainer(nestedConfig))
				{
					var nestedValue = nestedContainer.GetInstance<IComponent>();
					Assert.NotNull(nestedValue);
					Assert.Equal(typeof(Decorator2), nestedValue.GetType());
					var nestedDecorator2 = (Decorator2)nestedValue;
					Assert.Equal(typeof(Decorator1), nestedDecorator2.Component.GetType());
					var nestedDecorator1 = (Decorator1) nestedDecorator2.Component;
					
					Assert.Equal(typeof(Component), nestedDecorator1.Component.GetType());
				}
			}
		}

		[Fact]
		public void GetInstance_Decorate_Complex1()
		{
			var config = new BindingConfig();

			config.Decorate<IComponent>().With<Decorator1>();
			config.Decorate<IComponent>().With<Decorator2>();

			config.For<IComponent>().Inject<Component>();

			var container = new Container(config);

			var value = container.GetInstance<IComponent>();

			Assert.NotNull(value);
			Assert.Equal(typeof(Decorator2), value.GetType());
			var decorator2 = (Decorator2)value;

			Assert.Equal(typeof(Decorator1), decorator2.Component.GetType());
			var decorator1 = (Decorator1)decorator2.Component;

			Assert.Equal(typeof(Component), decorator1.Component.GetType());
		}

		[Fact]
		public void GetInstance_Decorate_Complex2()
		{
			var config = new BindingConfig();

			config.Decorate<ITestService11>().With<TestService11_Decorator1>();

			config.For<ITestService10>().Inject<TestService10>();
			config.For<ITestService11>().Inject<TestService11>();

			var container = new Container(config);

			var value = container.GetInstance<ITestService11>();

			Assert.NotNull(value);
			Assert.Equal(typeof(TestService11_Decorator1), value.GetType());
			var decorator1 = (TestService11_Decorator1)value;

			Assert.NotNull(decorator1.TestService11);
			Assert.Equal(typeof(TestService11), decorator1.TestService11.GetType());
			var testService11 = (TestService11)decorator1.TestService11;

			Assert.NotNull(testService11.TestService10);
			Assert.Equal(typeof(TestService10), testService11.TestService10.GetType());
		}

		[Fact]
		public void GetInstance_Decorate_Complex3()
		{
			var config = new BindingConfig();

			config.Decorate<ITestService11>().With<TestService11_Decorator1>();
			config.Decorate<ITestService11>().With<TestService11_Decorator2>();

			config.For<ITestService10>().Inject<TestService10>();
			config.For<ITestService11>().Inject<TestService11>();

			var container = new Container(config);

			var value = container.GetInstance<ITestService11>();

			Assert.NotNull(value);
			Assert.Equal(typeof(TestService11_Decorator2), value.GetType());
			var decorator2 = (TestService11_Decorator2)value;

			Assert.NotNull(decorator2.TestService11);
			Assert.NotEqual(decorator2.TestService10, decorator2.TestService10FromIOC);
			Assert.Equal(typeof(TestService11_Decorator1), decorator2.TestService11.GetType());
			var decorator1 = (TestService11_Decorator1)decorator2.TestService11;

			Assert.NotNull(decorator1.TestService11);
			Assert.Equal(typeof(TestService11), decorator1.TestService11.GetType());
			var testService11 = (TestService11)decorator1.TestService11;

			Assert.NotNull(testService11.TestService10);
			Assert.Equal(typeof(TestService10), testService11.TestService10.GetType());
		}

	    [Fact]
	    public void GetInstance_DecorateNestedContainer()
	    {
	        var config = new BindingConfig();

	        config.Decorate<ITestService11>().With<TestService11_Decorator1>();

	        config.For<ITestService10>().Inject<TestService10>();
	        config.For<ITestService11>().Inject<TestService11>().With(Lifetime.PerContainer);

	        var container = new Container(config);

	        var value = container.GetInstance<ITestService11>();

	        Assert.NotNull(value);
	        Assert.Equal(typeof(TestService11_Decorator1), value.GetType());
	        var decorator1 = (TestService11_Decorator1)value;

	        Assert.NotNull(decorator1.TestService11);
	        Assert.Equal(typeof(TestService11), decorator1.TestService11.GetType());
	        var testService11 = (TestService11)decorator1.TestService11;

	        Assert.NotNull(testService11.TestService10);
	        Assert.Equal(typeof(TestService10), testService11.TestService10.GetType());

            var nestedConfig = new BindingConfig();
            nestedConfig.Decorate<ITestService11>().With<TestService11_Decorator2>();
            var nestedContainer = container.GetNestedContainer(nestedConfig);

	        var nestedInstance = nestedContainer.GetInstance<ITestService11>();

            Assert.Equal(typeof(TestService11_Decorator2), nestedInstance.GetType());
	        var nestedDecorator2 = (TestService11_Decorator2) nestedInstance;
            Assert.Equal(typeof(TestService11_Decorator1), nestedDecorator2.TestService11.GetType());
	        var nestedDecorator1 = (TestService11_Decorator1)nestedDecorator2.TestService11;
            Assert.Equal(typeof(TestService11), nestedDecorator1.TestService11.GetType());
	    }
    }
}
