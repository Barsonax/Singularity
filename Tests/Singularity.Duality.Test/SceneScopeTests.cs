using System.Collections.Generic;
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
			using (var sceneScope = new SceneScope(new Container(Enumerable.Empty<IModule>()), new Scene(), new SceneEventsProviderMockup(), new LoggerMockup()))
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
            bindings.Register<IModule, TestModule>();
			using (var sceneScope = new SceneScope(new Container(bindings), scene, new SceneEventsProviderMockup(), new LoggerMockup()))
			{
				Assert.IsType<TestModule>(testComponent.Module);
				Assert.Single(testComponent.InitCalls);
			}
		}

		[Fact]
		public void AddGameObject_DependencyIsInjected()
		{
			var bindings = new BindingConfig();
			bindings.Register<IModule, TestModule>();
			var sceneEventsProvider = new SceneEventsProviderMockup();
			using (var sceneScope = new SceneScope(new Container(bindings), new Scene(), sceneEventsProvider, new LoggerMockup()))
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
			bindings.Register<IModule, TestModule>();
			var sceneEventsProvider = new SceneEventsProviderMockup();
			using (var sceneScope = new SceneScope(new Container(bindings), new Scene(), sceneEventsProvider, new LoggerMockup()))
			{
				var component = new TestComponentWithDependency();
				sceneEventsProvider.TriggerComponentAdded(component);

				Assert.IsType<TestModule>(component.Module);
				Assert.Single(component.InitCalls);
			}
		}
	}
}
