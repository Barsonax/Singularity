using System;
using System.ComponentModel;
using Microsoft.Extensions.Hosting;
using Singularity.Microsoft.DependencyInjection;

namespace Singularity
{
    /// <summary>
    /// Configuration extensions for <see cref="Microsoft.Extensions.Hosting"/>
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
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
