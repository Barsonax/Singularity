using System;
using Duality;

namespace Singularity.Duality.Scopes
{
    internal interface ISceneEventsProvider : IDisposable
	{
		event EventHandler<ComponentEventArgs> ComponentAdded;
		event EventHandler<GameObjectGroupEventArgs> GameObjectsAdded;

		event EventHandler Leaving;
		event EventHandler Entered;
	}
}