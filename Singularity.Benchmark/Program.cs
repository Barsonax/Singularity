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
            //BenchmarkRunner.Run<LookupBenchmark>();
            BenchmarkRunner.Run<InjectorBenchmark>();
            Console.ReadKey();
        }
    }

	public class InjectorBenchmark
	{
		private readonly Container _singulairtyContainer;
	    private readonly Func<IInjectorTest1> _instanceFactoryGeneric;
	    private readonly Func<object> _instanceFactory;
        private readonly MethodInjectorTest _methodInjectorTest = new MethodInjectorTest();
	    private readonly Action<object> _methodInjector;

        public InjectorBenchmark()
		{
		    var config = new BindingConfig();
		    config.Register<IInjectorTest1, InjectorTest1>().With(Lifetime.PerContainer);
            _singulairtyContainer = new Container(config);
		    _instanceFactoryGeneric = _singulairtyContainer.GetInstanceFactory<IInjectorTest1>();
		    _instanceFactory = _singulairtyContainer.GetInstanceFactory(typeof(IInjectorTest1));
		    _methodInjector = _singulairtyContainer.GetMethodInjector(typeof(MethodInjectorTest));
        }

	    //[Benchmark]
	    //public void MethodInjectorInvoke()
	    //{
     //       _methodInjector.Invoke(_methodInjectorTest);
	    //}

     //   [Benchmark]
	    //public void MethodInjection()
	    //{
	    //    _singulairtyContainer.MethodInject(_methodInjectorTest);
	    //}

        //[Benchmark]
        //public IInjectorTest1 GetInstanceFactoryGenericInvoke()
        //{
        //    return _instanceFactoryGeneric.Invoke();
        //}

        //[Benchmark]
        //public IInjectorTest1 GetInstanceFactoryInvoke()
        //{
        //    return (IInjectorTest1)_instanceFactory.Invoke();
        //}

        [Benchmark]
        public IInjectorTest1 GetInstanceGeneric()
        {
            return _singulairtyContainer.GetInstance<IInjectorTest1>();
        }

        [Benchmark]
        public IInjectorTest1 GetInstance()
        {
            return (IInjectorTest1)_singulairtyContainer.GetInstance(typeof(IInjectorTest1));
        }
    }

    public class MethodInjectorTest
    {

        public void Init(IInjectorTest1 iiInjectorTest)
        {

        }
    }

	public class InjectorTest1 : IInjectorTest1
	{

	}

	public interface IInjectorTest1
	{

	}

    public class InjectorTest2 : IInjectorTest2
    {

    }

    public interface IInjectorTest2
    {

    }

    public class InjectorTest3 : IInjectorTest3
    {

    }

    public interface IInjectorTest3
    {

    }

    public interface IInjectorTest4
    {

    }

    public interface IInjectorTest5
    {

    }
    public interface IInjectorTest6
    {

    }
    public interface IInjectorTest7
    {

    }

    public interface IInjectorTest8
    {

    }

    public interface IInjectorTest9
    {

    }

    public interface IInjectorTest10
    {

    }

    public interface IInjectorTest11
    {

    }

    public interface IInjectorTest12
    {

    }
}
