using System.Collections.Generic;
using System.Linq;
using Duality;
using Duality.Resources;
using NSubstitute;
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
			using (var sceneScope = new SceneScope(new Container(Enumerable.Empty<IModule>()), new Scene(), Substitute.For<ISceneEventsProvider>(), Substitute.For<ILogger>()))
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

			var bindings = new BindingConfig();
			bindings.For<IModule>().Inject<TestModule>();
			using (var sceneScope = new SceneScope(new Container(bindings), scene, Substitute.For<ISceneEventsProvider>(), Substitute.For<ILogger>()))
			{
				Assert.IsType<TestModule>(testComponent.Module);
				Assert.Single(testComponent.InitCalls);
			}
		}

		[Fact]
		public void AddGameObject_DependencyIsInjected()
		{
			var bindings = new BindingConfig();
			bindings.For<IModule>().Inject<TestModule>();
			var sceneEventsProvider = new SceneEventsProviderMockup();
			using (var sceneScope = new SceneScope(new Container(bindings), new Scene(), sceneEventsProvider, Substitute.For<ILogger>()))
			{
				var gameObject = new GameObject("Test");
				var testComponent = gameObject.AddComponent<TestComponentWithDependency>();

				sceneEventsProvider.TriggerGameObjectsAdded(new List<GameObject> { gameObject });

				Assert.IsType<TestModule>(testComponent.Module);
				Assert.Single(testComponent.InitCalls);
			}
		}

		[Fact]
		public void AddComponent_DependencyIsInjected()
		{
			var bindings = new BindingConfig();
			bindings.For<IModule>().Inject<TestModule>();
			var sceneEventsProvider = new SceneEventsProviderMockup();
			using (var sceneScope = new SceneScope(new Container(bindings), new Scene(), sceneEventsProvider, Substitute.For<ILogger>()))
			{
				var component = new TestComponentWithDependency();
				sceneEventsProvider.TriggerComponentAdded(component);

				Assert.IsType<TestModule>(component.Module);
				Assert.Single(component.InitCalls);
			}
		}
	}
}
