using System;
using System.Linq.Expressions;

namespace Singularity.Resolving.Generators
{
    public interface IGenericWrapperGenerator
    {
        bool CanResolve(Type type);
        Expression Wrap(Expression expression, Type unWrappedType, Type wrappedType);
        Type Target(Type type);

        Type? DependsOn(Type type);
    }
}
