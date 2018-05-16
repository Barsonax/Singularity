using System;
using System.Collections.Generic;
using Duality;
using Duality.Resources;
using Singularity.Bindings;

namespace Singularity.Duality
{
	public class SingularityPlugin : CorePlugin
	{
		private Container _container;

		protected override void OnGameStarting()
		{
			Scene.ComponentAdded += Scene_ComponentAdded;
			Scene.Entered += Scene_Entered;
			Scene.Leaving += Scene_Leaving;
		}

		private void Scene_Leaving(object sender, EventArgs e)
		{
			_container?.Dispose();
		}

		private void Scene_Entered(object sender, EventArgs e)
		{
			var modules = Scene.Current.FindComponents<IModule>();
			var config = new BindingConfig();
			foreach (var module in modules)
			{
				module.Register(config);
			}

			_container = new Container(config);

			InjectGameObjects(Scene.Current.AllObjects);
		}

		protected override void OnGameEnded()
		{
			Scene.ComponentAdded -= Scene_ComponentAdded;
			Scene.Entered -= Scene_Entered;
		}

		private void Scene_ComponentAdded(object sender, ComponentEventArgs e)
		{
			_container.MethodInject(e.Component);
		}

		public void InjectGameObjects(IEnumerable<GameObject> gameObjects)
		{
			foreach (var gameObject in gameObjects)
			{
				_container.MethodInjectAll(gameObject.GetComponents<Component>());
			}
		}
	}
}
