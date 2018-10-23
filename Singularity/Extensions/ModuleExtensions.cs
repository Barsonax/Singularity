using System.Collections.Generic;
using Singularity.Bindings;

namespace Singularity
{
	public static class ModuleExtensions
	{
		public static IEnumerable<IBinding> ToBindings(this IEnumerable<IModule> modules)
		{
			var config = new BindingConfig();
			foreach (var module in modules)
			{
				module.Register(config);
			}
			return config;
		}
	}
}
