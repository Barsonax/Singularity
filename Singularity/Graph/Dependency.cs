using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Singularity.Graph
{
    internal sealed class Dependency
    {
        public Binding Binding { get; }
        public ReadOnlyCollection<Expression> Decorators { get; }
        public ReadOnlyCollection<ParameterExpression> DecoratorParameters { get; }

        public Dependency[]? Children { get; set; }
        public Dependency[]? Parents { get; set; }
        public Expression? Expression { get; set; }
        public Func<Scoped, object>? InstanceFactory { get; set; }
        public Exception ResolveError { get; set; }

        private static readonly ReadOnlyCollection<ParameterExpression> EmptyParameters = new ReadOnlyCollection<ParameterExpression>(new ParameterExpression[0]);

        public Dependency(Binding unresolvedDependency, ReadOnlyCollection<Expression> decorators)
        {
            Binding = unresolvedDependency;

            Decorators = decorators ?? throw new ArgumentNullException(nameof(decorators));
            DecoratorParameters = decorators.Count != 0
                ? new ReadOnlyCollection<ParameterExpression>(
                    Decorators.SelectMany(x => x.GetParameterExpressions())
                        .Where(x => x.Type != Binding.DependencyType).ToArray())
                : EmptyParameters;
        }
    }
}