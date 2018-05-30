using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Singularity.Bindings
{
	public class ReadOnlyBinding : IBinding
	{
		public Type DependencyType { get; private set; }
		public Expression Expression { get; private set; }
		public Lifetime Lifetime { get; private set; }
		public Action<object> OnDeath { get; private set; }
		public IReadOnlyList<IDecoratorBinding> Decorators { get; private set; }

		public ReadOnlyBinding(Type dependencyType, Expression expression, Lifetime lifetime, Action<object> onDeath, IEnumerable<IDecoratorBinding> decorators)
		{
			Init(dependencyType, expression, lifetime, onDeath, decorators);
		}

		public ReadOnlyBinding(IBinding binding)
		{
			Init(binding.DependencyType, binding.Expression, binding.Lifetime, binding.OnDeath, binding.Decorators);
		}

		private void Init(Type dependencyType, Expression expression, Lifetime lifetime, Action<object> onDeath, IEnumerable<IDecoratorBinding> decorators)
		{
			if (decorators == null) throw new ArgumentException("Decorators cannot be null. If there are no decorators provide a empty IEnumerable<IDecoratorBinding>");
			if (expression == null && !decorators.Any()) throw new ArgumentException();
			DependencyType = dependencyType;
			Expression = expression;
			Lifetime = lifetime;
			OnDeath = onDeath;
			Decorators = new ReadOnlyCollection<IDecoratorBinding>(decorators.ToList());
		}
	}
}