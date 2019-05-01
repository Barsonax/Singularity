using System;
using Singularity.Collections;

namespace Singularity
{
    internal sealed class Registration
    {
        public Type DependencyType { get; }
        public ArrayList<ServiceBinding> Bindings { get; }
        public ServiceBinding Default { get; private set; }

        public Registration(Type dependencyType, ServiceBinding serviceBinding)
        {
            DependencyType = dependencyType;
            Default = serviceBinding;
            Bindings = new ArrayList<ServiceBinding>(new[] { serviceBinding });
        }

        public void AddBinding(ServiceBinding serviceBinding)
        {
            Default = serviceBinding;
            Bindings.Add(serviceBinding);
        }
    }
}