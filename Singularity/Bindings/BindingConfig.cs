using System;
using System.Collections.Generic;
using System.Reflection;

namespace Singularity
{
    public class BindingConfig
    {
        public readonly Dictionary<Type, IBinding> Bindings = new Dictionary<Type, IBinding>();
        public readonly Dictionary<Type, List<IDecoratorBinding>> Decorators = new Dictionary<Type, List<IDecoratorBinding>>();

        public Binding<TDependency> Bind<TDependency>()
        {
            var binding = new Binding<TDependency>();
            Add(binding);
            return binding;
        }

        public DecoratorBinding<TDecorator> RegisterDecorator<TDecorator>()
        {
            var decorator = new DecoratorBinding<TDecorator>(this);

            return decorator;
        }

        public void Add(IEnumerable<IBinding> bindings)
        {
            foreach (var binding in bindings)
            {
                Add(binding);
            }
        }

        public void Add(IBinding binding)
        {
            if (!binding.DependencyType.GetTypeInfo().IsInterface) throw new InterfaceExpectedException($"{binding.DependencyType} is not a interface.");
            Bindings.Add(binding.DependencyType, binding);
        }

        public void ValidateBindings()
        {
            foreach (var binding in Bindings.Values)
            {
                if (binding.Expression == null) throw new NullReferenceException();
            }
        }
    }
}