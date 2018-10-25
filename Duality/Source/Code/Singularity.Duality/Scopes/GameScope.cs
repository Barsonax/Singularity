using System;
using System.Collections.Generic;
using Duality;
using Duality.Resources;
using Singularity.Bindings;
using Singularity.Duality.Resources;

namespace Singularity.Duality.Scopes
{
	public class GameScope
	{
		public Container Container { get; }
		private SceneScope _sceneScope;

		public GameScope()
		{
			var dependencyResources = ContentProvider.GetAvailableContent<SingularityModules>();
			var modules = new List<IModule>();
			foreach (var dependencyResource in dependencyResources)
			{
				foreach (var moduleRef in dependencyResource.Res.Modules)
				{
					if (TryCreateModule(moduleRef, out var module))
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
				Log.Game.WriteWarning($"{nameof(Singularity)}: Could not resolve the type {moduleRef}");
			}
			else
			{
				try
				{
					var instance = Activator.CreateInstance(type);

					if (!(instance is IModule castedInstance))
					{
						Log.Game.WriteWarning($"{nameof(Singularity)}: The type {moduleRef} does not implement {nameof(IModule)}");
					}
					else
					{
						module = castedInstance;
						return true;
					}
				}
				catch (Exception e)
				{
					Log.Game.WriteWarning($"{nameof(Singularity)}: Could create a instance of the type {moduleRef}. The following exception was thrown {e}");
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
			_sceneScope = new SceneScope(this, Scene.Current);
		}

		private void Scene_Leaving(object sender, EventArgs e)
		{
			_sceneScope?.Dispose();
		}
	}
}
