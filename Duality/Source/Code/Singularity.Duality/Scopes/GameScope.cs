using System;
using System.Linq;
using Duality.Resources;
using Singularity.Bindings;

namespace Singularity.Duality.Scopes
{
	public class GameScope
	{
		public Container Container { get; }
		private SceneScope _sceneScope;

		public GameScope()
		{
			Container = new Container(Enumerable.Empty<IModule>()); //TODO implement logic here to define and get game scoped modules here.

			Scene.Entered += Scene_Entered;
			Scene.Leaving += Scene_Leaving;
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
