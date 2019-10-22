using Singularity.Collections;

namespace Singularity
{
    internal readonly struct Registration
    {
        public ArrayList<ServiceBinding> Bindings { get; }
        public ServiceBinding Default => Bindings.Last();

        public Registration(ServiceBinding serviceBinding)
        {
            Bindings = new ArrayList<ServiceBinding>(new[] { serviceBinding });
        }

        public void AddBinding(ServiceBinding serviceBinding)
        {
            Bindings.Add(serviceBinding);
        }
    }
}