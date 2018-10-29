using System;
using System.Collections.Generic;
using Duality;
using Duality.Resources;
using Singularity.Bindings;
using Singularity.Duality.Resources;

namespace Singularity.Duality.Scopes
{
	public class GameScope : IDisposable
	{
		public Container Container { get; }
		private SceneScope _sceneScope;
		private readonly ILogger _logger;
		private readonly ISceneScopeFactory _sceneScopeFactory;

		public GameScope(ILogger logger, ISceneScopeFactory sceneScopeFactory, IEnumerable<ContentRef<SingularityModules>> moduleResources)
		{
			_sceneScopeFactory = sceneScopeFactory;
			_logger = logger;
			var modules = new List<IModule>();
			foreach (var dependencyResource in moduleResources)
			{
				foreach (var moduleRef in dependencyResource.Res.Modules)
				{
					if (moduleRef == null)
					{
						_logger.WriteWarning($"{nameof(Singularity)}: {dependencyResource.FullName} contains a null module");
					}
					else if (TryCreateModule(moduleRef, out var module))
					{
						modules.Add(module);
					}
				}
			}

			Container = new Container(modules);

			Scene.Entered += Scene_Entered;
			Scene.Leaving += Scene_Leaving;
		}

		private bool TryCreateModule(ModuleRef moduleRef, out IModule module)
		{
			var type = moduleRef.Type;
			if (type == null)
			{
				_logger.WriteWarning($"{nameof(Singularity)}: Could not resolve the type {moduleRef}");
			}
			else
			{
				try
				{
					var instance = Activator.CreateInstance(type);

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
			Container.Dispose();

			Scene.Entered -= Scene_Entered;
			Scene.Leaving -= Scene_Leaving;
		}

		private void Scene_Entered(object sender, EventArgs e)
		{
			_sceneScope = _sceneScopeFactory.Create(this, Scene.Current);
		}

		private void Scene_Leaving(object sender, EventArgs e)
		{
			_sceneScope?.Dispose();
		}
	}
}
