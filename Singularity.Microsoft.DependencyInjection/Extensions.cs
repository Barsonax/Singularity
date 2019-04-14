using System;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Singularity.Bindings;

namespace Singularity.Microsoft.DependencyInjection
{
    public static class Extensions
    {
        public static IServiceProvider CreateServiceProvider(this BindingConfig config)
        {
            Container container = null;
            // ReSharper disable once AccessToModifiedClosure
            config.Register<Container>().Inject(() => container);
            config.Register<IServiceProvider, SingularityServiceProvider>();
            config.Register<IServiceScopeFactory, SingularityServiceScopeFactory>();

            container = new Container(config);

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
                Expression<Func<Scoped, object>> foo = scope => registration.ImplementationFactory.Invoke(new SingularityServiceProvider(scope.Container));
                WeaklyTypedConfiguredBinding binding = config.Register(registration.ServiceType).Inject(foo).With(ConvertLifetime(registration.Lifetime));
                if (typeof(IDisposable).IsAssignableFrom(registration.ImplementationType))
                {
                    binding.OnDeath(obj => ((IDisposable)obj).Dispose());
                }
            }
            else if (registration.ImplementationInstance != null)
            {
                WeaklyTypedConfiguredBinding binding = config.Register(registration.ServiceType).Inject(Expression.Constant(registration.ImplementationInstance));
                if (typeof(IDisposable).IsAssignableFrom(registration.ImplementationType))
                {
                    binding.OnDeath(obj => ((IDisposable)obj).Dispose());
                }
            }
            else
            {
                WeaklyTypedConfiguredBinding binding = config.Register(registration.ServiceType, registration.ImplementationType).With(ConvertLifetime(registration.Lifetime));
                if (typeof(IDisposable).IsAssignableFrom(registration.ImplementationType))
                {
                    binding.OnDeath(obj => ((IDisposable)obj).Dispose());
                }
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