using System;
using System.Collections.ObjectModel;
using System.Linq;
using Singularity.Exceptions;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Bindings
{
    public class BindingConfigTests
    {
        [Fact]
        public void IncorrectBinding_Throws()
        {
            Assert.Throws<BindingConfigException>(() =>
            {
                var config = new BindingConfig();
                config.Register<ITestService10>();
                var container = new Container(config);
            });
        }

        [Fact]
		public void Decorate_NotAInterface_Throws()
		{
			var config = new BindingConfig();
			Assert.Throws<InterfaceExpectedException>(() =>
            {
                config.Decorate<TestService10, DecoratorWithNoInterface>();
            });
		}

	    [Fact]
	    public void GetDependencies_SingleRegistration_Enumerate()
	    {
            //ARRANGE
		    var config = new BindingConfig();
		    config.Register<ITestService10, TestService10>();
		    config.Register<ITestService11, TestService11>();
		    config.Register<ITestService12, TestService12>();

            //ACT
            ReadOnlyCollection<Binding> bindings = config.GetDependencies();

            //ASSERT
			Assert.Equal(3, bindings.Count);
		    Assert.Contains(bindings, x => x.DependencyType == typeof(ITestService10));
		    Assert.Contains(bindings, x => x.DependencyType == typeof(ITestService11));
		    Assert.Contains(bindings, x => x.DependencyType == typeof(ITestService12));
		}

        [Fact]
        public void GetDependencies_MultiRegistration_Enumerate()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IPlugin, Plugin1>();
            config.Register<IPlugin, Plugin2>();
            config.Register<IPlugin, Plugin3>();

            //ACT
            ReadOnlyCollection<Binding> bindings = config.GetDependencies();

            //ASSERT
            Assert.Equal(3, bindings.Count);
            Assert.Equal(typeof(Plugin1) ,bindings[0].Expression!.Type);
            Assert.Equal(typeof(Plugin2), bindings[1].Expression!.Type);
            Assert.Equal(typeof(Plugin3), bindings[2].Expression!.Type);
        }

        [Fact]
        public void Decorate_InvalidConstructorArguments_WeaklyTyped_Throws()
        {
            var config = new BindingConfig();
            Assert.Throws<InvalidExpressionArgumentsException>(() =>
            {
                config.Decorate(typeof(ITestService10),typeof(DecoratorWrongConstructorArguments));
            });
        }

        [Fact]
        public void Decorate_InvalidConstructorArguments_StronglyTyped_Throws()
        {
            var config = new BindingConfig();
            Assert.Throws<InvalidExpressionArgumentsException>(() =>
            {
                config.Decorate<ITestService10, DecoratorWrongConstructorArguments>();
            });
        }

        [Fact]
        public void ForInjectArity1()
        {
            var config = new BindingConfig();
            config.Register<object>().Inject<object, object>((obj0) => new object());

            Binding binding = config.GetDependencies().First();
            Assert.Equal(typeof(object), binding.DependencyType);
            Assert.Equal(typeof(Func<object, object>), binding.Expression?.Type);
        }

        [Fact]
        public void ForInjectArity2()
        {
            var config = new BindingConfig();
            config.Register<object>().Inject<object, object, object>((obj0, obj1) => new object());

            Binding binding = config.GetDependencies().First();
            Assert.Equal(typeof(object), binding.DependencyType);
            Assert.Equal(typeof(Func<object, object, object>), binding.Expression?.Type);
        }

        [Fact]
        public void ForInjectArity3()
        {
            var config = new BindingConfig();
            config.Register<object>().Inject<object, object, object, object>((obj0, obj1, obj2) => new object());

            Binding binding = config.GetDependencies().First();
            Assert.Equal(typeof(object), binding.DependencyType);
            Assert.Equal(typeof(Func<object, object, object, object>), binding.Expression?.Type);
        }

        [Fact]
        public void ForInjectArity4()
        {
            var config = new BindingConfig();
            config.Register<object>().Inject<object, object, object, object, object>((obj0, obj1, obj2, obj3) => new object());

            Binding binding = config.GetDependencies().First();
            Assert.Equal(typeof(object), binding.DependencyType);
            Assert.Equal(typeof(Func<object, object, object, object, object>), binding.Expression?.Type);
        }

        [Fact]
        public void ForInjectArity5()
        {
            var config = new BindingConfig();
            config.Register<object>().Inject<object, object, object, object, object, object>((obj0, obj1, obj2, obj3, obj4) => new object());

            Binding binding = config.GetDependencies().First();
            Assert.Equal(typeof(object), binding.DependencyType);
            Assert.Equal(typeof(Func<object, object, object, object, object, object>), binding.Expression?.Type);
        }

        [Fact]
        public void ForInjectArity6()
        {
            var config = new BindingConfig();
            config.Register<object>().Inject<object, object, object, object, object, object, object>((obj0, obj1, obj2, obj3, obj4, obj5) => new object());

            Binding binding = config.GetDependencies().First();
            Assert.Equal(typeof(object), binding.DependencyType);
            Assert.Equal(typeof(Func<object, object, object, object, object, object, object>), binding.Expression?.Type);
        }

        [Fact]
        public void ForInjectArity7()
        {
            var config = new BindingConfig();
            config.Register<object>().Inject<object, object, object, object, object, object, object, object>((obj0, obj1, obj2, obj3, obj4, obj5, obj6) => new object());

            Binding binding = config.GetDependencies().First();
            Assert.Equal(typeof(object), binding.DependencyType);
            Assert.Equal(typeof(Func<object, object, object, object, object, object, object, object>), binding.Expression?.Type);
        }

        [Fact]
        public void ForInjectArity8()
        {
            var config = new BindingConfig();
            config.Register<object>().Inject<object, object, object, object, object, object, object, object, object>((obj0, obj1, obj2, obj3, obj4, obj5, obj6, obj7) => new object());

            Binding binding = config.GetDependencies().First();
            Assert.Equal(typeof(object), binding.DependencyType);
            Assert.Equal(typeof(Func<object, object, object, object, object, object, object, object, object>), binding.Expression?.Type);
        }
    }
}
