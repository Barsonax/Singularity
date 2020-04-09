using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Singularity.Resolving.Generators
{
    public interface IGenericGenerator
    {
        bool CanResolve(Type type);
    }

    public interface IGenericWrapperGenerator : IGenericGenerator
    {
        Expression Wrap<TUnwrapped, TWrapped>();
        Type? DependsOn(Type type);
    }

    public interface IGenericServiceGenerator: IGenericGenerator
    {
        IEnumerable<ServiceBinding> Wrap<TTarget>(IInstanceFactoryResolver resolver);
    }
}
