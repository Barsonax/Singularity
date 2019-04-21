using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Singularity.Bindings
{
    internal class ReadonlyRegistration
    {
        public Type[] DependencyTypes { get; }
        public ReadOnlyCollection<Binding> Bindings { get; }

        public ReadonlyRegistration(Type[] dependencyTypes, IEnumerable<Binding> bindings)
        {
            DependencyTypes = dependencyTypes;
            Bindings = new ReadOnlyCollection<Binding>(bindings.ToArray());
        }

        public ReadonlyRegistration(Type[] dependencyTypes, Binding binding)
        {
            DependencyTypes = dependencyTypes;
            Bindings = new ReadOnlyCollection<Binding>(new[] { binding });
        }
    }
}