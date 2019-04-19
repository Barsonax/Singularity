using Singularity.Bindings;
using Singularity.Exceptions;

namespace Singularity
{
    public static class BindingExtensions
    {
        public static T With<T>(this T binding, Dispose dispose)
            where T : WeaklyTypedConfiguredBinding
        {
            binding.NeedsDispose = dispose;
            return binding;
        }

        /// <summary>
        /// Sets the lifetime of the instance(s)
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public static T With<T>(this T binding, Lifetime lifetime)
            where T : WeaklyTypedConfiguredBinding
        {
            if (!EnumMetadata<Lifetime>.IsValidValue(lifetime)) throw new InvalidLifetimeException(lifetime);
            binding.Lifetime = lifetime;
            return binding;
        }
    }
}