using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Singularity.AspNetCore.MVC;

namespace Singularity
{
    /// <summary>
    /// Configuration extensions for <see cref="Microsoft.AspNetCore.Mvc"/>
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Setups services and configuration for using Singularity with AspNetCore MVC
        /// </summary>
        /// <param name="builder"></param>
        public static void SetupMvc(this ContainerBuilder builder)
        {
            builder.Register<IControllerActivator, SingularityControllerActivator>();
            builder.Register<IViewComponentActivator, SingularityViewActivator>();
        }
    }
}
