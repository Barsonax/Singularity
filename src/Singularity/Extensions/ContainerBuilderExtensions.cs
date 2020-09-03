using System;

namespace Singularity
{
    /// <summary>
    /// Extensions for <see cref="ContainerBuilder"/>.
    /// </summary>
    public static class ContainerBuilderExtensions
    {

        /// <summary>
        /// Registers services defined in an <see cref="IModule"/>.
        /// </summary>
        /// <typeparam name="TModule"></typeparam>
        public static void RegisterModule<TModule>(this ContainerBuilder containerBuilder)
            where TModule : IModule, new()
        {
            containerBuilder.RegisterModule(new TModule());
        }
    }
}