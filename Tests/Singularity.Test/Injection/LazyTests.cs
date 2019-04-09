using System;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class LazyTests
    {
        [Fact]
        public void RequestInstanceAsLazy()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IPlugin, Plugin1>();
            var container = new Container(config);

            //ACT
            var lazyPlugin = container.GetInstance<Lazy<IPlugin>>();
            var plugin = lazyPlugin.Value;

            //ASSERT
            Assert.IsType<Lazy<IPlugin>>(lazyPlugin);
            Assert.IsType<Plugin1>(plugin);
        }
    }
}
