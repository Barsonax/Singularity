using System;
using Singularity.Collections;

namespace Singularity.Bindings
{
    internal class ReadonlyRegistration
    {
        public Type[] DependencyTypes { get; }
        public ReadOnlyBindingCollection Bindings { get; }

        public ReadonlyRegistration(Type[] dependencyTypes, ReadOnlyBindingCollection bindings)
        {
            DependencyTypes = dependencyTypes;
            Bindings = bindings;
        }

        public ReadonlyRegistration(Type[] dependencyTypes, Binding binding)
        {
            DependencyTypes = dependencyTypes;
            Bindings = new ReadOnlyBindingCollection(new SinglyLinkedListNode<Binding>(null, binding));
        }
    }
}