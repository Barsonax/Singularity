using System;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace Singularity.Microsoft.DependencyInjection
{
    /// <summary>
    /// Extensions for microsoft dependency injection.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Registers the required services to support microsoft dependency injection.
        /// </summary>
        /// <param name="config"></param>
        public static void RegisterServiceProvider(this ContainerBuilder config)
        {
            config.Register<Container>(c => c.Inject(() => config.Container).With(ServiceAutoDispose.Never));
            config.Register<IServiceProvider, SingularityServiceProvider>(c => c.With(Lifetimes.PerContainer));
            config.Register<IServiceScopeFactory, SingularityServiceScopeFactory>(c => c.With(Lifetimes.PerContainer));
        }

        /// <summary>
        /// Registers all services that are container in the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="serviceCollection"></param>
        public static void RegisterServices(this ContainerBuilder config, IServiceCollection serviceCollection)
        {
            foreach (ServiceDescriptor registration in serviceCollection)
            {
                config.RegisterService(registration);
            }
        }

        /// <summary>
        /// Registers the <see cref="ServiceDescriptor"/>
        /// </summary>
        /// <param name="config"></param>
        /// <param name="registration"></param>
        public static void RegisterService(this ContainerBuilder config, ServiceDescriptor registration)
        {
            if (registration.ImplementationFactory != null)
            {
                ParameterExpression serviceProviderParameter = Expression.Parameter(typeof(IServiceProvider));
                config.Register(registration.ServiceType, c => c.Inject(
                    Expression.Lambda(
                            Expression.Convert(
                                    Expression.Invoke(
                                        Expression.Constant(registration.ImplementationFactory), serviceProviderParameter), registration.ServiceType), serviceProviderParameter))
                    .With(ConvertLifetime(registration.Lifetime)));
            }
            else if (registration.ImplementationInstance != null)
            {
                config.Register(registration.ServiceType, c => c.Inject(Expression.Constant(registration.ImplementationInstance)));
            }
            else
            {
                config.Register(registration.ServiceType, registration.ImplementationType, c => c.With(ConvertLifetime(registration.Lifetime)));
            }
        }

        private static ILifetime ConvertLifetime(ServiceLifetime serviceLifetime)
        {
            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton:
                    return Lifetimes.PerContainer;
                case ServiceLifetime.Scoped:
                    return Lifetimes.PerScope;
                case ServiceLifetime.Transient:
                    return Lifetimes.Transient;
                default:
                    throw new ArgumentOutOfRangeException(nameof(serviceLifetime), serviceLifetime, null);
            }
        }
    }
}