using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Singularity
{
    public static class ConfigurationExtensions
    {
        public static void SetupMvc(this ContainerBuilder builder)
        {
            builder.Register<IControllerActivator, SingularityControllerActivator>();
            builder.Register<IViewComponentActivator, SingularityViewActivator>();

            builder.ConfigureSettings(s =>
            {
                s.IgnoreResolveError(new PatternMatch("Microsoft.*"));
            });
        }
    }
}
