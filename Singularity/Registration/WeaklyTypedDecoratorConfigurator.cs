using System;
using System.Linq.Expressions;

namespace Singularity
{
    /// <summary>
    /// A weakly typed configurator for registering new decorators.
    /// </summary>
    public class WeaklyTypedDecoratorConfigurator
    {
        internal WeaklyTypedDecoratorConfigurator(Type dependencyType, Type decoratorType, string callerFilePath, int callerLineNumber, IModule? module = null)
        {
            _module = module;
            _callerFilePath = callerFilePath;
            _callerLineNumber = callerLineNumber;
            _dependencyType = dependencyType;
            _expression = decoratorType.AutoResolveConstructorExpression();
            DecoratorTypeValidator.CheckIsInterface(dependencyType);
            DecoratorTypeValidator.CheckParameters(_expression, dependencyType, decoratorType);
        }

        private IModule? _module;
        private string _callerFilePath;
        private int _callerLineNumber;
        private Type _dependencyType;
        private readonly Expression _expression;

        internal Expression ToBinding()
        {
            return _expression;
        }
    }
}
