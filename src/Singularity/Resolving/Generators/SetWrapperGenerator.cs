using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity.Resolving.Generators
{
    public class SetWrapperGenerator : IGenericWrapperGenerator
    {
        private static readonly MethodInfo CreateListMethod = typeof(SetWrapperGenerator).GetRuntimeMethods().Single(x => x.Name == nameof(CreateSet) && x.ContainsGenericParameters);

        public bool CanResolve(Type type) => type.IsGenericType && new[] { typeof(HashSet<>), typeof(ISet<>), }.Contains(type.GetGenericTypeDefinition());

        public Type? DependsOn(Type type) => typeof(Func<,>).MakeGenericType(typeof(Scoped), type).MakeArrayType();

        public Expression Wrap<TUnwrapped, TWrapped>(Expression expression, Type unWrappedType)
        {
            return Expression.Call(CreateListMethod.MakeGenericMethod(typeof(TUnwrapped)), Expression.Parameter(typeof(Scoped)), Expression.Parameter(typeof(Func<Scoped, TUnwrapped>[])));
        }

        private static HashSet<TElement> CreateSet<TElement>(Scoped scope, Func<Scoped, TElement>[] instanceFactories)
        {
            var list = new HashSet<TElement>();

            foreach (Func<Scoped, TElement> instanceFactory in instanceFactories)
            {
                list.Add(instanceFactory.Invoke(scope));
            }

            return list;
        }
    }
}