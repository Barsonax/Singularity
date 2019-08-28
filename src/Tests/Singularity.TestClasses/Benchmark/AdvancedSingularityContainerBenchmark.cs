using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Singularity.Microsoft.DependencyInjection;
using Singularity.TestClasses.TestClasses;

namespace Singularity.TestClasses.Benchmark
{
    public class AdvancedSingularityContainerBenchmark
    {
        private readonly Container _container;

        public AdvancedSingularityContainerBenchmark()
        {
            _container = NewContainer();
        }

        public void AspNetCore()
        {
            var factory = (IServiceScopeFactory)_container.GetInstance(typeof(IServiceScopeFactory));

            using (var scope = factory.CreateScope())
            {
                var controller = scope.ServiceProvider.GetService(typeof(TestController1));
            }

            //factory = (IServiceScopeFactory)_container.GetInstance(typeof(IServiceScopeFactory));

            //using (var scope = factory.CreateScope())
            //{
            //    var controller = scope.ServiceProvider.GetService(typeof(TestController2));
            //}

            //factory = (IServiceScopeFactory)_container.GetInstance(typeof(IServiceScopeFactory));

            //using (var scope = factory.CreateScope())
            //{
            //    var controller = scope.ServiceProvider.GetService(typeof(TestController3));
            //}
        }

        public IEnumerable<ISimpleAdapter> Multi()
        {
            return (IEnumerable<ISimpleAdapter>)_container.GetInstance(typeof(IEnumerable<ISimpleAdapter>));
        }

        public IDisposable Disposable()
        {
            return (IDisposable)_container.GetInstance(typeof(IDisposable));
        }

        public Container Register()
        {
            return new Container(Register, SingularitySettings.Microsoft);
        }

        public Container NewContainer()
        {
            return new Container(Register, SingularitySettings.Microsoft);
        }

        public IComplex1 NewContainerAndResolve()
        {
            var container = new Container(Register);
            return (IComplex1)container.GetInstance(typeof(IComplex1));
        }

        public Container NewNestedContainer()
        {
            return _container.GetNestedContainer(Enumerable.Empty<IModule>());
        }

        private static void Register(ContainerBuilder builder)
        {
            builder.RegisterServiceProvider();

            builder.Register<IDummyOne, DummyOne>();
            builder.Register<IDummyTwo, DummyTwo>();
            builder.Register<IDummyThree, DummyThree>();
            builder.Register<IDummyFour, DummyFour>();
            builder.Register<IDummyFive, DummyFive>();
            builder.Register<IDummySix, DummySix>();
            builder.Register<IDummySeven, DummySeven>();
            builder.Register<IDummyEight, DummyEight>();
            builder.Register<IDummyNine, DummyNine>();
            builder.Register<IDummyTen, DummyTen>();
            builder.Register<IDisposable, Disposable>(c => c.With(Lifetimes.PerScope));

            builder.Register<ISingleton1, Singleton1>(c => c.With(Lifetimes.PerContainer));
            builder.Register<ISingleton2, Singleton2>(c => c.With(Lifetimes.PerContainer));
            builder.Register<ISingleton3, Singleton3>(c => c.With(Lifetimes.PerContainer));
            builder.Register<ITransient1, Transient1>();
            builder.Register<ITransient2, Transient2>();
            builder.Register<ITransient3, Transient3>();
            builder.Register<ICombined1, Combined1>();
            builder.Register<ICombined2, Combined2>();
            builder.Register<ICombined3, Combined3>();
            builder.Register<ICalculator1, Calculator1>();
            builder.Register<ICalculator2, Calculator2>();
            builder.Register<ICalculator3, Calculator3>();

            builder.Register<ISubObjectOne, SubObjectOne>();
            builder.Register<ISubObjectTwo, SubObjectTwo>();
            builder.Register<ISubObjectThree, SubObjectThree>();
            builder.Register<IFirstService, FirstService>(c => c.With(Lifetimes.PerContainer));
            builder.Register<ISecondService, SecondService>(c => c.With(Lifetimes.PerContainer));
            builder.Register<IThirdService, ThirdService>(c => c.With(Lifetimes.PerContainer));
            builder.Register<IComplex1, Complex1>();

            builder.Register<ISimpleAdapter, SimpleAdapterOne>();
            builder.Register<ISimpleAdapter, SimpleAdapterTwo>();
            builder.Register<ISimpleAdapter, SimpleAdapterThree>();
            builder.Register<ISimpleAdapter, SimpleAdapterFour>();
            builder.Register<ISimpleAdapter, SimpleAdapterFive>();

            builder.Register<IDisposable, Disposable>(c => c.With(Lifetimes.PerContainer));

            builder.Register<TestController1, TestController1>();
            builder.Register<TestController2, TestController2>();
            builder.Register<TestController3, TestController3>();
            builder.Register<IRepositoryTransient1, RepositoryTransient1>();
            builder.Register<IRepositoryTransient2, RepositoryTransient2>();
            builder.Register<IRepositoryTransient3, RepositoryTransient3>();
            builder.Register<IRepositoryTransient4, RepositoryTransient4>();
            builder.Register<IRepositoryTransient5, RepositoryTransient5>();
            builder.Register<IScopedService1, ScopedService1>(c => c.With(Lifetimes.PerScope));
            builder.Register<IScopedService2, ScopedService2>(c => c.With(Lifetimes.PerScope));
            builder.Register<IScopedService3, ScopedService3>(c => c.With(Lifetimes.PerScope));
            builder.Register<IScopedService4, ScopedService4>(c => c.With(Lifetimes.PerScope));
            builder.Register<IScopedService5, ScopedService5>(c => c.With(Lifetimes.PerScope));
        }
    }
}
