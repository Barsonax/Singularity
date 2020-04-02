using System;
using System.Linq.Expressions;

using Singularity.Expressions;

namespace Singularity
{
    /// <summary>
    /// A strongly typed configurator for registering new decorators.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TDecorator"></typeparam>
    public sealed class StronglyTypedDecoratorConfigurator<TService, TDecorator>
        where TService : class
        where TDecorator : TService
    {
        internal StronglyTypedDecoratorConfigurator(in BindingMetadata bindingMetadata, SingularitySettings settings)
        {
            _bindingMetadata = bindingMetadata;
            _settings = settings;
            DecoratorTypeValidator.CheckIsInterface(typeof(TService));
        }

        private readonly BindingMetadata _bindingMetadata;
        private readonly SingularitySettings _settings;
        private IConstructorResolver? _constructorSelector;

        /// <summary>
        /// Overrides the default constructor selector for this decorator
        /// <param name="value"></param>
        /// </summary>
        public StronglyTypedDecoratorConfigurator<TService, TDecorator> With(IConstructorResolver value)
        {
            _constructorSelector = value;
            return this;
        }

        internal Expression ToBinding()
        {
            var expression = (_constructorSelector ?? _settings.ConstructorResolver).ResolveConstructorExpression(typeof(TDecorator));
            DecoratorTypeValidator.CheckParameters(expression, typeof(TService), typeof(TDecorator));
            return expression;
        }
    }
}
