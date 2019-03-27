using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Bindings;
using Singularity.Exceptions;
using Singularity.Expressions;

namespace Singularity.Graph
{
    internal sealed class DependencyGraph
    {
        private ReadOnlyDictionary<Type, Dependency> Dependencies { get; }
        private readonly Scoped _defaultScope;
        private static readonly MethodInfo _addMethod = typeof(Scoped).GetRuntimeMethod(nameof(Scoped.Add), new[] { typeof(object) });
        private readonly object _locker = new object();

        public DependencyGraph(ReadOnlyCollection<Binding> bindings, Scoped scope, DependencyGraph? parentDependencyGraph = null)
        {
            _defaultScope = scope;
            Dependencies = MergeBindings(bindings, parentDependencyGraph);
        }

        public bool TryGetDependency(Type type, out Dependency dependency)
        {
            if (Dependencies.TryGetValue(type, out dependency))
            {
                ResolveDependency(dependency);
                return true;
            }

            return false;
        }

        private void ResolveDependency(Dependency dependency)
        {
            lock (_locker)
            {
                if (dependency.Dependencies == null)
                {
                    dependency.Dependencies = GetDependencies(dependency.Binding, Dependencies);

                    foreach (var nestedDependency in dependency.Dependencies)
                    {
                        ResolveDependency(nestedDependency);
                    }

                    ResolveDependency(dependency, _defaultScope);
                }
            }
        }

        private static ReadOnlyDictionary<Type, Dependency> MergeBindings(ReadOnlyCollection<Binding> bindings, DependencyGraph? parentDependencyGraph)
        {
            var dependencies = new Dictionary<Type, Dependency>(bindings.Count);
            foreach (Binding binding in bindings)
            {
                dependencies.Add(binding.DependencyType, new Dependency(binding));
            }
            if (parentDependencyGraph != null)
            {
                MergeParentBindings(parentDependencyGraph, dependencies);
                return new ReadOnlyDictionary<Type, Dependency>(dependencies);
            }

            return new ReadOnlyDictionary<Type, Dependency>(dependencies);
        }

        private static void MergeParentBindings(DependencyGraph parentDependencyGraph, Dictionary<Type, Dependency> bindings)
        {
            foreach (Dependency parentBinding in parentDependencyGraph.Dependencies.Values)
            {
                if (bindings.TryGetValue(parentBinding.Binding.DependencyType, out Dependency childBinding))
                {
                    Expression[] decorators;
                    Expression? expression;
                    Action<object>? onDeathAction;
                    BindingMetadata bindingMetadata;
                    if (parentBinding.Binding.CreationMode == CreationMode.Singleton)
                    {
                        if (childBinding.Binding.Expression == null)
                        {
                            expression = parentBinding.ResolvedDependency.Expression;
                            bindingMetadata = parentBinding.Binding.BindingMetadata;
                            decorators = childBinding.Binding.Decorators; //The resolved expression already contains the decorators of the parent
                        }
                        else
                        {
                            bindingMetadata = childBinding.Binding.BindingMetadata;
                            expression = childBinding.Binding.Expression;
                            decorators = parentBinding.Binding.Decorators.Concat(childBinding.Binding.Decorators).ToArray();
                        }

                        onDeathAction = childBinding.Binding.OnDeathAction;
                    }
                    else
                    {
                        bindingMetadata = parentBinding.Binding.BindingMetadata;
                        expression = childBinding.Binding.Expression ?? parentBinding.Binding.Expression;
                        decorators = parentBinding.Binding.Decorators.Concat(childBinding.Binding.Decorators).ToArray();
                        onDeathAction = parentBinding.Binding.OnDeathAction;
                    }

                    var readonlyBinding = new Binding(bindingMetadata, childBinding.Binding.DependencyType, expression, parentBinding.Binding.CreationMode, decorators, onDeathAction);
                    bindings[childBinding.Binding.DependencyType] = new Dependency(readonlyBinding);
                }
                else
                {
                    if (parentBinding.Binding.CreationMode == CreationMode.Singleton)
                    {
                        var readonlyBinding = new Binding(
                            parentBinding.Binding.BindingMetadata,
                            parentBinding.Binding.DependencyType,
                            parentBinding.Binding.Expression,
                            parentBinding.Binding.CreationMode,
                            parentBinding.Binding.Decorators,
                            null);
                        bindings.Add(parentBinding.Binding.DependencyType, new Dependency(readonlyBinding));
                    }
                    else
                    {
                        bindings.Add(parentBinding.Binding.DependencyType, new Dependency(parentBinding.Binding));
                    }
                }
            }
        }

        private static Dependency[] GetDependencies(Binding unresolvedDependency, ReadOnlyDictionary<Type, Dependency> unresolvedDependencies)
        {
            if (unresolvedDependency.Expression == null) return new Dependency[0]; //TODO: probably a bug

            var resolvedDependencies = new List<Dependency>();
            if (unresolvedDependency.Expression.GetParameterExpressions()
                .TryExecute(dependencyType => { resolvedDependencies.Add(GetDependency(dependencyType.Type, unresolvedDependencies)); }, out IList<Exception> dependencyExceptions))
            {
                throw new SingularityAggregateException($"Could not find all dependencies for registered binding in {unresolvedDependency.BindingMetadata.GetPosition()}", dependencyExceptions);
            }

            if (unresolvedDependency.Decorators
                .SelectMany(x => x.GetParameterExpressions())
                .Where(x => x.Type != unresolvedDependency.DependencyType)
                .TryExecute(parameterExpression => { resolvedDependencies.Add(GetDependency(parameterExpression.Type, unresolvedDependencies)); }, out IList<Exception> decoratorExceptions))
            {
                throw new SingularityAggregateException($"Could not find all decorator dependencies for registered binding in {unresolvedDependency.BindingMetadata.GetPosition()}", decoratorExceptions);
            }

            return resolvedDependencies.ToArray();
        }

        private static TValue GetDependency<TValue>(Type type, IReadOnlyDictionary<Type, TValue> unresolvedDependencies)
        {
            if (unresolvedDependencies.TryGetValue(type, out TValue parent)) return parent;
            throw new DependencyNotFoundException(type);
        }

        private void ResolveDependency(Dependency dependency, Scoped scope)
        {

            if (dependency.ResolvedDependency == null)
            {
                Expression expression = GenerateDependencyExpression(dependency, scope);

                if (dependency.Binding.CreationMode is CreationMode.Singleton)
                {
                    Delegate action = Expression.Lambda(expression).Compile();
                    object value = action.DynamicInvoke();
                    expression = Expression.Constant(value, dependency.Binding.DependencyType);
                }

                dependency.ResolvedDependency = new ResolvedDependency(expression);
            }
        }

        private static Expression GenerateDependencyExpression(Dependency dependency, Scoped scope)
        {
            Expression expression = dependency.Binding.Expression! is LambdaExpression lambdaExpression ? lambdaExpression.Body : dependency.Binding.Expression;
            var body = new List<Expression>();
            var parameters = new List<ParameterExpression>();
            parameters.AddRange(expression.GetParameterExpressions());

            ParameterExpression instanceParameter = Expression.Variable(dependency.Binding.DependencyType, $"{expression.Type} instance");
            body.Add(Expression.Assign(instanceParameter, Expression.Convert(expression, dependency.Binding.DependencyType)));

            if (dependency.Binding.OnDeathAction != null)
            {
                scope.RegisterAction(dependency.Binding.Expression.Type, dependency.Binding.OnDeathAction);
                body.Add(Expression.Call(Expression.Constant(scope), _addMethod, instanceParameter));
            }

            if (dependency.Binding.Decorators.Length > 0)
            {
                Expression previousDecorator = instanceParameter;
                foreach (var decorator in dependency.Binding.Decorators)
                {
                    var visitor = new ReplaceExpressionVisitor(decorator.GetParameterExpressions().First(x => x.Type == instanceParameter.Type), previousDecorator);
                    Expression decoratorExpression = visitor.Visit(decorator);
                    parameters.AddRange(decoratorExpression.GetParameterExpressions().Where(parameterExpression => parameterExpression.Type != instanceParameter.Type));
                    previousDecorator = decoratorExpression;
                }
                body.Add(previousDecorator);
            }

            foreach (ParameterExpression unresolvedParameter in parameters)
            {
                var nestedDependency = dependency.Dependencies.First(x => x.Binding.DependencyType == unresolvedParameter.Type);
                body.Insert(0, Expression.Assign(unresolvedParameter, nestedDependency.ResolvedDependency!.Expression));
            }

            if (body.Last().Type == typeof(void)) body.Add(instanceParameter);
            return body.Count == 1 ? expression : Expression.Block(parameters.Concat(new[] { instanceParameter }), body);
        }
    }
}