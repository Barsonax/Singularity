using System;
using Singularity.Expressions;

namespace Singularity.Graph
{
    internal sealed class InstanceFactory
    {
        public Type DependencyType { get; }
        public ReadOnlyExpressionContext Context { get; }

        private Func<Scoped, object>? _factory;
        public Func<Scoped, object> Factory => _factory ??= ExpressionCompiler.Compile(Context);

        public InstanceFactory(Type dependencyType, ReadOnlyExpressionContext context, Func<Scoped, object>? factory = null)
        {
            DependencyType = dependencyType;
            Context = context;
            _factory = factory;
        }
    }
}