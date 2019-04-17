using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Singularity.Bindings;
using Singularity.Microsoft.DependencyInjection;
using Singularity.TestClasses.TestClasses;

namespace Singularity.TestClasses.Benchmark
{
    public class ContainerBenchmark
    {
        private Container _container;
        private BindingConfig _cachedBindingConfig;

        public ContainerBenchmark()
        {
            _container = NewContainer();
            _cachedBindingConfig = new BindingConfig();
            Register(_cachedBindingConfig);
        }

        public void AspNetCore()
        {
            var factory = (IServiceScopeFactory)_container.GetInstance(typeof(IServiceScopeFactory));

            using (var scope = factory.CreateScope())
            {
                var controller = scope.ServiceProvider.GetService(typeof(TestController1));
            }
        }

        public ISingleton1 Singleton()
        {
            return (ISingleton1)_container.GetInstance(typeof(ISingleton1));
        }

        public ITransient1 Transient()
        {
            return (ITransient1)_container.GetInstance(typeof(ITransient1));
        }

        public ICombined1 Combined()
        {
            return (ICombined1)_container.GetInstance(typeof(ICombined1));
        }

        public IComplex1 Complex()
        {
            return (IComplex1)_container.GetInstance(typeof(IComplex1));
        }

        public IEnumerable<ISimpleAdapter> Multi()
        {
            return (IEnumerable<ISimpleAdapter>)_container.GetInstance(typeof(IEnumerable<ISimpleAdapter>));
        }

        public IDisposable Disposable()
        {
            return (IDisposable)_container.GetInstance(typeof(IDisposable));
        }

        public void Register()
        {
            var config = new BindingConfig();

            Register(config);
        }

        public void RegisterAndEnumerate()
        {
            var config = new BindingConfig();

            Register(config);

            config.GetDependencies();
        }

        public Container NewContainer()
        {
            var config = new BindingConfig();

            Register(config);

            config.Register<Container>().Inject(() => this._container).With(Dispose.Never);
            config.Register<IServiceProvider, SingularityServiceProvider>();
            config.Register<IServiceScopeFactory, SingularityServiceScopeFactory>();

            //return new Container(config);
            return new Container(config, new SingularitySettings() {AutoDispose = true});
        }

        public Container NewContainerFromCachedConfig()
        {
            return new Container(_cachedBindingConfig);
        }

        public IComplex1 NewContainerAndResolve()
        {
            var config = new BindingConfig();

            Register(config);

            var container = new Container(config);
            return (IComplex1)container.GetInstance(typeof(IComplex1));
        }

        public Container NewNestedContainer()
        {
            return _container.GetNestedContainer(Enumerable.Empty<IModule>());
        }

        private static void Register(BindingConfig config)
        {
            config.Register<IDummyOne, DummyOne>();
            config.Register<IDummyTwo, DummyTwo>();
            config.Register<IDummyThree, DummyThree>();
            config.Register<IDummyFour, DummyFour>();
            config.Register<IDummyFive, DummyFive>();
            config.Register<IDummySix, DummySix>();
            config.Register<IDummySeven, DummySeven>();
            config.Register<IDummyEight, DummyEight>();
            config.Register<IDummyNine, DummyNine>();
            config.Register<IDummyTen, DummyTen>();
            config.Register<IDisposable, Disposable>().With(Lifetime.PerScope);

            config.Register<ISingleton1, Singleton1>().With(Lifetime.PerContainer);
            config.Register<ISingleton2, Singleton2>().With(Lifetime.PerContainer);
            config.Register<ISingleton3, Singleton3>().With(Lifetime.PerContainer);
            config.Register<ITransient1, Transient1>();
            config.Register<ITransient2, Transient2>();
            config.Register<ITransient3, Transient3>();
            config.Register<ICombined1, Combined1>();
            config.Register<ICombined2, Combined2>();
            config.Register<ICombined3, Combined3>();
            config.Register<ICalculator1, Calculator1>();
            config.Register<ICalculator2, Calculator2>();
            config.Register<ICalculator3, Calculator3>();

            config.Register<ISubObjectOne, SubObjectOne>();
            config.Register<ISubObjectTwo, SubObjectTwo>();
            config.Register<ISubObjectThree, SubObjectThree>();
            config.Register<IFirstService, FirstService>().With(Lifetime.PerContainer);
            config.Register<ISecondService, SecondService>().With(Lifetime.PerContainer);
            config.Register<IThirdService, ThirdService>().With(Lifetime.PerContainer);
            config.Register<IComplex1, Complex1>();

            config.Register<ISimpleAdapter, SimpleAdapterOne>();
            config.Register<ISimpleAdapter, SimpleAdapterTwo>();
            config.Register<ISimpleAdapter, SimpleAdapterThree>();
            config.Register<ISimpleAdapter, SimpleAdapterFour>();
            config.Register<ISimpleAdapter, SimpleAdapterFive>();

            config.Register<IDisposable, Disposable>().With(Lifetime.PerScope);

            config.Register<TestController1, TestController1>();
            config.Register<TestController2, TestController2>();
            config.Register<TestController3, TestController3>();
            config.Register<IRepositoryTransient1, RepositoryTransient1>();
            config.Register<IRepositoryTransient2, RepositoryTransient2>();
            config.Register<IRepositoryTransient3, RepositoryTransient3>();
            config.Register<IScopedService, ScopedService>().With(Lifetime.PerScope);
        }
    }
}
