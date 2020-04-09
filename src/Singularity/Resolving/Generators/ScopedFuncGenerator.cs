using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity.Resolving.Generators
{
    public class ScopedFuncGenerator : IGenericServiceGenerator
    {
        private static readonly MethodInfo GenericResolveMethod = typeof(ScopedFuncGenerator).GetRuntimeMethods().Single(x => x.Name == nameof(Resolve) && x.ContainsGenericParameters);

        public bool CanResolve(Type type)
        {
            return type.IsArray && type.GetElementType().IsGenericType && type.GetElementType().GetGenericTypeDefinition() == typeof(Func<,>) && type.GetElementType().GetGenericArguments()[0] == typeof(Scoped);
        }

        public ServiceBinding Resolve<TElement>(IInstanceFactoryResolver resolver)
        {
            Func<Scoped, TElement>[] instanceFactories = resolver.FindOrGenerateApplicableBindings(typeof(TElement))
                .Select(x => resolver.TryResolveDependency(typeof(TElement), x))
                .Where(x => x != null)
                .Select(x => (Func<Scoped, TElement>)((Scoped scoped) => (TElement)x!.Factory(scoped)!))
                .ToArray();
            var expression = Expression.Constant(instanceFactories);
            return new ServiceBinding(typeof(Func<Scoped, TElement>), BindingMetadata.GeneratedInstance, expression, typeof(Func<Scoped, TElement>), ConstructorResolvers.Default, Lifetimes.PerContainer);
        }

        public IEnumerable<ServiceBinding> Wrap<TTarget>(IInstanceFactoryResolver resolver)
        {
            var elementType = typeof(TTarget).GetGenericArguments()[1];
            MethodInfo resolveMethod = GenericResolveMethod.MakeGenericMethod(elementType);
            var binding = (ServiceBinding)resolveMethod.Invoke(this, new object[] { resolver });
            yield return binding;
        }
    }
}