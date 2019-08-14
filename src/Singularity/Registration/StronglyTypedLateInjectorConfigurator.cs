using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Collections;
using Singularity.Graph;

namespace Singularity
{
    /// <summary>
    /// A strongly typed configurator for configuring late injections such as method injection.
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    public sealed class StronglyTypedLateInjectorConfigurator<TInstance>
    {
        private readonly BindingMetadata _bindingMetadata;
        private readonly ArrayList<MethodInfo> _injectionMethods = new ArrayList<MethodInfo>();
        private readonly ArrayList<MemberInfo> _injectionMembers = new ArrayList<MemberInfo>();

        internal StronglyTypedLateInjectorConfigurator(BindingMetadata bindingMetadata)
        {
            _bindingMetadata = bindingMetadata;
        }

        /// <summary>
        /// Searches for a public method that matches the provided <paramref name="methodName"/> and registers it as a target for method injection.
        /// </summary>
        /// <param name="methodName">The name of the to be registered public method</param>
        /// <returns></returns>
        public StronglyTypedLateInjectorConfigurator<TInstance> UseMethod(string methodName)
        {
            MethodInfo method = (from m in typeof(TInstance).GetRuntimeMethods()
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
        public StronglyTypedLateInjectorConfigurator<TInstance> UseMethod(MethodInfo methodInfo)
        {
            _injectionMethods.Add(methodInfo);
            return this;
        }

        /// <summary>
        /// Registers a member for injection using a strongly typed expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public StronglyTypedLateInjectorConfigurator<TInstance> UseMember(Expression<Func<TInstance, object?>> expression)
        {
            switch (expression.Body)
            {
                case MemberExpression memberExpression:
                    switch (memberExpression.Member)
                    {
                        case PropertyInfo propertyInfo:
                            return UseProperty(propertyInfo);
                        case FieldInfo fieldInfo:
                            return UseField(fieldInfo);
                        default:
                            throw new NotSupportedException($"Expected a property or a field but got a {memberExpression.Member}");
                    }
                default:
                    throw new NotSupportedException($"Expected a member expression but got a {expression.Body}");
            }
        }

        /// <summary>
        /// Searches for a property with a public setter that matches the provided <paramref name="propertyName"/> and registers it as a target for member injection.
        /// </summary>
        /// <param name="propertyName">The name of the to be registered public property</param>
        /// <returns></returns>
        public StronglyTypedLateInjectorConfigurator<TInstance> UseProperty(string propertyName)
        {
            PropertyInfo propertyInfo = (from m in typeof(TInstance).GetRuntimeProperties()
                                         where m.SetMethod?.IsPublic == true
                                         where m.Name == propertyName
                                         select m).Single();
            return UseProperty(propertyInfo);
        }

        /// <summary>
        /// Searches for a public field that matches the provided <paramref name="fieldName"/> and registers it as a target for member injection.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public StronglyTypedLateInjectorConfigurator<TInstance> UseField(string fieldName)
        {
            FieldInfo fieldInfo = (from m in typeof(TInstance).GetRuntimeFields()
                                   where m.IsPublic
                                   where m.Name == fieldName
                                   select m).Single();
            return UseField(fieldInfo);
        }

        /// <summary>
        /// Registers the provided <paramref name="fieldInfo"/> as a target for member injection.
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public StronglyTypedLateInjectorConfigurator<TInstance> UseField(FieldInfo fieldInfo)
        {
            _injectionMembers.Add(fieldInfo);
            return this;
        }

        /// <summary>
        /// Registers the provided <paramref name="propertyInfo"/> as a target for member injection.
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public StronglyTypedLateInjectorConfigurator<TInstance> UseProperty(PropertyInfo propertyInfo)
        {
            _injectionMembers.Add(propertyInfo);
            return this;
        }

        internal LateInjectorBinding ToBinding()
        {
            return new LateInjectorBinding(typeof(TInstance), _bindingMetadata, _injectionMethods, _injectionMembers);
        }
    }
}
