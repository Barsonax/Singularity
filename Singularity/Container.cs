using Singularity.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Singularity.Bindings;
using Singularity.Graph;

namespace Singularity
{
    public class Container : IDisposable
    {
        private readonly DependencyGraph _dependencyGraph;

        private readonly Dictionary<Type, Action<object>> _injectionCache = new Dictionary<Type, Action<object>>(ReferenceEqualityComparer<Type>.Instance);
        private readonly Dictionary<Type, Func<object>> _getInstanceCache = new Dictionary<Type, Func<object>>(ReferenceEqualityComparer<Type>.Instance);

        public Container(IBindingConfig bindingConfig, DependencyGraph parentDependencies = null)
        {
            _dependencyGraph = new DependencyGraph(bindingConfig, parentDependencies);
        }

        public Container GetNestedContainer(BindingConfig bindingConfig)
        {
            return new Container(bindingConfig, _dependencyGraph);
        }

        /// <summary>
        /// Injects dependencies by calling all methods marked with <see cref="InjectAttribute"/> on the <paramref name="instances"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instances"></param>
        /// <exception cref="DependencyNotFoundException">If the method had parameters that couldnt be resolved</exception>
        public void MethodInjectAll<T>(IEnumerable<T> instances)
        {
            foreach (var instance in instances)
            {
                MethodInject(instance);
            }
        }

        /// <summary>
        /// Injects dependencies by calling all methods marked with <see cref="InjectAttribute"/> on the <paramref name="instance"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <exception cref="DependencyNotFoundException">If the method had parameters that couldnt be resolved</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MethodInject<T>(T instance)
        {
            GetMethodInjector(instance.GetType()).Invoke(instance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Action<object> GetMethodInjector(Type type)
        {
            if (!_injectionCache.TryGetValue(type, out var action))
            {
                action = GenerateMethodInjector(type);
                _injectionCache.Add(type, action);
            }
            return action;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Action<object> GetMethodInjector<T>()
        {
            var type = typeof(T);
            if (!_injectionCache.TryGetValue(type, out var action))
            {
                action = GenerateMethodInjector(type);
                _injectionCache.Add(type, action);
            }
            return action;
        }

        /// <summary>
        /// Resolves a instance for the given dependency type
        /// </summary>
        /// <typeparam name="T">The type of the dependency</typeparam>
        /// <exception cref="DependencyNotFoundException">If the dependency is not configured</exception>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Func<T> GetInstanceFactory<T>() where T : class
        {
            var type = typeof(T);
            if (!_getInstanceCache.TryGetValue(type, out var action))
            {
                action = GenerateInstanceFactory<T>();
                _getInstanceCache.Add(type, action);
            }
            return (Func<T>)action;
        }

        /// <summary>
        /// Resolves a instance for the given dependency type
        /// </summary>
        /// <param name="type">The type of the dependency</param>
        /// <exception cref="DependencyNotFoundException">If the dependency is not configured</exception>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Func<object> GetInstanceFactory(Type type)
        {
            if (!_getInstanceCache.TryGetValue(type, out var action))
            {
                action = GenerateInstanceFactory(type);
                _getInstanceCache.Add(type, action);
            }
            return action;
        }

        /// <summary>
        /// Resolves a instance for the given dependency type
        /// </summary>
        /// <typeparam name="T">The type of the dependency</typeparam>
        /// <exception cref="DependencyNotFoundException">If the dependency is not configured</exception>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetInstance<T>() where T : class
        {
            return GetInstanceFactory<T>().Invoke();
        }

        /// <summary>
        /// Resolves a instance for the given dependency type
        /// </summary>
        /// <param name="type">The type of the dependency</param>
        /// <exception cref="DependencyNotFoundException">If the dependency is not configured</exception>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object GetInstance(Type type)
        {
            return GetInstanceFactory(type).Invoke();
        }

        private Func<object> GenerateInstanceFactory(Type type)
        {
            var expression = GetDependencyExpression(type);
            return (Func<object>)Expression.Lambda(expression).Compile();
        }

        private Func<T> GenerateInstanceFactory<T>()
        {
            var expression = GetDependencyExpression(typeof(T));
            return Expression.Lambda<Func<T>>(expression).Compile();
        }

        private Expression GetDependencyExpression(Type type)
        {
            if (_dependencyGraph.Dependencies.TryGetValue(type, out var dependencyNode))
            {
                return dependencyNode.ResolvedDependency.Expression;
            }

            throw new DependencyNotFoundException(type);
        }

        private Action<object> GenerateMethodInjector(Type type)
        {
            var instanceParameter = Expression.Parameter(typeof(object));

            var body = new List<Expression>();
            var instanceCasted = Expression.Variable(type, "instanceCasted");
            body.Add(Expression.Assign(instanceCasted, Expression.Convert(instanceParameter, type)));
            foreach (var methodInfo in type.GetRuntimeMethods())
            {
                if (methodInfo.CustomAttributes.All(x => x.AttributeType != typeof(InjectAttribute))) continue;
                var parameterTypes = methodInfo.GetParameters();
                var parameterExpressions = new Expression[parameterTypes.Length];
                for (var i = 0; i < parameterTypes.Length; i++)
                {
                    var parameterType = parameterTypes[i].ParameterType;
                    parameterExpressions[i] = GetDependencyExpression(parameterType);
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
            _dependencyGraph.Dispose();
        }
    }
}