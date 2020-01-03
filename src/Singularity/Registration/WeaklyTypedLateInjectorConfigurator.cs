using System;
using System.Linq;
using System.Reflection;

using Singularity.Collections;

namespace Singularity
{
    /// <summary>
    /// A weakly typed configurator for configuring late injections such as method injection.
    /// </summary>
    public sealed class WeaklyTypedLateInjectorConfigurator
    {
        private readonly BindingMetadata _bindingMetadata;
        private readonly Type _instanceType;
        private readonly ArrayList<MethodInfo> _injectionMethods = new ArrayList<MethodInfo>();
        private readonly ArrayList<MemberInfo> _injectionProperties = new ArrayList<MemberInfo>();

        internal WeaklyTypedLateInjectorConfigurator(Type instanceType, in BindingMetadata bindingMetadata)
        {
            _bindingMetadata = bindingMetadata;
            _instanceType = instanceType;
        }

        /// <summary>
        /// Searches for a public method that matches the provided <paramref name="methodName"/> and registers it as a target for method injection.
        /// </summary>
        /// <param name="methodName">The name of the to be registered public method</param>
        /// <returns></returns>
        public WeaklyTypedLateInjectorConfigurator UseMethod(string methodName)
        {
            MethodInfo method = (from m in _instanceType.GetRuntimeMethods()
                                 where m.IsPublic
                                 where m.Name == methodName
                                 select m).Single();
            return UseMethod(method);
        }

        /// <summary>
        /// Registers the provided <paramref name="methodInfo"/> as a target for method injection.
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        public WeaklyTypedLateInjectorConfigurator UseMethod(MethodInfo methodInfo)
        {
            _injectionMethods.Add(methodInfo);
            return this;
        }

        /// <summary>
        /// Searches for a property with a public setter that matches the provided <paramref name="propertyName"/> and registers it as a target for property injection.
        /// </summary>
        /// <param name="propertyName">The name of the to be registered public property</param>
        /// <returns></returns>
        public WeaklyTypedLateInjectorConfigurator UseProperty(string propertyName)
        {
            PropertyInfo method = (from m in _instanceType.GetRuntimeProperties()
                                   where m.SetMethod?.IsPublic == true
                                   where m.Name == propertyName
                                   select m).Single();
            return UseProperty(method);
        }

        /// <summary>
        /// Registers the provided <paramref name="propertyInfo"/> as a target for property injection.
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public WeaklyTypedLateInjectorConfigurator UseProperty(PropertyInfo propertyInfo)
        {
            _injectionProperties.Add(propertyInfo);
            return this;
        }

        internal LateInjectorBinding ToBinding()
        {
            return new LateInjectorBinding(_instanceType, _bindingMetadata, _injectionMethods, _injectionProperties);
        }
    }
}