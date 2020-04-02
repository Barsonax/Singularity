using System;
using System.Linq.Expressions;

using Singularity.Expressions;

namespace Singularity
{
    /// <summary>
    /// A weakly typed configurator for registering new decorators.
    /// </summary>
    public sealed class WeaklyTypedDecoratorConfigurator
    {
        internal WeaklyTypedDecoratorConfigurator(Type serviceType, Type decoratorType, in BindingMetadata bindingMetadata, SingularitySettings settings)
        {
            _bindingMetadata = bindingMetadata;
            _serviceType = serviceType;
            _settings = settings;
            _decoratorType = decoratorType;
            DecoratorTypeValidator.CheckIsInterface(serviceType);
        }

        private readonly BindingMetadata _bindingMetadata;
        private readonly Type _serviceType;
        private readonly Type _decoratorType;
        private readonly SingularitySettings _settings;
        private IConstructorResolver? _constructorSelector;

        /// <summary>
        /// Overrides the default constructor selector for this decorator
        /// <param name="value"></param>
        /// </summary>
        public WeaklyTypedDecoratorConfigurator With(IConstructorResolver value)
        {
            _constructorSelector = value;
            return this;
        }

        internal Expression ToBinding()
        {
            var expression = (_constructorSelector ?? _settings.ConstructorResolver).ResolveConstructorExpression(_decoratorType);
            DecoratorTypeValidator.CheckParameters(expression, _serviceType, _decoratorType);
            return expression;
        }
    }
}
