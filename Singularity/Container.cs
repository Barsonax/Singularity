using Singularity.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Singularity.Attributes;
using Singularity.Collections;
using Singularity.Graph;

namespace Singularity
{
    /// <summary>
    /// A thread safe and lock free dependency injection container.
    /// </summary>
	public sealed class Container : IDisposable
    {
        /// <summary>
        /// Is the container disposed or not?
        /// </summary>
		public bool IsDisposed { get; private set; }
        private readonly DependencyGraph _dependencyGraph;
        private readonly ThreadSafeDictionary<Type, Action<object>> _injectionCache = new ThreadSafeDictionary<Type, Action<object>>();
        private readonly ThreadSafeDictionary<Type, Func<object>> _getInstanceCache = new ThreadSafeDictionary<Type, Func<object>>();
        private readonly Scoped _containerScope = new Scoped();

        /// <summary>
        /// Creates a new container using all the bindings that are in the provided modules
        /// </summary>
        /// <param name="modules"></param>
		public Container(IEnumerable<IModule> modules) : this(modules.ToBindings(), null)
        {

        }

        public Container(BindingConfig bindings) : this(bindings, null)
        {

        }

        private Container(BindingConfig bindings, DependencyGraph? parentDependencyGraph)
        {
            _dependencyGraph = new DependencyGraph(bindings.GetDependencies(), _containerScope, parentDependencyGraph);
        }

        /// <summary>
        /// Creates a new nested container using all the bindings that are in the provided modules
        /// </summary>
        /// <param name="modules"></param>
        public Container GetNestedContainer(IEnumerable<IModule> modules) => GetNestedContainer(modules.ToBindings());

        /// <summary>
        /// Creates a new nested container with the provided bindings.
        /// </summary>
        /// <param name="bindings"></param>
        public Container GetNestedContainer(BindingConfig bindings) => new Container(bindings, _dependencyGraph);

        /// <summary>
        /// Injects dependencies by calling all methods marked with <see cref="InjectAttribute"/> on the <paramref name="instances"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instances"></param>
        /// <exception cref="DependencyNotFoundException">If the method had parameters that couldn't be resolved</exception>
        public void MethodInjectAll<T>(IEnumerable<T> instances)
        {
            foreach (T instance in instances)
            {
                if (instance != null) MethodInject(instance);
            }
        }

        /// <summary>
        /// Injects dependencies by calling all methods marked with <see cref="InjectAttribute"/> on the <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance"></param>
        /// <exception cref="DependencyNotFoundException">If the method had parameters that couldn't be resolved</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MethodInject(object instance) => GetMethodInjector(instance.GetType()).Invoke(instance);

        /// <summary>
        /// Gets a action that can be used to inject dependencies through method injection
        /// <seealso cref="MethodInject(object)"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="throwError"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Action<object> GetMethodInjector(Type type, bool throwError = true)
        {
            Action<object> action = _injectionCache.Search(type);
            if (action == null)
            {
                action = GenerateMethodInjector(type, throwError);
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
        public Func<T> GetInstanceFactory<T>() where T : class => (Func<T>)GetInstanceFactory(typeof(T));

        /// <summary>
        /// Resolves a instance for the given dependency type
        /// </summary>
        /// <param name="type">The type of the dependency</param>
        /// <param name="throwError"></param>
        /// <exception cref="DependencyNotFoundException">If the dependency is not configured</exception>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Func<object> GetInstanceFactory(Type type, bool throwError = true)
        {
            Func<object> func = _getInstanceCache.Search(type);
            if (func == null)
            {
                func = GenerateInstanceFactory(type, throwError);
                _getInstanceCache.Add(type, func);
            }
            return func;
        }

        /// <summary>
        /// Resolves a instance for the given dependency type
        /// </summary>
        /// <typeparam name="T">The type of the dependency</typeparam>
        /// <exception cref="DependencyNotFoundException">If the dependency is not configured</exception>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetInstance<T>(bool throwError = true) where T : class => (T)GetInstance(typeof(T), throwError);

        /// <summary>
        /// Resolves a instance for the given dependency type
        /// </summary>
        /// <param name="type">The type of the dependency</param>
        /// <param name="throwError"></param>
        /// <exception cref="DependencyNotFoundException">If the dependency is not configured</exception>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object GetInstance(Type type, bool throwError = true) => GetInstanceFactory(type, throwError).Invoke();

        private Func<object> GenerateInstanceFactory(Type type, bool throwError)
        {
            Expression expression = GetDependencyExpression(type, throwError);

            return (Func<object>)Expression.Lambda(expression).Compile();
        }

        public Expression GetDependencyExpression(Type type, bool throwError)
        {
            if (type.GetTypeInfo().IsInterface)
            {
                if (_dependencyGraph.TryGetDependency(type, out Dependency dependencyNode))
                {
                    return dependencyNode.ResolvedDependency.Expression;
                }

                if (throwError)
                {
                    throw new DependencyNotFoundException(type);
                }
                else
                {
                    return Expression.Default(type);
                }
            }

            return GenerateConstructorInjector(type, throwError);
        }

        private Expression GenerateConstructorInjector(Type type, bool throwError)
        {
            ConstructorInfo constructor = type.AutoResolveConstructor();
            ParameterInfo[] parameters = constructor.GetParameters();

            var arguments = new Expression[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                Expression dependencyExpression = GetDependencyExpression(parameters[i].ParameterType, throwError);
                arguments[i] = dependencyExpression;
            }

            return Expression.New(constructor, arguments);
        }

        private Action<object> GenerateMethodInjector(Type type, bool throwError)
        {
            ParameterExpression instanceParameter = Expression.Parameter(typeof(object));

            var body = new List<Expression>();
            ParameterExpression instanceCasted = Expression.Variable(type, "instanceCasted");
            body.Add(Expression.Assign(instanceCasted, Expression.Convert(instanceParameter, type)));
            foreach (MethodInfo methodInfo in type.GetRuntimeMethods())
            {
                if (methodInfo.CustomAttributes.All(x => x.AttributeType != typeof(InjectAttribute))) continue;
                ParameterInfo[] parameterTypes = methodInfo.GetParameters();
                var parameterExpressions = new Expression[parameterTypes.Length];
                for (var i = 0; i < parameterTypes.Length; i++)
                {
                    Type parameterType = parameterTypes[i].ParameterType;
                    parameterExpressions[i] = GetDependencyExpression(parameterType, throwError);
                }
                body.Add(Expression.Call(instanceCasted, methodInfo, parameterExpressions));
            }
            BlockExpression block = Expression.Block(new[] { instanceCasted }, body);
            Expression<Action<object>> expressionTree = Expression.Lambda<Action<object>>(block, instanceParameter);

            Action<object> action = expressionTree.Compile();

            return action;
        }

        /// <summary>
        /// Disposes the container.
        /// </summary>
		public void Dispose()
        {
            _containerScope.Dispose();
            IsDisposed = true;
        }
    }
}