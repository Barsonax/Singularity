using System;
using System.Collections.Generic;
using Duality;
using Duality.Resources;

namespace Singularity.Duality
{
	public class SingularityPlugin : CorePlugin
	{
		private Container _container;

		protected override void OnGameStarting()
		{
			var config = new BindingConfig();
			//TODO add logic to configure the dependencies

			_container = new Container(config);
			Scene.ComponentAdded += Scene_ComponentAdded;
			Scene.Entered += Scene_Entered;
		}

		private void Scene_Entered(object sender, EventArgs e)
		{
			InjectGameObjects(Scene.Current.AllObjects);
		}

		protected override void OnGameEnded()
		{
			Scene.ComponentAdded -= Scene_ComponentAdded;
			Scene.Entered -= Scene_Entered;
		}

		private void Scene_ComponentAdded(object sender, ComponentEventArgs e)
		{
			_container.Inject(e.Component);
		}

		public void InjectGameObjects(IEnumerable<GameObject> gameObjects)
		{
			foreach (var gameObject in gameObjects)
			{
				_container.Inject(gameObject.GetComponents<Component>());
			}
		}
	}
}
