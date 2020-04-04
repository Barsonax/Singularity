using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity.Resolving.Generators
{
    public class ArrayWrapperGenerator : IGenericWrapperGenerator
    {
        private static readonly MethodInfo CreateArrayMethod = typeof(ArrayWrapperGenerator).GetRuntimeMethods().Single(x => x.Name == nameof(CreateArray) && x.ContainsGenericParameters);

        public bool CanResolve(Type type) => type.IsArray && !(type.GetElementType().IsGenericType && type.GetElementType().GetGenericTypeDefinition() == typeof(Func<,>) && type.GetElementType().GetGenericArguments()[0] == typeof(Scoped));

        public Type? DependsOn(Type type) => typeof(Func<,>).MakeGenericType(typeof(Scoped), type).MakeArrayType();

        public Expression Wrap<TUnwrapped, TWrapped>(Expression expression, Type unWrappedType)
        {
            return Expression.Call(CreateArrayMethod.MakeGenericMethod(typeof(TUnwrapped)), Expression.Parameter(typeof(Scoped)), Expression.Parameter(typeof(Func<Scoped, TUnwrapped>[])));
        }

        private static TElement[] CreateArray<TElement>(Scoped scope, Func<Scoped, TElement>[] instanceFactories)
        {
            var list = new TElement[instanceFactories.Length];

            for (int i = 0; i < instanceFactories.Length; i++)
            {
                list[i] = instanceFactories[i].Invoke(scope);
            }

            return list;
        }
    }
}