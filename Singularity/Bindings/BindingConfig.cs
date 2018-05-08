using System;
using System.Collections.Generic;
using System.Reflection;

namespace Singularity
{
    public class BindingConfig
    {
        public readonly Dictionary<Type, IBinding> Bindings = new Dictionary<Type, IBinding>();
        public readonly Dictionary<Type, List<IDecoratorBinding>> Decorators = new Dictionary<Type, List<IDecoratorBinding>>();

        /// <summary>
        /// Begins configuring a binding for a certain dependency
        /// </summary>
        /// <typeparam name="TDependency"></typeparam>
        /// <returns></returns>
        public Binding<TDependency> Bind<TDependency>()
        {
            var binding = new Binding<TDependency>();
            Add(binding);
            return binding;
        }

        /// <summary>
        /// Begins configuring a decorator to wrap around a certain type.
        /// </summary>
        /// <typeparam name="TDecorator"></typeparam>
        /// <returns></returns>
        public DecoratorBinding<TDecorator> Decorate<TDecorator>()
        {
            var decorator = new DecoratorBinding<TDecorator>(this);

            return decorator;
        }

        public void Add(IBinding binding)
        {            
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