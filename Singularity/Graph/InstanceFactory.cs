using System;
using System.Linq.Expressions;
using Singularity.Expressions;
using Singularity.FastExpressionCompiler;

namespace Singularity.Graph
{
    internal sealed class InstanceFactory
    {
        public Type DependencyType { get; }
        public Expression Expression { get; }

        private Func<Scoped, object>? _factory;
        public Func<Scoped, object> Factory => _factory ??= (Func<Scoped, object>)Expression.Lambda(Expression, ExpressionGenerator.ScopeParameter).CompileFast();

        public InstanceFactory(Type dependencyType, Expression expression, Func<Scoped, object>? factory = null)
        {
            DependencyType = dependencyType;
            Expression = expression;
            _factory = factory;
        }
    }
}