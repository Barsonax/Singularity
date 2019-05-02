using System;
using System.Collections.Generic;
using Duality;
using Singularity.Duality.Scopes;

namespace Singularity.Duality.Test
{
	public class SceneEventsProviderMockup : ISceneEventsProvider
	{
		public event EventHandler<ComponentEventArgs> ComponentAdded;
		public event EventHandler<GameObjectGroupEventArgs> GameObjectsAdded;
		public event EventHandler Leaving;
		public event EventHandler Entered;

		public void TriggerComponentAdded(Component component)
		{
			ComponentAdded?.Invoke(this, new ComponentEventArgs(component));
		}

		public void TriggerGameObjectsAdded(List<GameObject> gameObjects)
		{
			GameObjectsAdded?.Invoke(this, new GameObjectGroupEventArgs(gameObjects));
		}

		public void TriggerLeaving()
		{
			Leaving?.Invoke(this, EventArgs.Empty);
		}

		public void TriggerEntered()
		{
			Entered?.Invoke(this, EventArgs.Empty);
		}

		public void Dispose()
		{

		}
	}
}