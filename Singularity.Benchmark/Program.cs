using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using Singularity.Bindings;
using Singularity.Enums;

namespace Singularity.Benchmark
{
    class Program
    {
        static void Main()
        {
			BenchmarkRunner.Run<InjectorBenchmark>();
			Console.ReadKey();
        }
    }

	public class InjectorBenchmark
	{
		private readonly Container _singulairtyContainer;
	    private readonly Func<IInjectorTest> _instanceFactoryGeneric;
	    private readonly Func<object> _instanceFactory;
        private readonly MethodInjectorTest _methodInjectorTest = new MethodInjectorTest();
	    private readonly Action<object> _methodInjector;

        public InjectorBenchmark()
		{
		    var config = new BindingConfig();
		    config.For<IInjectorTest>().Inject<InjectorTest>().With(Lifetime.PerContainer);
            _singulairtyContainer = new Container(config);
		    _instanceFactoryGeneric = _singulairtyContainer.GetInstanceFactory<IInjectorTest>();
		    _instanceFactory = _singulairtyContainer.GetInstanceFactory(typeof(IInjectorTest));
		    _methodInjector = _singulairtyContainer.GetMethodInjector(typeof(MethodInjectorTest));
		   var f =  (IInjectorTest)_singulairtyContainer.GetInstance(typeof(IInjectorTest));
        }

	    [Benchmark]
	    public void MethodInjectorInvoke()
	    {
            _methodInjector.Invoke(_methodInjectorTest);
	    }

        [Benchmark]
	    public void MethodInjection()
	    {
	        _singulairtyContainer.MethodInject(_methodInjectorTest);
	    }

        [Benchmark]
        public IInjectorTest GetInstanceFactoryGenericInvoke()
        {
            return _instanceFactoryGeneric.Invoke();
        }

        [Benchmark]
        public IInjectorTest GetInstanceFactoryInvoke()
        {
            return (IInjectorTest)_instanceFactory.Invoke();
        }

        [Benchmark]
        public IInjectorTest GetInstanceGeneric()
        {
            return _singulairtyContainer.GetInstance<IInjectorTest>();
        }

        [Benchmark]
        public IInjectorTest GetInstance()
        {
            return (IInjectorTest)_singulairtyContainer.GetInstance(typeof(IInjectorTest));
        }
    }

    public class MethodInjectorTest
    {

        public void Init(IInjectorTest iiInjectorTest)
        {

        }
    }

	public class InjectorTest : IInjectorTest
	{

	}

	public interface IInjectorTest
	{

	}
}
