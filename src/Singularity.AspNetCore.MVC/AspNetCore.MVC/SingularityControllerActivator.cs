using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Singularity.AspNetCore.MVC
{
    internal class SingularityControllerActivator : IControllerActivator
    {
        public object Create(ControllerContext context) => context.HttpContext.RequestServices.GetService(context.ActionDescriptor.ControllerTypeInfo.AsType());

        public void Release(ControllerContext context, object controller) { }
    }
}
