using System;
using System.Collections.Generic;
using Duality;
using Duality.Resources;
using Singularity.Bindings;

namespace Singularity.Duality.Scopes
{
	public class SceneScope : IDisposable
	{
		public bool IsDisposed { get; private set; }
		public Container Container { get; }
		private readonly ISceneEventsProvider _sceneEventsProvider;

		public SceneScope(Container parentContainer, Scene scene, ISceneEventsProvider sceneEventsProvider)
		{
			_sceneEventsProvider = sceneEventsProvider;
			Container = parentContainer.GetNestedContainer(scene.FindComponents<IModule>());

			InjectGameObjects(scene.AllObjects);
			_sceneEventsProvider.ComponentAdded += Scene_ComponentAdded;
			_sceneEventsProvider.GameObjectsAdded += Scene_GameObjectsAdded;
		}

		public void Dispose()
		{
			_sceneEventsProvider.ComponentAdded -= Scene_ComponentAdded;
			_sceneEventsProvider.GameObjectsAdded -= Scene_GameObjectsAdded;
			Container.Dispose();
			IsDisposed = true;
		}

		private void Scene_GameObjectsAdded(object sender, GameObjectGroupEventArgs e)
		{
			InjectGameObjects(e.Objects);
		}

		private void Scene_ComponentAdded(object sender, ComponentEventArgs e)
		{
			Container.MethodInject(e.Component);
		}

		public void InjectGameObjects(IEnumerable<GameObject> gameObjects)
		{
			foreach (GameObject gameObject in gameObjects)
			{
				Container.MethodInjectAll(gameObject.GetComponents<Component>());
			}
		}
	}
}
