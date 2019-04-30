using System;
using System.Linq.Expressions;

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
        internal StronglyTypedDecoratorConfigurator(string callerFilePath, int callerLineNumber, IModule? module = null)
        {
            _module = module;
            _callerFilePath = callerFilePath;
            _callerLineNumber = callerLineNumber;
            _dependencyType = typeof(TDependency);
            _expression = AutoResolveConstructorExpressionCache<TDecorator>.Expression;
            DecoratorTypeValidator.CheckIsInterface(typeof(TDependency));
            DecoratorTypeValidator.CheckParameters(_expression, typeof(TDependency), typeof(TDecorator));
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
