using System.Linq;
using System.Reflection;
using Singularity.Collections;
using Singularity.Graph;

namespace Singularity
{
    public class StronglyTypedLateInjectorConfigurator<TInstance>
    {
        internal StronglyTypedLateInjectorConfigurator(BindingMetadata bindingMetadata)
        {
            _bindingMetadata = bindingMetadata;
        }

        private readonly BindingMetadata _bindingMetadata;
        private readonly ArrayList<MethodInfo> _injectionMethods = new ArrayList<MethodInfo>();
        private readonly ArrayList<PropertyInfo> _injectionProperties = new ArrayList<PropertyInfo>();

        public StronglyTypedLateInjectorConfigurator<TInstance> UseMethod(string methodName)
        {
            MethodInfo method = (from m in typeof(TInstance).GetRuntimeMethods()
                                 where m.IsPublic
                                 where m.Name == methodName
                                 select m).Single();
            return UseMethod(method);
        }

        public StronglyTypedLateInjectorConfigurator<TInstance> UseMethod(MethodInfo methodInfo)
        {
            _injectionMethods.Add(methodInfo);
            return this;
        }

        public StronglyTypedLateInjectorConfigurator<TInstance> UseProperty(string propertyName)
        {
            PropertyInfo method = (from m in typeof(TInstance).GetRuntimeProperties()
                                   where m.SetMethod?.IsPublic == true
                                   where m.Name == propertyName
                                   select m).Single();
            return UseProperty(method);
        }

        public StronglyTypedLateInjectorConfigurator<TInstance> UseProperty(PropertyInfo propertyInfo)
        {
            _injectionProperties.Add(propertyInfo);
            return this;
        }

        internal LateInjectorBinding ToBinding()
        {
            return new LateInjectorBinding(typeof(TInstance), _bindingMetadata, _injectionMethods, _injectionProperties);
        }
    }
}
