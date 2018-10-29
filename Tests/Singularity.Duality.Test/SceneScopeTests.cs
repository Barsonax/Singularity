using System.Linq;
using Duality;
using Duality.Resources;
using Singularity.Bindings;
using Singularity.Duality.Scopes;
using Xunit;

namespace Singularity.Duality.Test
{
	public class SceneScopeTests
	{
		[Fact]
		public void SceneIsDisposed_ContainerIsDiposed()
		{
			var scene = new Scene();
			Scene.SwitchTo(new ContentRef<Scene>(scene));
		    using (var sceneScope = new SceneScope(new Container(Enumerable.Empty<IModule>()), scene))
		    {
		        Assert.False(sceneScope.Container.IsDisposed);
		        sceneScope.Dispose();
		        Assert.True(sceneScope.Container.IsDisposed);
		    }
		}

	    [Fact]
	    public void SceneEntered_DependencyIsInjected()
	    {
	        var scene = new Scene();
	        var gameObject = new GameObject("Test");
	        var testComponent = gameObject.AddComponent<TestComponentWithDependency>();

	        scene.AddObject(gameObject);
            Scene.SwitchTo(new ContentRef<Scene>(scene));

	        var bindings = new BindingConfig();
	        bindings.For<IModule>().Inject<TestModule>();
	        using (var sceneScope = new SceneScope(new Container(bindings), scene))
	        {
	            Assert.IsType<TestModule>(testComponent.Module);
	            Assert.Single(testComponent.InitCalls);
            }
	    }

        [Fact]
		public void AddGameObject_DependencyIsInjected()
		{
			var scene = new Scene();
			Scene.SwitchTo(new ContentRef<Scene>(scene));

			var bindings = new BindingConfig();
			bindings.For<IModule>().Inject<TestModule>();
		    using (var sceneScope = new SceneScope(new Container(bindings), scene))
		    {
		        var gameObject = new GameObject("Test");
		        var testComponent = gameObject.AddComponent<TestComponentWithDependency>();

		        scene.AddObject(gameObject);

		        Assert.IsType<TestModule>(testComponent.Module);
		        Assert.Single(testComponent.InitCalls);
            }
        }

		[Fact]
		public void AddComponent_DependencyIsInjected()
		{
			var scene = new Scene();
			Scene.SwitchTo(new ContentRef<Scene>(scene));

			var bindings = new BindingConfig();
			bindings.For<IModule>().Inject<TestModule>();
		    using (var sceneScope = new SceneScope(new Container(bindings), scene))
		    {
		        var gameObject = new GameObject("Test");

		        scene.AddObject(gameObject);
		        var testComponent = gameObject.AddComponent<TestComponentWithDependency>();

		        Assert.IsType<TestModule>(testComponent.Module);
		        Assert.Single(testComponent.InitCalls);
            }
		}
	}
}
