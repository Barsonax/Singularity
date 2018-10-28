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

		public SceneScope(Container parentContainer, Scene scene)
		{
			Container = parentContainer.GetNestedContainer(scene.FindComponents<IModule>());

			InjectGameObjects(Scene.Current.AllObjects);
			Scene.ComponentAdded += Scene_ComponentAdded;
			Scene.GameObjectsAdded += Scene_GameObjectsAdded;
		}

		public void Dispose()
		{
			Scene.GameObjectsAdded -= Scene_GameObjectsAdded;
			Scene.ComponentAdded -= Scene_ComponentAdded;
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
			foreach (var gameObject in gameObjects)
			{
				Container.MethodInjectAll(gameObject.GetComponents<Component>());
			}
		}
	}
}
