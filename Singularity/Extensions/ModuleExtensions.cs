using System.Collections.Generic;
using Singularity.Bindings;

namespace Singularity
{
	public static class ModuleExtensions
	{
		public static IEnumerable<IBinding> ToBindings(this IEnumerable<IModule> modules)
		{
			var config = new BindingConfig();
			foreach (IModule module in modules)
			{
				config.CurrentModule = module;
				module.Register(config);
			}

			config.CurrentModule = null;
			return config;
		}
	}
}
