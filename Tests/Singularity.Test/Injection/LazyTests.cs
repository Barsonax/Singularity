using System;
using System.Collections.Generic;
using Singularity.Collections;
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
            IPlugin plugin = lazyPlugin.Value;

            //ASSERT
            Assert.IsType<Lazy<IPlugin>>(lazyPlugin);
            Assert.IsType<Plugin1>(plugin);
        }

        [Fact]
        public void GetInstance_AsLazyEnumerable()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IPlugin, Plugin1>();
            config.Register<IPlugin, Plugin2>();
            config.Register<IPlugin, Plugin3>();
            var container = new Container(config);

            //ACT
            var lazyInstances = container.GetInstance<IReadOnlyList<Lazy<IPlugin>>>();

            //ASSERT
            Assert.IsType<InstanceFactoryList<Lazy<IPlugin>>>(lazyInstances);
            Assert.Equal(3, lazyInstances.Count);
            Assert.IsType<Plugin1>(lazyInstances[0].Value);
            Assert.IsType<Plugin2>(lazyInstances[1].Value);
            Assert.IsType<Plugin3>(lazyInstances[2].Value);
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
            IPlugin plugin = lazyPlugin.Value;
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
