using System;
using System.Linq.Expressions;
using Singularity.Graph;

namespace Singularity
{
    /// <summary>
    /// A strongly typed configurator for registering new decorators.
    /// </summary>
    /// <typeparam name="TDependency"></typeparam>
    /// <typeparam name="TDecorator"></typeparam>
    public sealed class StronglyTypedDecoratorConfigurator<TDependency, TDecorator>
        where TDependency : class
        where TDecorator : TDependency
    {
        internal StronglyTypedDecoratorConfigurator(in BindingMetadata bindingMetadata)
        {
            _bindingMetadata = bindingMetadata;
            _dependencyType = typeof(TDependency);
            _expression = AutoResolveConstructorExpressionCache<TDecorator>.Expression;
            DecoratorTypeValidator.CheckIsInterface(typeof(TDependency));
            DecoratorTypeValidator.CheckParameters(_expression, typeof(TDependency), typeof(TDecorator));
        }

        private readonly BindingMetadata _bindingMetadata;
        private readonly Type _dependencyType;
        private readonly Expression _expression;

        internal Expression ToBinding()
        {
            return _expression;
        }
    }
}
