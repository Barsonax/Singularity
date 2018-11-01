using System;
using System.Collections;
using System.Linq;
using Singularity.Bindings;
using Singularity.Exceptions;
using Singularity.Test.TestClasses;
using Xunit;

namespace Singularity.Test.Bindings
{
    public class BindingConfigTests
    {
		[Fact]
		public void Decorate_NotAInterface_Throws()
		{
			var config = new BindingConfig();
			Assert.Throws<InterfaceExpectedException>(() =>
			{
				config.Decorate<TestService10>().With<DecoratorWithNoInterface>();
			});
		}

	    [Fact]
	    public void Enumerable_Enumerate()
	    {
		    var config = new BindingConfig();
		    config.For<ITestService10>().Inject<TestService10>();
		    config.For<ITestService11>().Inject<TestService11>();
		    config.For<ITestService12>().Inject<TestService12>();

		    IEnumerable enumerable = config;

		    IBinding[] bindings = enumerable.OfType<IBinding>().ToArray();
			Assert.Equal(3, bindings.Length);
		    Assert.Contains(bindings, x => x.DependencyType == typeof(ITestService10));
		    Assert.Contains(bindings, x => x.DependencyType == typeof(ITestService11));
		    Assert.Contains(bindings, x => x.DependencyType == typeof(ITestService12));
		}

		[Fact]
        public void Decorate_WrongConstructorArguments_Throws()
        {
            var config = new BindingConfig();
            Assert.Throws<InterfaceNotImplementedException>(() =>
            {
                config.Decorate<ITestService10>().With(typeof(Component));
            });
        }

        [Fact]
        public void Decorate_InvalidConstructorArguments_Throws()
        {
            var config = new BindingConfig();
            Assert.Throws<InvalidExpressionArgumentsException>(() =>
            {
                config.Decorate<ITestService10>().With<DecoratorWrongConstructorArguments>();
            });
        }

        [Fact]
        public void ForInjectArity0()
        {
            var config = new BindingConfig();
            config.For<object>().Inject<object>();

            IBinding binding = config.Bindings.Values.First();
            Assert.Equal(typeof(object), binding.DependencyType);
            Assert.Equal(typeof(object), binding.Expression.Type);
        }

        [Fact]
        public void ForInjectArity1()
        {
            var config = new BindingConfig();
            config.For<object>().Inject<object, object>((obj0) => new object());

            IBinding binding = config.Bindings.Values.First();
            Assert.Equal(typeof(object), binding.DependencyType);
            Assert.Equal(typeof(Func<object, object>), binding.Expression.Type);
        }

        [Fact]
        public void ForInjectArity2()
        {
            var config = new BindingConfig();
            config.For<object>().Inject<object, object, object>((obj0, obj1) => new object());

            IBinding binding = config.Bindings.Values.First();
            Assert.Equal(typeof(object), binding.DependencyType);
            Assert.Equal(typeof(Func<object, object, object>), binding.Expression.Type);
        }

        [Fact]
        public void ForInjectArity3()
        {
            var config = new BindingConfig();
            config.For<object>().Inject<object, object, object, object>((obj0, obj1, obj2) => new object());

            IBinding binding = config.Bindings.Values.First();
            Assert.Equal(typeof(object), binding.DependencyType);
            Assert.Equal(typeof(Func<object, object, object, object>), binding.Expression.Type);
        }

        [Fact]
        public void ForInjectArity4()
        {
            var config = new BindingConfig();
            config.For<object>().Inject<object, object, object, object, object>((obj0, obj1, obj2, obj3) => new object());

            IBinding binding = config.Bindings.Values.First();
            Assert.Equal(typeof(object), binding.DependencyType);
            Assert.Equal(typeof(Func<object, object, object, object, object>), binding.Expression.Type);
        }

        [Fact]
        public void ForInjectArity5()
        {
            var config = new BindingConfig();
            config.For<object>().Inject<object, object, object, object, object, object>((obj0, obj1, obj2, obj3, obj4) => new object());

            IBinding binding = config.Bindings.Values.First();
            Assert.Equal(typeof(object), binding.DependencyType);
            Assert.Equal(typeof(Func<object, object, object, object, object, object>), binding.Expression.Type);
        }

        [Fact]
        public void ForInjectArity6()
        {
            var config = new BindingConfig();
            config.For<object>().Inject<object, object, object, object, object, object, object>((obj0, obj1, obj2, obj3, obj4, obj5) => new object());

            IBinding binding = config.Bindings.Values.First();
            Assert.Equal(typeof(object), binding.DependencyType);
            Assert.Equal(typeof(Func<object, object, object, object, object, object, object>), binding.Expression.Type);
        }

        [Fact]
        public void ForInjectArity7()
        {
            var config = new BindingConfig();
            config.For<object>().Inject<object, object, object, object, object, object, object, object>((obj0, obj1, obj2, obj3, obj4, obj5, obj6) => new object());

            IBinding binding = config.Bindings.Values.First();
            Assert.Equal(typeof(object), binding.DependencyType);
            Assert.Equal(typeof(Func<object, object, object, object, object, object, object, object>), binding.Expression.Type);
        }

        [Fact]
        public void ForInjectArity8()
        {
            var config = new BindingConfig();
            config.For<object>().Inject<object, object, object, object, object, object, object, object, object>((obj0, obj1, obj2, obj3, obj4, obj5, obj6, obj7) => new object());

            IBinding binding = config.Bindings.Values.First();
            Assert.Equal(typeof(object), binding.DependencyType);
            Assert.Equal(typeof(Func<object, object, object, object, object, object, object, object, object>), binding.Expression.Type);
        }
    }
}
