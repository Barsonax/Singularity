using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Singularity.Bindings
{
    internal class ReadonlyRegistration
    {
        public Type DependencyType { get; }
        public ReadOnlyCollection<Binding> Bindings { get; }
        public ReadOnlyCollection<Expression> Decorators { get; }

        public ReadonlyRegistration(Type dependencyType, IEnumerable<Binding> bindings, IEnumerable<Expression> decorators)
        {
            DependencyType = dependencyType;
            Bindings = new ReadOnlyCollection<Binding>(bindings.ToArray());
            Decorators = new ReadOnlyCollection<Expression>(decorators.ToArray());
        }

        public ReadonlyRegistration(Type dependencyType, Binding binding, IEnumerable<Expression> decorators)
        {
            DependencyType = dependencyType;
            Bindings = new ReadOnlyCollection<Binding>(new[] { binding });
            Decorators = new ReadOnlyCollection<Expression>(decorators.ToArray());
        }
    }
}