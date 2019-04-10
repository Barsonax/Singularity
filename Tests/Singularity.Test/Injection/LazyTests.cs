using System;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class LazyTests
    {
        [Fact]
        public void GetInstance_AsLazy()
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

        [Fact]
        public void GetInstance_AsLazy_Dispose()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IPlugin, Plugin1>().OnDeath(x => x.Dispose());
            var container = new Container(config);

            //ACT
            var lazyPlugin = container.GetInstance<Lazy<IPlugin>>();
            var plugin = lazyPlugin.Value;
            int disposeCountBefore = plugin.DisposeInvocations;
            container.Dispose();
            int disposeCountAfter = plugin.DisposeInvocations;

            //ASSERT
            Assert.IsType<Lazy<IPlugin>>(lazyPlugin);
            Assert.IsType<Plugin1>(plugin);
            Assert.Equal(0, disposeCountBefore);
            Assert.Equal(1, disposeCountAfter);
        }

        [Fact]
        public void GetInstance_Inject_AsLazy()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IPlugin, Plugin1>();
            var container = new Container(config);

            //ACT
            var lazyPlugin = container.GetInstance<LazyPlugin>();
            IPlugin plugin = lazyPlugin.Lazy.Value;

            //ASSERT
            Assert.IsType<LazyPlugin>(lazyPlugin);
            Assert.IsType<Plugin1>(plugin);
        }

        [Fact]
        public void GetInstance_Inject_AsLazy_Dispose()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IPlugin, Plugin1>().OnDeath(x => x.Dispose());
            var container = new Container(config);

            //ACT
            var lazyPlugin = container.GetInstance<LazyPlugin>();
            IPlugin plugin = lazyPlugin.Lazy.Value;
            int disposeCountBefore = plugin.DisposeInvocations;
            container.Dispose();
            int disposeCountAfter = plugin.DisposeInvocations;

            //ASSERT
            Assert.IsType<LazyPlugin>(lazyPlugin);
            Assert.IsType<Plugin1>(plugin);
            Assert.Equal(0, disposeCountBefore);
            Assert.Equal(1, disposeCountAfter);
        }
    }
}
