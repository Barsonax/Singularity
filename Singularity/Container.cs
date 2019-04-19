using Singularity.Exceptions;
using System;
using System.Collections.Generic;
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
	public sealed class Container : IContainer
    {
        /// <summary>
        /// Is the container disposed or not?
        /// </summary>
		public bool IsDisposed { get; private set; }
        private readonly DependencyGraph _dependencyGraph;
        private readonly ThreadSafeDictionary<Type, Action<Scoped, object>> _injectionCache = new ThreadSafeDictionary<Type, Action<Scoped, object>>();
        private readonly ThreadSafeDictionary<Type, Func<Scoped, object>> _getInstanceCache = new ThreadSafeDictionary<Type, Func<Scoped, object>>();
        private readonly Scoped _containerScope;
        private readonly SingularitySettings _options;

        /// <summary>
        /// Creates a new container using all the bindings that are in the provided modules
        /// </summary>
        /// <param name="modules"></param>
		public Container(IEnumerable<IModule> modules) : this(modules.ToBindings()) { }

        /// <summary>
        /// Creates a new container with the provided bindings.
        /// </summary>
        /// <param name="bindings"></param>
        /// <param name="options"></param>
        public Container(BindingConfig bindings, SingularitySettings? options = null)
        {
            if (bindings == null) throw new ArgumentNullException(nameof(bindings));
            _options = options ?? SingularitySettings.Default;
            _containerScope = new Scoped(this);
            _dependencyGraph = new DependencyGraph(bindings.GetDependencies(), _containerScope, _options);
        }

        private Container(BindingConfig bindings, Container parentContainer)
        {
            if (bindings == null) throw new ArgumentNullException(nameof(bindings));
            _options = parentContainer._options;
            _containerScope = new Scoped(this);
            _dependencyGraph = new DependencyGraph(bindings.GetDependencies(), _containerScope, _options, parentContainer._dependencyGraph);
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
        public Container GetNestedContainer(BindingConfig bindings) => new Container(bindings, this);

        /// <summary>
        /// Starts a new scope
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Scoped BeginScope()
        {
            return new Scoped(this);
        }

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
        /// Resolves a instance for the given dependency type
        /// </summary>
        /// <typeparam name="T">The type of the dependency</typeparam>
        /// <exception cref="DependencyNotFoundException">If the dependency is not configured</exception>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetInstance<T>() where T : class => GetInstance<T>(_containerScope);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal T GetInstance<T>(Scoped scope) where T : class => (T)GetInstance(typeof(T), scope);

        /// <summary>
        /// Resolves a instance for the given dependency type
        /// </summary>
        /// <param name="type">The type of the dependency</param>
        /// <exception cref="DependencyNotFoundException">If the dependency is not configured</exception>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object GetInstance(Type type) => GetInstance(type, _containerScope);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal object GetInstance(Type type, Scoped scope)
        {
            Func<Scoped, object> func = _getInstanceCache.Search(type);
            if (func == null)
            {
                func = _dependencyGraph.GetResolvedFactory(type)!;
                _getInstanceCache.Add(type, func);
            }
            return func(scope);
        }

        /// <summary>
        /// Injects dependencies by calling all methods marked with <see cref="InjectAttribute"/> on the <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance"></param>
        /// <exception cref="DependencyNotFoundException">If the method had parameters that couldn't be resolved</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MethodInject(object instance) => MethodInject(instance, _containerScope);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void MethodInject(object instance, Scoped scope)
        {
            Type type = instance.GetType();
            Action<Scoped, object> action = _injectionCache.Search(type);
            if (action == null)
            {
                action = GenerateMethodInjector(type);
                _injectionCache.Add(type, action);
            }
            action(scope, instance);
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
            Expression<Action<Scoped, object>> expressionTree = Expression.Lambda<Action<Scoped, object>>(block, ExpressionGenerator.ScopeParameter, instanceParameter);

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