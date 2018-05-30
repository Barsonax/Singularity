using System;
using System.Collections.Generic;

namespace Singularity.Bindings
{
	public interface IBindingConfig
	{
		IReadOnlyDictionary<Type, IBinding> Bindings { get; }
	}
}