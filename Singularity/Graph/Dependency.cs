using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using Singularity.Bindings;
using Singularity.Collections;
using Singularity.Expressions;
using Singularity.FastExpressionCompiler;

namespace Singularity.Graph
{
    internal sealed class Dependency
    {
        public ReadonlyRegistration Registration { get; }
        public ArrayList<ResolvedDependency> ResolvedDependencies { get; }
        public ResolvedDependency Default { get; }

        public Dependency(ReadonlyRegistration registration)
        {
            Registration = registration ?? throw new ArgumentNullException(nameof(registration));
            ResolvedDependencies = new ArrayList<ResolvedDependency>();
            foreach (Binding binding in registration.Bindings)
            {
                ResolvedDependencies.Add(new ResolvedDependency(registration.DependencyTypes, binding));
            }

            Default = ResolvedDependencies.Array.LastOrDefault();
        }

        public Dependency(Type[] type, Expression expression, Lifetime lifetime) : this(new ReadonlyRegistration(type, new Binding(new BindingMetadata(type), expression, lifetime, null, DisposeBehavior.Default)))
        {

        }

        public Dependency(Type[] type, IEnumerable<Expression> expressions, Lifetime lifetime) :
            this(new ReadonlyRegistration(type, new ReadOnlyBindingCollection(expressions.Select(expression => new Binding(new BindingMetadata(type), expression, lifetime, null, DisposeBehavior.Default)))))
        {

        }
    }

    internal sealed class ResolvedDependency
    {
        public Type[] DependencyTypes { get; }
        public Binding Binding { get; }
        public Expression? BaseExpression { get; set; }
        public Exception? ResolveError { get; set; }
        public ArrayList<InstanceFactory> Factories { get; } = new ArrayList<InstanceFactory>();

        public bool TryGetInstanceFactory(Type type, out InstanceFactory factory)
        {
            factory = Factories.Array.FirstOrDefault(x => x.DependencyType == type);
            return factory != null;
        }


        public ResolvedDependency(Type[] dependencyTypes, Binding binding)
        {
            DependencyTypes = dependencyTypes;
            Binding = binding;
        }
    }

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