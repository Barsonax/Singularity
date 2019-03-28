using System;
using System.Collections.Generic;

using Duality.Resources;
using Singularity.Duality.Resources;

namespace Singularity.Duality.Scopes
{
    internal sealed class GameScope : IDisposable
	{
		public Container Container { get; }
		private SceneScope _sceneScope;
		private readonly ILogger _logger;
		private readonly ISceneScopeFactory _sceneScopeFactory;
		private readonly ISceneEventsProvider _sceneEventsProvider;

		internal GameScope(ILogger logger, ISceneScopeFactory sceneScopeFactory, ISceneEventsProvider sceneEventsProvider, IEnumerable<SingularityModules> moduleResources)
		{
			_sceneEventsProvider = sceneEventsProvider;
			_sceneScopeFactory = sceneScopeFactory;
			_logger = logger;
			var modules = new List<IModule>();
			foreach (SingularityModules dependencyResource in moduleResources)
			{
				foreach (ModuleRef moduleRef in dependencyResource.Modules)
				{
					if (moduleRef == null)
					{
						_logger.WriteWarning($"{nameof(Singularity)}: {dependencyResource.FullName} contains a null module");
					}
					else if (TryCreateModule(moduleRef, out IModule? module))
					{
						modules.Add(module!);
					}
				}
			}

			try
			{
				Container = new Container(modules);
				_sceneEventsProvider.Entered += Scene_Entered;
				_sceneEventsProvider.Leaving += Scene_Leaving;
			}
			catch (Exception e)
			{
				_logger.WriteError("Errors occured while initializing the game scoped container");
				_logger.WriteError(e.Message);
			}
		}

		private bool TryCreateModule(ModuleRef moduleRef, out IModule? module)
		{
			Type type = moduleRef.Type;
			if (type == null)
			{
				_logger.WriteWarning($"{nameof(Singularity)}: Could not resolve the type {moduleRef}");
			}
			else
			{
				try
				{
					object instance = Activator.CreateInstance(type);

					if (!(instance is IModule castedInstance))
					{
						_logger.WriteWarning($"{nameof(Singularity)}: The type {moduleRef} does not implement {nameof(IModule)}");
					}
					else
					{
						module = castedInstance;
						return true;
					}
				}
				catch (Exception e)
				{
					_logger.WriteWarning($"{nameof(Singularity)}: Could create a instance of the type {moduleRef}. The following exception was thrown {e}");
				}
			}
			module = null;
			return false;
		}

		public void Dispose()
		{
			_sceneEventsProvider.Dispose();
			Container?.Dispose();
			_sceneEventsProvider.Entered -= Scene_Entered;
			_sceneEventsProvider.Leaving -= Scene_Leaving;
		}

		private void Scene_Entered(object sender, EventArgs e)
		{
			_sceneScope = _sceneScopeFactory.Create(this, Scene.Current, _sceneEventsProvider, _logger);
		}

		private void Scene_Leaving(object sender, EventArgs e)
		{
			_sceneScope?.Dispose();
		}
	}
}
