using System;
using System.Reflection;
using Singularity.Collections;
using Singularity.Graph;

namespace Singularity
{
    public class LateInjectorBinding
    {
        public Type InstanceType { get; }
        public ArrayList<MethodInfo> InjectionMethods { get; }
        public ArrayList<PropertyInfo> InjectionProperties { get; }
        public BindingMetadata BindingMetadata { get; }

        public LateInjectorBinding(Type instanceType, BindingMetadata bindingMetadata, ArrayList<MethodInfo> injectionMethods, ArrayList<PropertyInfo> injectionProperties)
        {
            InstanceType = instanceType;
            InjectionMethods = injectionMethods;
            InjectionProperties = injectionProperties;
            BindingMetadata = bindingMetadata;
        }
    }
}