using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity.Resolving.Generators
{
    public class ListWrapperGenerator : IGenericWrapperGenerator
    {
        private static readonly MethodInfo CreateListMethod = typeof(ListWrapperGenerator).GetRuntimeMethods().Single(x => x.Name == nameof(CreateList) && x.ContainsGenericParameters);

        public bool CanResolve(Type type) => type.IsGenericType && new[] { typeof(List<>), typeof(IList<>), typeof(ICollection<>), }.Contains(type.GetGenericTypeDefinition());

        public Type? DependsOn(Type type) => typeof(Func<,>).MakeGenericType(typeof(Scoped), type).MakeArrayType();

        public Expression Wrap<TUnwrapped, TWrapped>(Expression expression, Type unWrappedType)
        {
            return Expression.Call(CreateListMethod.MakeGenericMethod(typeof(TUnwrapped)), Expression.Parameter(typeof(Scoped)), Expression.Parameter(typeof(Func<Scoped, TUnwrapped>[])));
        }

        private static List<TElement> CreateList<TElement>(Scoped scope, Func<Scoped, TElement>[] instanceFactories)
        {
            var list = new List<TElement>(instanceFactories.Length);

            foreach (Func<Scoped, TElement> instanceFactory in instanceFactories)
            {
                list.Add(instanceFactory.Invoke(scope));
            }

            return list;
        }
    }
}