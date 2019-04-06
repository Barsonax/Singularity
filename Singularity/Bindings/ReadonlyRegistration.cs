using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Singularity.Bindings
{
    internal class ReadonlyRegistration
    {
        public Type DependencyType { get; }
        public ReadOnlyCollection<Binding> Bindings { get; }
        public ReadOnlyCollection<Expression> Decorators { get; }

        public ReadonlyRegistration(Registration registration)
        {
            DependencyType = registration.DependencyType;
            var bindings = new Binding[registration.Bindings.Count];
            for (var i = 0; i < registration.Bindings.Count; i++)
            {
                bindings[i] = new Binding(registration.Bindings[i]);
            }

            Bindings = new ReadOnlyCollection<Binding>(bindings);

            var decorators = new Expression[registration.DecoratorBindings.Count];

            for (var i = 0; i < registration.DecoratorBindings.Count; i++)
            {
                decorators[i] = registration.DecoratorBindings[i].Expression;
            }

            Decorators = new ReadOnlyCollection<Expression>(decorators);
        }
    }
}