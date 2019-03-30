using System;
using System.Collections.Generic;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test
{
    public class ContainerTests
	{
		public class Nested
		{
			public class WithDependencies
			{
				[Fact]
				public void GetInstance_1DeepAndUsingDependencyFromParentContainer_CorrectDependencyIsReturned()
				{
					var config = new BindingConfig();
					config.Register<ITestService10, TestService10>();

					using (var container = new Container(config))
					{
						var nestedConfig = new BindingConfig();

						nestedConfig.Register<ITestService11, TestService11>();
						using (Container nestedContainer = container.GetNestedContainer(nestedConfig))
						{
							var nestedValue = nestedContainer.GetInstance<ITestService11>();
							Assert.Equal(typeof(TestService11), nestedValue.GetType());
						}

						var value = container.GetInstance<ITestService10>();
						Assert.Equal(typeof(TestService10), value.GetType());
					}
				}

				[Fact]
				public void GetInstance_2DeepAndUsingDependencyFromParentContainer_CorrectDependencyIsReturned()
				{
					var config = new BindingConfig();
					config.Register<ITestService10, TestService10>();

					using (var container = new Container(config))
					{
						var nestedConfig = new BindingConfig();

						nestedConfig.Register<ITestService11, TestService11>();
						using (Container nestedContainer = container.GetNestedContainer(nestedConfig))
						{
							var nestedValue = nestedContainer.GetInstance<ITestService11>();
							Assert.Equal(typeof(TestService11), nestedValue.GetType());
							Assert.Equal(typeof(TestService10), nestedValue.TestService10.GetType());

							var nestedConfig2 = new BindingConfig();
							nestedConfig2.Register<ITestService12, TestService12>();

							using (Container nestedContainer2 = nestedContainer.GetNestedContainer(nestedConfig2))
							{
								var nestedValue2 = nestedContainer2.GetInstance<ITestService12>();
								Assert.Equal(typeof(TestService12), nestedValue2.GetType());
								Assert.Equal(typeof(TestService11), nestedValue2.TestService11.GetType());
								Assert.Equal(typeof(TestService10), nestedValue2.TestService11.TestService10.GetType());
							}
						}

						var value = container.GetInstance<ITestService10>();
						Assert.Equal(typeof(TestService10), value.GetType());
					}
				}
			}

			public class NoDependencies
			{
				[Fact]
				public void GetInstance_CorrectDependencyIsReturned()
				{
					var config = new BindingConfig();
					config.Register<ITestService10, TestService10>();

					using (var container = new Container(config))
					{
						var nestedConfig = new BindingConfig();

						using (Container nestedContainer = container.GetNestedContainer(nestedConfig))
						{
							var nestedValue = nestedContainer.GetInstance<ITestService10>();
							Assert.Equal(typeof(TestService10), nestedValue.GetType());
						}

						var value = container.GetInstance<ITestService10>();
						Assert.Equal(typeof(TestService10), value.GetType());
					}
				}

				[Fact]
				public void GetInstance_Module_CorrectDependencyIsReturned()
				{

					var config = new BindingConfig();
					config.Register<ITestService10, TestService10>();

					using (var container = new Container(config))
					{
						var module = new TestModule1();

						using (Container nestedContainer = container.GetNestedContainer(new[] { module }))
						{
							var nestedValue = nestedContainer.GetInstance<ITestService10>();
							Assert.Equal(typeof(TestService10), nestedValue.GetType());
						}

						var value = container.GetInstance<ITestService10>();
						Assert.Equal(typeof(TestService10), value.GetType());
					}
				}

				[Fact]
				public void GetInstance_Override_CorrectDependencyIsReturned()
				{
					var config = new BindingConfig();
					config.Register<ITestService10, TestService10>();

					using (var container = new Container(config))
					{
						var nestedConfig = new BindingConfig();

						nestedConfig.Register<ITestService10, TestService10Variant>();
						using (Container nestedContainer = container.GetNestedContainer(nestedConfig))
						{
							var nestedValue = nestedContainer.GetInstance<ITestService10>();
							Assert.Equal(typeof(TestService10Variant), nestedValue.GetType());
						}

						var value = container.GetInstance<ITestService10>();
						Assert.Equal(typeof(TestService10), value.GetType());
					}
				}
			}
		}

		public class Flat
		{
			public class Decorators
			{
				[Fact]
				public void GetInstance_Decorate_Simple()
				{
					var config = new BindingConfig();
                    config.Decorate<IComponent, Decorator1>();
					config.Register<IComponent, Component>();

					var container = new Container(config);

					var value = container.GetInstance<IComponent>();

					Assert.NotNull(value);
					Assert.Equal(typeof(Decorator1), value.GetType());
					var decorator1 = (Decorator1)value;
					Assert.Equal(typeof(Component), decorator1.Component.GetType());
				}

				[Fact]
				public void GetInstance_Decorate_Complex1()
				{
					var config = new BindingConfig();

					config.Decorate<IComponent, Decorator1>();
					config.Decorate<IComponent, Decorator2>();

					config.Register<IComponent, Component>();

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

					config.Decorate<ITestService11, TestService11_Decorator1>();

					config.Register<ITestService10, TestService10>();
					config.Register<ITestService11, TestService11>();

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

					config.Decorate<ITestService11, TestService11_Decorator1>();
					config.Decorate<ITestService11, TestService11_Decorator2>();

					config.Register<ITestService10, TestService10>();
					config.Register<ITestService11, TestService11>();

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
			}

			public class Disposed
			{
				[Fact]
				public void GetInstance_PerContainerLifetime_IsDisposed()
				{
					var config = new BindingConfig();
					config.Register<IDisposable, Disposable>().With(CreationMode.Singleton).OnDeath(x => x.Dispose());

					var container = new Container(config);

					var disposable = container.GetInstance<IDisposable>();
					Assert.NotNull(disposable);
					Assert.Equal(typeof(Disposable), disposable.GetType());

					var value = (Disposable)disposable;
					Assert.False(value.IsDisposed);
					container.Dispose();
					Assert.True(value.IsDisposed);
				}

				[Fact]
				public void GetInstance_PerCallLifetime_IsDisposed()
				{
					var config = new BindingConfig();
					config.Register<IDisposable, Disposable>().OnDeath(x => x.Dispose());

					var container = new Container(config);

					var disposable = container.GetInstance<IDisposable>();
					Assert.NotNull(disposable);
					Assert.Equal(typeof(Disposable), disposable.GetType());

					var value = (Disposable)disposable;
					Assert.False(value.IsDisposed);
					container.Dispose();
					Assert.True(value.IsDisposed);
				}

				[Fact]
				public void GetInstance_Decorator_IsDisposed()
				{
					var config = new BindingConfig();
					config.Register<IDisposable, Disposable>().OnDeath(x => x.Dispose());
					config.Decorate<IDisposable, DisposableDecorator>();

					var container = new Container(config);

					var disposable = container.GetInstance<IDisposable>();
					Assert.NotNull(disposable);
					Assert.Equal(typeof(DisposableDecorator), disposable.GetType());
					var disposableDecorator = (DisposableDecorator)disposable;
					Assert.Equal(typeof(Disposable), disposableDecorator.Disposable.GetType());

					var value = (Disposable)disposableDecorator.Disposable;
					Assert.False(value.IsDisposed);
					container.Dispose();
					Assert.True(value.IsDisposed);
				}
			}

			public class WithDependencies
			{
				[Fact]
				public void GetInstance_1Deep_DependenciesAreCorrectlyInjected()
				{
					var config = new BindingConfig();
					config.Register<ITestService10, TestService10>();
					config.Register<ITestService11, TestService11>();

					var container = new Container(config);

					var value = container.GetInstance<ITestService11>();
					Assert.Equal(typeof(TestService11), value.GetType());
					Assert.NotNull(value.TestService10);
					Assert.Equal(typeof(TestService10), value.TestService10.GetType());
				}

				[Fact]
				public void GetInstance_2Deep_DependenciesAreCorrectlyInjected()
				{
					var config = new BindingConfig();
					config.Register<ITestService10, TestService10>();
					config.Register<ITestService11, TestService11>();
					config.Register<ITestService12, TestService12>();

					var container = new Container(config);

					var value = container.GetInstance<ITestService12>();
					Assert.Equal(typeof(TestService12), value.GetType());
					Assert.NotNull(value.TestService11);
					Assert.Equal(typeof(TestService11), value.TestService11.GetType());
					Assert.NotNull(value.TestService11.TestService10);
					Assert.Equal(typeof(TestService10), value.TestService11.TestService10.GetType());
				}

				[Fact]
				public void GetInstance_1Deep_ReturnsNewInstancePerCall()
				{
					var config = new BindingConfig();
					config.Register<ITestService10, TestService10>();
					config.Register<ITestService11, TestService11>();

					var container = new Container(config);

					var value1 = container.GetInstance<ITestService11>();
					var value2 = container.GetInstance<ITestService11>();

					Assert.NotNull(value1);
					Assert.NotNull(value2);
					Assert.NotEqual(value1, value2);

					Assert.NotNull(value1.TestService10);
					Assert.NotNull(value2.TestService10);
					Assert.NotEqual(value1.TestService10, value2.TestService10);
				}

				[Fact]
				public void GetInstance_1DeepAndPerContainerLifetime_ReturnsSameInstancePerCall()
				{
					var config = new BindingConfig();
					config.Register<ITestService10, TestService10>().With(CreationMode.Singleton);
					config.Register<ITestService11, TestService11>();

					var container = new Container(config);

					var value1 = container.GetInstance<ITestService11>();
					var value2 = container.GetInstance<ITestService11>();

					Assert.NotNull(value1);
					Assert.NotNull(value2);
					Assert.NotEqual(value1, value2);

					Assert.NotNull(value1.TestService10);
					Assert.NotNull(value2.TestService10);
					Assert.Equal(value1.TestService10, value2.TestService10);
				}

				[Fact]
				public void GetInstance_2Deep_ReturnsNewInstancePerCall()
				{
					var config = new BindingConfig();
					config.Register<ITestService10, TestService10>();
					config.Register<ITestService11, TestService11>();
					config.Register<ITestService12, TestService12>();

					var container = new Container(config);

					var value1 = container.GetInstance<ITestService12>();
					var value2 = container.GetInstance<ITestService12>();

					Assert.NotNull(value1);
					Assert.NotNull(value2);
					Assert.NotEqual(value1, value2);

					Assert.NotNull(value1.TestService11);
					Assert.NotNull(value2.TestService11);
					Assert.NotEqual(value1.TestService11, value2.TestService11);

					Assert.NotNull(value1.TestService11.TestService10);
					Assert.NotNull(value2.TestService11.TestService10);
					Assert.NotEqual(value1.TestService11.TestService10, value2.TestService11.TestService10);
				}

				[Fact]
				public void GetInstance_2DeepAndPerContainerLifetime_ReturnsNewInstancePerCall()
				{
					var config = new BindingConfig();
					config.Register<ITestService10, TestService10>().With(CreationMode.Singleton);
					config.Register<ITestService11, TestService11>();
					config.Register<ITestService12, TestService12>();

					var container = new Container(config);

					var value1 = container.GetInstance<ITestService12>();
					var value2 = container.GetInstance<ITestService12>();

					Assert.NotNull(value1);
					Assert.NotNull(value2);
					Assert.NotEqual(value1, value2);

					Assert.NotNull(value1.TestService11);
					Assert.NotNull(value2.TestService11);
					Assert.NotEqual(value1.TestService11, value2.TestService11);

					Assert.NotNull(value1.TestService11.TestService10);
					Assert.NotNull(value2.TestService11.TestService10);
					Assert.Equal(value1.TestService11.TestService10, value2.TestService11.TestService10);
				}

				[Fact]
				public void GetInstanceFactory_GetDependencyByConcreteType_ReturnsCorrectDependency()
				{
					var config = new BindingConfig();
					config.Register<ITestService10, TestService10>();

					var container = new Container(config);

					TestService11 value = container.GetInstanceFactory<TestService11>().Invoke();

					Assert.Equal(typeof(TestService11), value.GetType());
				}

				[Fact]
				public void GetInstanceFactory_GetDependencyByConcreteType_WithConcreteDependency_ReturnsCorrectDependency()
				{
					var config = new BindingConfig();

					var container = new Container(config);

					TestService11WithConcreteDependency value = container.GetInstanceFactory<TestService11WithConcreteDependency>().Invoke();

					Assert.Equal(typeof(TestService11WithConcreteDependency), value.GetType());
					Assert.NotNull(value.TestService10);
				}

				[Fact]
				public void GetInstanceFactory_GetDependencyByConcreteType_WithConcreteDependency_2Deep_ReturnsCorrectDependency()
				{
					var config = new BindingConfig();

					var container = new Container(config);

					TestService12WithConcreteDependency value = container.GetInstanceFactory<TestService12WithConcreteDependency>().Invoke();

					Assert.Equal(typeof(TestService12WithConcreteDependency), value.GetType());
					Assert.NotNull(value.TestService11);
				}

				[Fact]
				public void GetInstanceFactory_GetDependencyByConcreteType_WithMixedConcreteDependency_2Deep_ReturnsCorrectDependency()
				{
					var config = new BindingConfig();
					config.Register<ITestService10, TestService10>();
					config.Register<ITestService11, TestService11>();

					var container = new Container(config);

					TestService12WithMixedConcreteDependency value = container.GetInstanceFactory<TestService12WithMixedConcreteDependency>().Invoke();

					Assert.Equal(typeof(TestService12WithMixedConcreteDependency), value.GetType());
					var testService11 = Assert.IsType<TestService11>(value.TestService11);
					Assert.IsType<TestService10>(testService11.TestService10);
				}
			}

			public class NoDependencies
			{
				[Fact]
				public void GetInstanceFactory_GetDependencyByInterface_ReturnsCorrectDependency()
				{
					var config = new BindingConfig();
					config.Register<ITestService10, TestService10>();

					var container = new Container(config);

					ITestService10 value = container.GetInstanceFactory<ITestService10>().Invoke();

					Assert.Equal(typeof(TestService10), value.GetType());
				}

				[Fact]
				public void GetInstanceFactory_GetDependencyByConcreteType_ReturnsCorrectDependency()
				{
					var config = new BindingConfig();

					var container = new Container(config);

					TestService10 value = container.GetInstanceFactory<TestService10>().Invoke();

					Assert.Equal(typeof(TestService10), value.GetType());
				}

				[Fact]
				public void GetInstance_ReturnsCorrectDependency()
				{
					var config = new BindingConfig();
					config.Register<ITestService10, TestService10>();

					var container = new Container(config);

					var value = container.GetInstance<ITestService10>();
					Assert.Equal(typeof(TestService10), value.GetType());
				}

				[Fact]
				public void GetInstance_FuncWithMethodCall_ReturnsCorrectDependency()
				{
					var config = new BindingConfig();
					config.Register<ITestService10>().Inject(() => CreateTestService());

					var container = new Container(config);

					var value = container.GetInstance<ITestService10>();
					Assert.Equal(typeof(TestService10), value.GetType());
				}

				[Fact]
				public void GetInstance_FuncWithConstructorCall_ReturnsCorrectDependency()
				{
					var config = new BindingConfig();
					config.Register<ITestService10>().Inject(() => new TestService10());

					var container = new Container(config);

					var value = container.GetInstance<ITestService10>();
					Assert.Equal(typeof(TestService10), value.GetType());
				}

				[Fact]
				public void GetInstance_FuncWithDelegateCall_ReturnsCorrectDependency()
				{
					var config = new BindingConfig();
					Func<TestService10> func = () => new TestService10();
					config.Register<ITestService10>().Inject(() => func.Invoke());

					var container = new Container(config);

					var value = container.GetInstance<ITestService10>();
					Assert.Equal(typeof(TestService10), value.GetType());
				}

				private TestService10 CreateTestService()
				{
					return new TestService10();
				}

				[Fact]
				public void MethodInject_InjectsCorrectDependencies()
				{
					var config = new BindingConfig();
					config.Register<ITestService10, TestService10>();

					var container = new Container(config);

					var instance = new MethodInjectionClass();
					container.MethodInject(instance);

					Assert.Equal(typeof(TestService10), instance.TestService10?.GetType());
				}

				[Fact]
				public void MethodInjectAll_InjectsCorrectDependencies()
				{
					var config = new BindingConfig();
					config.Register<ITestService10, TestService10>();

					var container = new Container(config);

					var instances = new List<MethodInjectionClass>();
					for (var i = 0; i < 10; i++)
					{
						instances.Add(new MethodInjectionClass());
					}
					container.MethodInjectAll(instances);

					foreach (MethodInjectionClass instance in instances)
					{
						Assert.Equal(typeof(TestService10), instance.TestService10?.GetType());
					}
				}

				[Fact]
				public void GetInstance_PerContainerLifetime_ReturnsSameInstancePerCall()
				{
					var config = new BindingConfig();
					config.Register<ITestService10, TestService10>().With(CreationMode.Singleton);

					var container = new Container(config);

					var value1 = container.GetInstance<ITestService10>();
					var value2 = container.GetInstance<ITestService10>();

					Assert.NotNull(value1);
					Assert.NotNull(value2);
					Assert.Equal(value1, value2);
				}

				[Fact]
				public void GetInstance_PerCallLifetime_ReturnsNewInstancePerCall()
				{
					var config = new BindingConfig();
					config.Register<ITestService10, TestService10>();

					var container = new Container(config);

					var value1 = container.GetInstance<ITestService10>();
					var value2 = container.GetInstance<ITestService10>();

					Assert.NotNull(value1);
					Assert.NotNull(value2);
					Assert.NotEqual(value1, value2);
				}
			}
		}
	}
}