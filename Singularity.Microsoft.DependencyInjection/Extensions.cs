using System;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace Singularity.Microsoft.DependencyInjection
{
    public static class Extensions
    {
        public static IServiceProvider CreateServiceProvider(this BindingConfig config)
        {
            Container? container = null;
            // ReSharper disable once AccessToModifiedClosure
            config.Register<Container>().Inject(() => container).With(DisposeBehavior.Never);
            config.Register<IServiceProvider, SingularityServiceProvider>();
            config.Register<IServiceScopeFactory, SingularityServiceScopeFactory>();

            container = new Container(config, new SingularitySettings { AutoDispose = true });

            return new SingularityServiceProvider(container);
        }

        public static void RegisterServices(this BindingConfig config, IServiceCollection serviceCollection)
        {
            foreach (ServiceDescriptor registration in serviceCollection)
            {
                config.RegisterService(registration);
            }
        }

        public static void RegisterService(this BindingConfig config, ServiceDescriptor registration)
        {
            if (registration.ImplementationFactory != null)
            {
                ParameterExpression serviceProviderParameter = Expression.Parameter(typeof(IServiceProvider));
                config.Register(registration.ServiceType)
                    .Inject(Expression.Lambda(
                            Expression.Convert(
                                    Expression.Invoke(
                                        Expression.Constant(registration.ImplementationFactory), serviceProviderParameter), registration.ServiceType), serviceProviderParameter))
                    .With(ConvertLifetime(registration.Lifetime));
            }
            else if (registration.ImplementationInstance != null)
            {
                config.Register(registration.ServiceType).Inject(Expression.Constant(registration.ImplementationInstance));
            }
            else
            {
                config.Register(registration.ServiceType, registration.ImplementationType).With(ConvertLifetime(registration.Lifetime));
            }
        }

        private static Lifetime ConvertLifetime(ServiceLifetime serviceLifetime)
        {
            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton:
                    return Lifetime.PerContainer;
                case ServiceLifetime.Scoped:
                    return Lifetime.PerScope;
                case ServiceLifetime.Transient:
                    return Lifetime.Transient;
                default:
                    throw new ArgumentOutOfRangeException(nameof(serviceLifetime), serviceLifetime, null);
            }
        }
    }
}