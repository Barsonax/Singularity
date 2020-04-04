using System;
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

        public Type? DependsOn(Type type)
        {
            return null;
        }

        public Expression Wrap(IInstanceFactoryResolver resolver, Type targetType)
        {
            var elementType = targetType.GetGenericArguments()[1];
            MethodInfo resolveMethod = GenericResolveMethod.MakeGenericMethod(elementType);
            return (Expression)resolveMethod.Invoke(this, new object[] { resolver });
        }

        public Expression Resolve<TElement>(IInstanceFactoryResolver resolver)
        {
            Func<Scoped, TElement>[] instanceFactories = resolver.FindApplicableBindings(typeof(TElement))
                .Select(x => resolver.TryResolveDependency(typeof(TElement), x))
                .Where(x => x != null)
                .Select(x => (Func<Scoped, TElement>)((Scoped scoped) => (TElement)x!.Factory(scoped)!))
                .ToArray();
            return Expression.Constant(instanceFactories);
        }
    }
}