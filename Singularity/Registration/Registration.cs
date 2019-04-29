using System;
using Singularity.Collections;

namespace Singularity
{
    internal class Registration
    {
        public Type DependencyType { get; }
        public ArrayList<Binding> Bindings { get; }
        public Binding Default { get; private set; }

        public Registration(Type dependencyType, Binding binding)
        {
            DependencyType = dependencyType;
            Default = binding;
            Bindings = new ArrayList<Binding>(new[] { binding });
        }

        public void AddBinding(Binding binding)
        {
            Default = binding;
            Bindings.Add(binding);
        }
    }
}