using System;
using Duality;
using Duality.Resources;

namespace Singularity.Duality.Scopes
{
	public class SceneEventsProvider : ISceneEventsProvider
	{
		public event EventHandler<ComponentEventArgs> ComponentAdded;
		public event EventHandler<GameObjectGroupEventArgs> GameObjectsAdded;
		public event EventHandler Leaving;
		public event EventHandler Entered;

		public SceneEventsProvider()
		{
			Scene.ComponentAdded += OnSceneOnComponentAdded;
			Scene.GameObjectsAdded += OnSceneOnGameObjectsAdded;
			Scene.Leaving += Scene_Leaving;
			Scene.Entered += Scene_Entered;
		}

		private void Scene_Entered(object sender, EventArgs e)
		{
			Entered?.Invoke(sender, e);
		}

		private void Scene_Leaving(object sender, EventArgs e)
		{
			Leaving?.Invoke(sender, e);
		}

		private void OnSceneOnGameObjectsAdded(object sender, GameObjectGroupEventArgs args)
		{
			GameObjectsAdded?.Invoke(sender, args);
		}

		private void OnSceneOnComponentAdded(object sender, ComponentEventArgs args)
		{
			ComponentAdded?.Invoke(sender, args);
		}

		public void Dispose()
		{
			Scene.ComponentAdded -= OnSceneOnComponentAdded;
			Scene.GameObjectsAdded -= OnSceneOnGameObjectsAdded;
			Scene.Leaving -= Scene_Leaving;
			Scene.Entered -= Scene_Entered;
		}
	}
}
