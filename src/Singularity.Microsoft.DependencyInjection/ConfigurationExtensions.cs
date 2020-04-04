using System;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace Singularity
{
    /// <summary>
    /// Configuration extensions for <see cref="Microsoft.Extensions.DependencyInjection"/>.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Creates a singularity <see cref="Container"/> from a <see cref="IServiceCollection"/>.
        /// Also calls <see cref="RegisterServiceProvider"/> and <see cref="RegisterServices"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static ContainerBuilder CreateContainerBuilder(this IServiceCollection services, SingularitySettings? settings = null)
        {
            var builder = new ContainerBuilder(settings: settings);
            builder.RegisterServiceProvider();
            builder.RegisterServices(services);
            return builder;
        }

        /// <summary>
        /// Registers the required services to support microsoft dependency injection.
        /// </summary>
        /// <param name="config"></param>
        public static void RegisterServiceProvider(this ContainerBuilder config)
        {
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
                RegisterWithFactory(config, registration);
            }
            else if (registration.ImplementationInstance != null)
            {
                config.Register(registration.ServiceType, c =>
                {
                    c.Inject(Expression.Constant(registration.ImplementationInstance))
                     .With(ConstructorResolvers.BestMatch);
                });
            }
            else
            {
                config.Register(registration.ServiceType, registration.ImplementationType, c =>
                {
                    c.With(ConvertLifetime(registration.Lifetime))
                     .With(ConstructorResolvers.BestMatch);
                });
            }
        }

        private static void RegisterWithFactory(ContainerBuilder config, ServiceDescriptor registration)
        {
            ParameterExpression serviceProviderParameter = Expression.Parameter(typeof(IServiceProvider));
            config.Register(registration.ServiceType, c =>
            {
                c.Inject(Expression.Convert(
                                 Expression.Invoke(
                                     Expression.Constant(registration.ImplementationFactory), serviceProviderParameter),
                                 registration.ServiceType))
                 .With(ConvertLifetime(registration.Lifetime))
                 .With(ConstructorResolvers.BestMatch);
            });
        }

        private static ILifetime ConvertLifetime(ServiceLifetime serviceLifetime)
        {
            return serviceLifetime switch
            {
                ServiceLifetime.Singleton => Lifetimes.PerContainer,
                ServiceLifetime.Scoped => Lifetimes.PerScope,
                ServiceLifetime.Transient => Lifetimes.Transient,
                _ => throw new ArgumentOutOfRangeException(nameof(serviceLifetime), serviceLifetime, null)
            };
        }
    }
}