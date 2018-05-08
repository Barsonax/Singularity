using Singularity.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity
{
    public class Container : IDisposable
    {
        private readonly DependencyGraph _dependencyGraph;
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        private readonly Dictionary<Type, Action<object>> _injectionCache = new Dictionary<Type, Action<object>>(ReferenceEqualityComparer<Type>.Instance);
        private readonly Dictionary<Type, Func<object>> _getInstanceCache = new Dictionary<Type, Func<object>>(ReferenceEqualityComparer<Type>.Instance);


        public Container(BindingConfig bindingConfig)
        {
            bindingConfig.ValidateBindings();
            _dependencyGraph = new DependencyGraph(bindingConfig);
            foreach (var node in _dependencyGraph.Dependencies.Values)
            {
                if (node.Lifetime == Lifetime.PerContainer)
                {
                    var value = ((ConstantExpression)node.Expression).Value;
                    if (value is IDisposable disposable)
                    {
                        _disposables.Add(disposable);
                    }
                }
            }
        }

        /// <summary>
        /// Injects dependencies by calling all methods marked with <see cref="InjectAttribute"/> on the <paramref name="instances"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instances"></param>
        /// <exception cref="DependencyNotFoundException">If the method had parameters that couldnt be resolved</exception>
        public void InjectAll<T>(IEnumerable<T> instances)
        {
            foreach (var instance in instances)
            {
                Inject(instance);
            }
        }

        /// <summary>
        /// Injects dependencies by calling all methods marked with <see cref="InjectAttribute"/> on the <paramref name="instance"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <exception cref="DependencyNotFoundException">If the method had parameters that couldnt be resolved</exception>
        public void Inject<T>(T instance)
        {
            var type = instance.GetType();
            if (!_injectionCache.TryGetValue(type, out var action))
            {
                action = GenerateInjectionExpression(type);
                _injectionCache.Add(type, action);
            }
            action.Invoke(instance);
        }

        /// <summary>
        /// Resolves a instance for the given dependency type
        /// </summary>
        /// <typeparam name="T">The type of the dependency</typeparam>
        /// <exception cref="DependencyNotFoundException">If the dependency is not configured</exception>
        /// <returns></returns>
        public T GetInstance<T>() where T : class
        {
            var value = GetInstance(typeof(T));
            return (T)value;
        }

        /// <summary>
        /// Resolves a instance for the given dependency type
        /// </summary>
        /// <param name="type">The type of the dependency</param>
        /// <exception cref="DependencyNotFoundException">If the dependency is not configured</exception>
        /// <returns></returns>
        public object GetInstance(Type type)
        {
            if (!_getInstanceCache.TryGetValue(type, out var action))
            {
                action = GenerateGetInstanceExpression(type);
                _getInstanceCache.Add(type, action);
            }
            var value = action.Invoke();
            return value;
        }

        private Func<object> GenerateGetInstanceExpression(Type type)
        {
            if (_dependencyGraph.Dependencies.TryGetValue(type, out var dependencyNode))
            {
                if (dependencyNode.Expression is ConstantExpression constantExpression)
                {
                    return () => constantExpression.Value;
                }
                return Expression.Lambda<Func<object>>(dependencyNode.Expression).Compile();
            }
            else
            {
                throw new DependencyNotFoundException($"No configured dependency found for {type}");
            }
        }

        private Action<object> GenerateInjectionExpression(Type type)
        {
            var instanceParameter = Expression.Parameter(typeof(object));

            var body = new List<Expression>();
            var instanceCasted = Expression.Variable(type, "instanceCasted");
            body.Add(instanceCasted);
            body.Add(Expression.Assign(instanceCasted, Expression.Convert(instanceParameter, type)));
            foreach (var methodInfo in type.GetRuntimeMethods())
            {
                if (methodInfo.CustomAttributes.All(x => x.AttributeType != typeof(InjectAttribute))) continue;
                var parameterTypes = methodInfo.GetParameters();
                var parameterExpressions = new Expression[parameterTypes.Length];
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    var parameterType = parameterTypes[i].ParameterType;
                    if (_dependencyGraph.Dependencies.TryGetValue(parameterType, out var dependencyNode))
                    {
                        parameterExpressions[i] = dependencyNode.Expression;
                    }
                    else
                    {
                        throw new DependencyNotFoundException($"No configured dependency found for {parameterType}");
                    }
                }

                body.Add(Expression.Call(instanceCasted, methodInfo, parameterExpressions));
            }
            var block = Expression.Block(new[] { instanceCasted }, body);
            var expressionTree = Expression.Lambda<Action<object>>(block, instanceParameter);

            var action = expressionTree.Compile();
            return action;
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}