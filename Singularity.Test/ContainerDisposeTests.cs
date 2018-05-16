using System;
using Singularity.Bindings;
using Singularity.Test.TestClasses;
using Xunit;

namespace Singularity.Test
{
	public class ContainerDisposeTests
    {
	    [Fact]
	    public void GetInstance_NestedContainerWithPerContainerLifetime_IsDisposed()
	    {
		    var config = new BindingConfig();
		    config.For<IDisposable>().Inject<Disposable>().With(Lifetime.PerContainer).OnDeath(x => x.Dispose());

		    var container = new Container(config);

		    var topLevelInstance = container.GetInstance<IDisposable>();
		    Assert.NotNull(topLevelInstance);
		    Assert.Equal(typeof(Disposable), topLevelInstance.GetType());

		    {
			    var nestedConfig = new BindingConfig();
			    nestedConfig.For<IDisposable>().Inject<Disposable>().With(Lifetime.PerContainer).OnDeath(x => x.Dispose());
			    var nestedContainer = container.GetNestedContainer(nestedConfig);
			    var nestedInstance = nestedContainer.GetInstance<IDisposable>();

			    Assert.NotNull(nestedInstance);
			    Assert.Equal(typeof(Disposable), nestedInstance.GetType());

			    var castednestedInstance = (Disposable)nestedInstance;
			    Assert.False(castednestedInstance.IsDisposed);
			    nestedContainer.Dispose();
			    Assert.True(castednestedInstance.IsDisposed);
		    }

		    var castedTopLevelInstance = (Disposable)topLevelInstance;
		    Assert.False(castedTopLevelInstance.IsDisposed);
		    container.Dispose();
		    Assert.True(castedTopLevelInstance.IsDisposed);
	    }

	    [Fact]
	    public void GetInstance_NestedContainerWithPerContainerLifetime_IsDisposedInTopLevel()
	    {
		    var config = new BindingConfig();
		    config.For<IDisposable>().Inject<Disposable>().With(Lifetime.PerContainer).OnDeath(x => x.Dispose());

		    var container = new Container(config);

		    var topLevelInstance = container.GetInstance<IDisposable>();
		    Assert.NotNull(topLevelInstance);
		    Assert.Equal(typeof(Disposable), topLevelInstance.GetType());

		    {
			    var nestedConfig = new BindingConfig();
			    
			    var nestedContainer = container.GetNestedContainer(nestedConfig);
			    var nestedInstance = nestedContainer.GetInstance<IDisposable>();

			    Assert.NotNull(nestedInstance);
			    Assert.Equal(typeof(Disposable), nestedInstance.GetType());

			    var castednestedInstance = (Disposable)nestedInstance;
			    Assert.False(castednestedInstance.IsDisposed);
			    nestedContainer.Dispose();
			    Assert.False(castednestedInstance.IsDisposed);
		    }

		    var castedTopLevelInstance = (Disposable)topLevelInstance;
		    Assert.False(castedTopLevelInstance.IsDisposed);
		    container.Dispose();
		    Assert.True(castedTopLevelInstance.IsDisposed);
	    }

	    [Fact]
	    public void GetInstance_NestedContainerWithPerCallLifetime_IsDisposedInTopLevel()
	    {
		    var config = new BindingConfig();
		    config.For<IDisposable>().Inject<Disposable>().OnDeath(x => x.Dispose());

		    var container = new Container(config);

		    var topLevelInstance = container.GetInstance<IDisposable>();
		    Assert.NotNull(topLevelInstance);
		    Assert.Equal(typeof(Disposable), topLevelInstance.GetType());

		    {
			    var nestedConfig = new BindingConfig();

			    var nestedContainer = container.GetNestedContainer(nestedConfig);
			    var nestedInstance = nestedContainer.GetInstance<IDisposable>();

			    Assert.NotNull(nestedInstance);
			    Assert.Equal(typeof(Disposable), nestedInstance.GetType());

			    var castednestedInstance = (Disposable)nestedInstance;
			    Assert.False(castednestedInstance.IsDisposed);
			    nestedContainer.Dispose();
			    Assert.True(castednestedInstance.IsDisposed);
		    }

		    var castedTopLevelInstance = (Disposable)topLevelInstance;
		    Assert.False(castedTopLevelInstance.IsDisposed);
		    container.Dispose();
		    Assert.True(castedTopLevelInstance.IsDisposed);
	    }


		[Fact]
	    public void GetInstance_WithPerContainerLifetime_IsDisposed()
	    {
		    var config = new BindingConfig();
		    config.For<IDisposable>().Inject<Disposable>().With(Lifetime.PerContainer).OnDeath(x => x.Dispose());

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
	    public void GetInstance_WithPerCallLifetime_IsDisposed()
	    {
		    var config = new BindingConfig();
		    config.For<IDisposable>().Inject<Disposable>().OnDeath(x => x.Dispose());

		    var container = new Container(config);

		    var disposable = container.GetInstance<IDisposable>();
		    Assert.NotNull(disposable);
		    Assert.Equal(typeof(Disposable), disposable.GetType());

		    var value = (Disposable)disposable;
		    Assert.False(value.IsDisposed);
		    container.Dispose();
		    Assert.True(value.IsDisposed);
	    }
	}
}
