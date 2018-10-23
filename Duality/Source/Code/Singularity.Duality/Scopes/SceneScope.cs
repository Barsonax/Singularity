using System.Collections.Generic;
using Duality;
using Duality.Resources;
using Singularity.Bindings;

namespace Singularity.Duality.Scopes
{
	public class SceneScope
	{
		public Container Container { get; }

		public SceneScope(GameScope gameScope, Scene scene)
		{
			Container = gameScope.Container.GetNestedContainer(scene.FindComponents<IModule>());

			InjectGameObjects(Scene.Current.AllObjects);
			Scene.ComponentAdded += Scene_ComponentAdded;
		}

		public void Dispose()
		{
			Scene.ComponentAdded -= Scene_ComponentAdded;
			Container.Dispose();
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
