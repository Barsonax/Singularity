using System;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class FactoryTests
    {
        [Fact]
        public void RequestInstanceAsFactory()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IPlugin, Plugin1>();
            var container = new Container(config);

            //ACT
            var factory = container.GetInstance<Func<IPlugin>>();
            var instance = factory.Invoke();

            //ASSERT
            Assert.IsType<Func<IPlugin>>(factory);
            Assert.IsType<Plugin1>(instance);
        }
    }
}