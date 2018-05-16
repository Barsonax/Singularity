using Singularity.Bindings;

namespace Singularity
{
    public interface IModule
    {
	    void Register(BindingConfig bindingConfig);
    }
}
