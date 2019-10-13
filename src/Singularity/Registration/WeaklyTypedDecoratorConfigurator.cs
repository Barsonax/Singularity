using System;
using System.Linq.Expressions;
using Singularity.Graph;

namespace Singularity
{
    /// <summary>
    /// A weakly typed configurator for registering new decorators.
    /// </summary>
    public sealed class WeaklyTypedDecoratorConfigurator
    {
        internal WeaklyTypedDecoratorConfigurator(Type dependencyType, Type decoratorType, in BindingMetadata bindingMetadata)
        {
            _bindingMetadata = bindingMetadata;
            _dependencyType = dependencyType;
            _expression = decoratorType.AutoResolveConstructorExpression();
            DecoratorTypeValidator.CheckIsInterface(dependencyType);
            DecoratorTypeValidator.CheckParameters(_expression, dependencyType, decoratorType);
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
