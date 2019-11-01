using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Singularity.Collections;
using Singularity.Expressions;
using Singularity.Graph.Resolvers;

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
        internal RegistrationStore Registrations { get; }
        internal SingularitySettings Settings { get; }

        internal readonly Scoped ContainerScope;
        private readonly ResolverPipeline _dependencyGraph;
        private readonly ThreadSafeDictionary<Type, Action<Scoped, object>?> _injectionCache = new ThreadSafeDictionary<Type, Action<Scoped, object>?>();
        private readonly ThreadSafeDictionary<Type, Func<Scoped, object?>?> _getInstanceCache = new ThreadSafeDictionary<Type, Func<Scoped, object?>?>();
        private readonly Container? _parentContainer;

        /// <summary>
        /// Creates a new container using all the bindings that are in the provided modules
        /// </summary>
        /// <param name="modules"></param>
        public Container(IEnumerable<IModule> modules) : this(ToBuilder(modules)) { }

        /// <summary>
        /// Creates a new container using the provided configurator.
        /// </summary>
        /// <param name="configurator"></param>
        public Container(Action<ContainerBuilder>? configurator = null) : this(new ContainerBuilder(configurator))
        {
        }

        /// <summary>
        /// Creates a new container, consuming the provided builder.
        /// </summary>
        /// <param name="builder"></param>
        public Container(ContainerBuilder builder)
        {
            ContainerScope = new Scoped(this);
            Registrations = builder.Registrations;
            Settings = builder.Settings;
            _dependencyGraph = new ResolverPipeline(builder.Registrations, ContainerScope, Settings, null);
        }

        /// <summary>
        /// Creates a new child container using the provided configurator.
        /// </summary>
        /// <param name="parentContainer"></param>
        /// <param name="configurator"></param>
        private Container(Container parentContainer, Action<ContainerBuilder>? configurator) : this(parentContainer, new ContainerBuilder(configurator, parentContainer.Settings))
        {

        }

        /// <summary>
        /// Creates a new child container using the provided builder.
        /// </summary>
        /// <param name="parentContainer"></param>
        /// <param name="builder"></param>
        private Container(Container parentContainer, ContainerBuilder builder)
        {
            Settings = parentContainer.Settings;
            _parentContainer = parentContainer;
            ContainerScope = new Scoped(this);
            Registrations = builder.Registrations;
            _dependencyGraph = new ResolverPipeline(builder.Registrations, ContainerScope, Settings, parentContainer._dependencyGraph);
        }

        /// <summary>
        /// Creates a new nested container using all the bindings that are in the provided modules
        /// </summary>
        /// <param name="modules"></param>
        public Container GetNestedContainer(IEnumerable<IModule> modules) => GetNestedContainer(ToBuilder(modules));

        /// <summary>
        /// Creates a new nested container with the provided bindings.
        /// </summary>
        /// <param name="builder"></param>
        public Container GetNestedContainer(Action<ContainerBuilder>? builder = null) => new Container(this, builder);

        /// <summary>
        /// Starts a new scope
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Scoped BeginScope()
        {
            return new Scoped(this);
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetInstance<T>() where T : class => GetInstance<T>(ContainerScope);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T GetInstance<T>(Scoped scope) where T : class => (T)GetInstance(typeof(T), scope);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object GetInstance(Type type) => GetInstance(type, ContainerScope);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal object GetInstance(Type type, Scoped scope)
        {
            Func<Scoped, object>? func = _getInstanceCache.GetOrDefault(type);
            if (func == null)
            {
                func = _dependencyGraph.Resolve(type)?.Factory ?? (s => null);
                _getInstanceCache.Add(type, func);
            }
            return func(scope);
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T? GetInstanceOrDefault<T>() where T : class => GetInstanceOrDefault<T>(ContainerScope);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T? GetInstanceOrDefault<T>(Scoped scope) where T : class => (T?)GetInstanceOrDefault(typeof(T), scope);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? GetInstanceOrDefault(Type type) => GetInstanceOrDefault(type, ContainerScope);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal object? GetInstanceOrDefault(Type type, Scoped scope)
        {
            Func<Scoped, object?>? func = _getInstanceCache.GetOrDefault(type);
            if (func == null)
            {
                func = _dependencyGraph.TryResolve(type)?.Factory ?? (scoped => null);
                _getInstanceCache.Add(type, func);
            }
            return func(scope);
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LateInjectAll<T>(IEnumerable<T> instances)
        {
            LateInjectAll(instances, ContainerScope);
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LateInject(object instance) => LateInject(instance, ContainerScope);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void LateInjectAll<T>(IEnumerable<T> instances, Scoped scope)
        {
            foreach (T instance in instances)
            {
                if (instance != null) LateInject(instance, scope);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void LateInject(object instance, Scoped scope)
        {
            Type type = instance.GetType();
            Action<Scoped, object> action = _injectionCache.GetOrDefault(type);
            if (action == null)
            {
                action = GenerateLateInjector(type);
                _injectionCache.Add(type, action);
            }
            action(scope, instance);
        }

        private Action<Scoped, object> GenerateLateInjector(Type type)
        {
            if (Registrations.LateInjectorBindings.TryGetValue(type, out ArrayList<LateInjectorBinding> lateInjectorBindings))
            {
                ParameterExpression instanceParameter = Expression.Parameter(typeof(object));

                var body = new List<Expression>();

                Expression instanceCasted = Expression.Convert(instanceParameter, type);
                foreach (MethodInfo methodInfo in lateInjectorBindings.SelectMany(x => x.InjectionMethods))
                {
                    ParameterInfo[] parameterTypes = methodInfo.GetParameters();
                    var parameterExpressions = new Expression[parameterTypes.Length];
                    for (var i = 0; i < parameterTypes.Length; i++)
                    {
                        Type parameterType = parameterTypes[i].ParameterType;
                        parameterExpressions[i] = _dependencyGraph.Resolve(parameterType).Context.Expression;
                    }
                    body.Add(Expression.Call(instanceCasted, methodInfo, parameterExpressions));
                }

                foreach (MemberInfo memberInfo in lateInjectorBindings.SelectMany(x => x.InjectionProperties))
                {
                    MemberExpression memberAccessExpression = Expression.MakeMemberAccess(instanceCasted, memberInfo);
                    body.Add(Expression.Assign(memberAccessExpression, _dependencyGraph.Resolve(memberAccessExpression.Type).Context.Expression));
                }

                if (body.Count == 0) return (scope, instance) => { };
                Expression expression = body.Count == 1 ? body[0] : Expression.Block(body);
                Expression<Action<Scoped, object>> expressionTree = Expression.Lambda<Action<Scoped, object>>(expression, ExpressionGenerator.ScopeParameter, instanceParameter);

                Action<Scoped, object> action = expressionTree.Compile();

                return action;
            }
            else if (_parentContainer != null)
            {
                return _parentContainer.GenerateLateInjector(type);
            }

            return (scope, instance) => { };
        }

        /// <summary>
        /// Disposes the container.
        /// </summary>
		public void Dispose()
        {
            ContainerScope?.Dispose();
            IsDisposed = true;
        }

        private static Action<ContainerBuilder> ToBuilder(IEnumerable<IModule> modules)
        {
            if (modules == null) throw new ArgumentNullException(nameof(modules));
            return builder =>
            {
                foreach (IModule module in modules)
                {
                    builder.Registrations.CurrentModule = module;
                    module.Register(builder);
                }
            };
        }

        /// <inheritdoc />
        object IServiceProvider.GetService(Type serviceType)
        {
            return GetInstance(serviceType);
        }
    }
}