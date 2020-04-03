using System;
using System.Linq.Expressions;

namespace Singularity.Resolving.Generators
{
    public interface IGenericWrapperGenerator
    {
        bool CanResolve(Type type);
        Expression Wrap(IInstanceFactoryResolver resolver, Expression expression, Type unWrappedType, Type wrappedType);
        Type? DependsOn(Type type);
    }
}
