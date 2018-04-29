using System;
using System.Linq.Expressions;

namespace Singularity
{
	public interface IBinding
	{
		Type DependencyType { get; }
		Expression BindingExpression { get; set; }
		Lifetime Lifetime { get; }
		bool IsResolved { get; set; }
	}
}
