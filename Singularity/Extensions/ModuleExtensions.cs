using System.Collections.Generic;

namespace Singularity
{
	internal static class ModuleExtensions
	{
		internal static BindingConfig ToBindings(this IEnumerable<IModule> modules)
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
