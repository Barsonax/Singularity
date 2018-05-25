using System;
using Singularity.Bindings;

namespace Singularity.Extensions
{
    public static class BindingConfigExtensions
    {
        public static void ValidateBindings(this IBindingConfig bindingConfig)
        {
            foreach (var binding in bindingConfig.Bindings.Values)
            {
                if (binding.Decorators == null) throw new NullReferenceException();
                if (binding.ConfiguredBinding?.Expression == null && binding.Decorators.Count == 0) throw new NullReferenceException();
            }
        }
    }
}
