using System;
using System.Reflection;
using Singularity.Collections;
using Singularity.Graph;

namespace Singularity
{
    internal sealed class LateInjectorBinding
    {
        public Type InstanceType { get; }
        public ArrayList<MethodInfo> InjectionMethods { get; }
        public ArrayList<MemberInfo> InjectionProperties { get; }
        public BindingMetadata BindingMetadata { get; }

        public LateInjectorBinding(Type instanceType, BindingMetadata bindingMetadata, ArrayList<MethodInfo> injectionMethods, ArrayList<MemberInfo> injectionProperties)
        {
            InstanceType = instanceType;
            InjectionMethods = injectionMethods;
            InjectionProperties = injectionProperties;
            BindingMetadata = bindingMetadata;
        }
    }
}