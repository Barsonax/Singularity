using System;
using System.Collections.Generic;
using Duality;
using Duality.Resources;
using Singularity.Bindings;
using Singularity.Exceptions;

namespace Singularity.Duality.Scopes
{
	public class SceneScope : IDisposable
	{
		public bool IsDisposed { get; private set; }
		public Container Container { get; }
		private readonly ISceneEventsProvider _sceneEventsProvider;
		private readonly ILogger _logger;

		public SceneScope(Container parentContainer, Scene scene, ISceneEventsProvider sceneEventsProvider, ILogger logger)
		{
			_logger = logger;
			_sceneEventsProvider = sceneEventsProvider;
			try
			{
				Container = parentContainer.GetNestedContainer(scene.FindComponents<IModule>());
			}
			catch (Exception e)
			{
				_logger.WriteError("Errors occured while initializing the scene scoped container");
				_logger.WriteError(e.Message);
				return;
			}
			InjectGameObjects(scene.AllObjects);
			_sceneEventsProvider.ComponentAdded += Scene_ComponentAdded;
			_sceneEventsProvider.GameObjectsAdded += Scene_GameObjectsAdded;
		}

		public void Dispose()
		{
			_sceneEventsProvider.ComponentAdded -= Scene_ComponentAdded;
			_sceneEventsProvider.GameObjectsAdded -= Scene_GameObjectsAdded;
			Container?.Dispose();
			IsDisposed = true;
		}

		private void Scene_GameObjectsAdded(object sender, GameObjectGroupEventArgs args)
		{
			InjectGameObjects(args.Objects);
		}

		private void Scene_ComponentAdded(object sender, ComponentEventArgs args)
		{
			try
			{
				Container.MethodInject(args.Component);
			}
			catch (Exception e)
			{
				_logger.WriteError(e.Message);
			}
		}

		public void InjectGameObjects(IEnumerable<GameObject> gameObjects)
		{
			foreach (GameObject gameObject in gameObjects)
			{
				try
				{
					Container.MethodInjectAll(gameObject.GetComponents<Component>());
				}
				catch (Exception e)
				{
					_logger.WriteError(e.Message);
				}
			}
		}
	}
}
