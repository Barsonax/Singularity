using System;
using System.Linq.Expressions;

using Singularity.Enums;

namespace Singularity.Bindings
{
	public interface IConfiguredBinding
	{
		Expression Expression { get; }
		Lifetime Lifetime { get; }
        Action<object> OnDeath { get; }
	}
}