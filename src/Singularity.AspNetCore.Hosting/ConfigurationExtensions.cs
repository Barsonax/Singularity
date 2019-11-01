using Microsoft.Extensions.Hosting;

namespace Singularity
{
    public static class ConfigurationExtensions
    {
        public static IHostBuilder UseSingularity(this IHostBuilder builder, SingularitySettings? settings = null)
        {
            return builder.UseServiceProviderFactory(new SingularityServiceProviderFactory(settings));
        }
    }
}
