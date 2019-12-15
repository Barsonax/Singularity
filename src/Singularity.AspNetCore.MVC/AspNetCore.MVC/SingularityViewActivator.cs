using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Singularity.AspNetCore.MVC
{
    internal class SingularityViewActivator : IViewComponentActivator
    {
        public object Create(ViewComponentContext context) => context.ViewContext.HttpContext.RequestServices.GetService(context.ViewComponentDescriptor.TypeInfo.AsType());

        public void Release(ViewComponentContext context, object viewComponent) { }
    }
}
