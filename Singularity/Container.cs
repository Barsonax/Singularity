using Singularity.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Singularity.Attributes;
using Singularity.Collections;
using Singularity.Expressions;
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
        private readonly ThreadSafeDictionary<Type, Action<Scoped, object>> _injectionCache = new ThreadSafeDictionary<Type, Action<Scoped, object>>();
        private readonly ThreadSafeDictionary<Type, Func<Scoped, object>> _getInstanceCache = new ThreadSafeDictionary<Type, Func<Scoped, object>>();
        private readonly Container? _parentContainer;
        private readonly Scoped _effectiveScope;
        private readonly Scoped? _containerScope;

        /// <summary>
        /// Creates a new container using all the bindings that are in the provided modules
        /// </summary>
        /// <param name="modules"></param>
		public Container(IEnumerable<IModule> modules) : this(modules.ToBindings())
        {

        }

        /// <summary>
        /// Creates a new container with the provided bindings.
        /// </summary>
        /// <param name="bindings"></param>
        public Container(BindingConfig bindings)
        {
            if (bindings == null) throw new ArgumentNullException(nameof(bindings));
            _containerScope = new Scoped();
            _effectiveScope = FindScope();
            _dependencyGraph = new DependencyGraph(bindings.GetDependencies(), _containerScope);
        }

        private Container(BindingConfig bindings, Scoped? containerScope, Container parentContainer)
        {
            if (bindings == null) throw new ArgumentNullException(nameof(bindings));
            _parentContainer = parentContainer ?? throw new ArgumentNullException(nameof(parentContainer));
            _containerScope = containerScope;
            _effectiveScope = containerScope ?? FindScope();
            _dependencyGraph = new DependencyGraph(bindings.GetDependencies(), _effectiveScope ?? throw new ArgumentException(nameof(parentContainer._containerScope)), parentContainer._dependencyGraph);
        }

        private Scoped FindScope()
        {
            Container container = this;
            do
            {
                if (container._parentContainer == null) break;
                container = container._parentContainer;
            }
            while (container._containerScope == null);
            return container._containerScope;
        }

        /// <summary>
        /// Creates a new nested container using all the bindings that are in the provided modules
        /// </summary>
        /// <param name="modules"></param>
        /// <param name="containerScope"></param>
        public Container GetNestedContainer(IEnumerable<IModule> modules, Scoped? containerScope = null) => GetNestedContainer(modules.ToBindings(), containerScope);

        /// <summary>
        /// Creates a new nested container with the provided bindings.
        /// </summary>
        /// <param name="bindings"></param>
        /// <param name="containerScope"></param>
        public Container GetNestedContainer(BindingConfig bindings, Scoped? containerScope = null) => new Container(bindings, containerScope, this);

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
        public void MethodInject(object instance) => GetMethodInjector(instance.GetType()).Invoke(_effectiveScope, instance);

        /// <summary>
        /// Resolves a instance for the given dependency type
        /// </summary>
        /// <typeparam name="T">The type of the dependency</typeparam>
        /// <exception cref="DependencyNotFoundException">If the dependency is not configured</exception>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetInstance<T>() where T : class => (T)GetInstance(typeof(T));

        /// <summary>
        /// Resolves a instance for the given dependency type
        /// </summary>
        /// <param name="type">The type of the dependency</param>
        /// <exception cref="DependencyNotFoundException">If the dependency is not configured</exception>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object GetInstance(Type type) => GetInstanceFactory(type).Invoke(_effectiveScope);

        /// <summary>
        /// Resolves a instance for the given dependency type
        /// </summary>
        /// <param name="type">The type of the dependency</param>
        /// <exception cref="DependencyNotFoundException">If the dependency is not configured</exception>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Func<Scoped, object> GetInstanceFactory(Type type)
        {
            Func<Scoped, object> func = _getInstanceCache.Search(type);
            if (func == null)
            {
                func = _dependencyGraph.GetResolvedFactory(type)!;
                _getInstanceCache.Add(type, func);
            }
            return func;
        }

        /// <summary>
        /// Gets a action that can be used to inject dependencies through method injection
        /// <seealso cref="MethodInject(object)"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Action<Scoped, object> GetMethodInjector(Type type)
        {
            Action<Scoped, object> action = _injectionCache.Search(type);
            if (action == null)
            {
                action = GenerateMethodInjector(type);
                _injectionCache.Add(type, action);
            }
            return action;
        }

        private Action<Scoped, object> GenerateMethodInjector(Type type)
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
                    parameterExpressions[i] = _dependencyGraph.GetResolvedExpression(parameterType)!;
                }
                body.Add(Expression.Call(instanceCasted, methodInfo, parameterExpressions));
            }
            BlockExpression block = Expression.Block(new[] { instanceCasted }, body);
            var expressionTree = Expression.Lambda<Action<Scoped, object>>(block, ExpressionGenerator.ScopeParameter, instanceParameter);

            Action<Scoped, object> action = expressionTree.Compile();

            return action;
        }

        /// <summary>
        /// Disposes the container.
        /// </summary>
		public void Dispose()
        {
            _containerScope?.Dispose();
            IsDisposed = true;
        }
    }
}