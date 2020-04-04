using System;
using System.Linq.Expressions;

namespace Singularity.Resolving.Generators
{
    public interface IGenericGenerator
    {
        bool CanResolve(Type type);
        Type? DependsOn(Type type);
    }

    public interface IGenericWrapperGenerator : IGenericGenerator
    {
        Expression Wrap<TUnwrapped, TWrapped>(Expression expression, Type unWrappedType);
    }

    public interface IGenericServiceGenerator: IGenericGenerator
    {
        Expression Wrap(IInstanceFactoryResolver resolver, Type targetType);
    }
}
