using Singularity.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;

namespace Singularity
{
    public class DependencyNode
    {
        public int? Depth { get; set; }
        public ParameterDependency[] Dependencies { get; }
        public Type DependencyType { get; }
		public Type ActualType { get; }
        public Expression Expression { get; }

        public DependencyNode(Type dependencyType, Expression expression)
        {
            DependencyType = dependencyType;
			ActualType = expression.Type;
            Expression = expression;
            Dependencies = GetParameterExpressions(expression);
            if (Dependencies.Length == 0) Depth = 0;
        }

        private ParameterDependency[] GetParameterExpressions(Expression expression)
        {
            switch (expression)
            {
                case ConstantExpression _:
                    return new ParameterDependency[0];
                case LambdaExpression lambdaExpression:
                    return lambdaExpression.Parameters.Select(x => new ParameterDependency(x)).ToArray();
                case NewExpression newExpression:
                    return newExpression.Arguments.Select(x => new ParameterDependency((ParameterExpression)x)).ToArray();
                default:
                    throw new NotSupportedException($"The expression of type {expression.GetType()} is not supported");
            }
        }

        public override string ToString()
        {
            return $"Depth {Depth} Type {DependencyType}";
        }
    }

    public class ParameterDependency
    {
        public DependencyNode Dependency { get; set; }
        public ParameterExpression Parameter { get; }

        public ParameterDependency(ParameterExpression parameter)
        {
            Parameter = parameter;
        }

        public override string ToString()
        {
            return Dependency.ToString();
        }
    }

    public class BindingConfig : IDisposable
    {
        public event EventHandler<IEnumerable<IBinding>> OnFinishBuildingDependencies;
        public readonly Dictionary<Type, IBinding> Bindings = new Dictionary<Type, IBinding>();

        public void Dispose()
        {
            ResolveDependencies();
            ValidateBindings();
            OnFinishBuildingDependencies?.Invoke(this, Bindings.Values);
        }

        private void ValidateBindings()
        {
            foreach (var binding in Bindings.Values)
            {
                if (binding.BindingExpression == null) throw new NullReferenceException();
            }
        }

        private void ResolveDependencies()
        {
            foreach (var dependency in Bindings.Values)
            {
                ResolveDependency(dependency);
            }
        }

        private void ResolveDependency(IBinding binding)
        {
            if (binding.IsResolved) return;
            Expression expression;
            switch (binding.BindingExpression)
            {
                case LambdaExpression lambdaExpression:
                    {
                        var body = ResolveMethodCallExpression(binding, lambdaExpression.Parameters);
                        body.Add(lambdaExpression.Body);
                        expression = Expression.Block(lambdaExpression.Parameters, body);
                    }
                    break;
                case NewExpression newExpression:
                    {
                        var body = ResolveMethodCallExpression(binding, newExpression.Arguments);
                        body.Add(newExpression);
                        expression = Expression.Block(newExpression.Arguments.Cast<ParameterExpression>(), body);
                    }
                    break;
                default:
                    throw new NotSupportedException($"The expression of type {binding.BindingExpression.GetType()} is not supported");
            }

            switch (binding.Lifetime)
            {
                case Lifetime.PerCall:
                    break;
                case Lifetime.PerContainer:
                    var action = Expression.Lambda(expression).Compile();
                    var value = action.DynamicInvoke();
                    expression = Expression.Constant(value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            binding.BindingExpression = expression;
            binding.IsResolved = true;
        }

        private List<Expression> ResolveMethodCallExpression(IBinding binding, IReadOnlyCollection<Expression> parameterExpressions)
        {
            var body = new List<Expression>();
            foreach (var unresolvedParameter in parameterExpressions)
            {
                if (Bindings.TryGetValue(unresolvedParameter.Type, out var dependency))
                {
                    if (!dependency.IsResolved)
                    {
                        ResolveDependency(dependency);
                    }
                    body.Add(Expression.Assign(unresolvedParameter, dependency.BindingExpression));
                }
                else
                {
                    throw new CannotResolveDependencyException($"Error while resolving internal dependencies for {binding.DependencyType}. A instance of type {unresolvedParameter} is needed but was not found in the container.");
                }
            }
            return body;
        }

        public Binding<TDependency> Bind<TDependency>()
        {
            if (!typeof(TDependency).GetTypeInfo().IsInterface) throw new InterfaceExpectedException($"{typeof(TDependency)} is not a interface.");
            var binding = new Binding<TDependency>();
            Bindings.Add(binding.DependencyType, binding);
            return binding;
        }
    }
}