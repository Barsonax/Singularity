using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Singularity.Bindings
{
	public class ReadOnlyBindingConfig : IBindingConfig
	{
		public IReadOnlyDictionary<Type, IBinding> Bindings { get; }

		public ReadOnlyBindingConfig(IEnumerable<IBinding> bindings)
		{
			var dic = new Dictionary<Type, IBinding>();
			foreach (var bindingConfigBinding in bindings)
			{
				dic.Add(bindingConfigBinding.DependencyType, new ReadOnlyBinding(bindingConfigBinding));
			}
			Bindings = new ReadOnlyDictionary<Type, IBinding>(dic);
		}
	}
}