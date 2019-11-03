using System;
using Microsoft.Extensions.Hosting;

namespace Singularity
{
    /// <summary>
    /// Configuration extensions for <see cref="Microsoft.Extensions.Hosting"/>
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Configures the <paramref name="builder"/> to use Singularity as <see cref="IServiceProvider"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static IHostBuilder UseSingularity(this IHostBuilder builder, SingularitySettings? settings = null)
        {
            return builder.UseServiceProviderFactory(new SingularityServiceProviderFactory(settings));
        }
    }
}
