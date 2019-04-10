using System;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class FactoryTests
    {
        [Fact]
        public void GetInstance_AsFactory()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IPlugin, Plugin1>();
            var container = new Container(config);

            //ACT
            var factory = container.GetInstance<Func<IPlugin>>();
            IPlugin plugin = factory.Invoke();

            //ASSERT
            Assert.IsType<Func<Plugin1>>(factory);
            Assert.IsType<Plugin1>(plugin);
        }

        [Fact]
        public void GetInstance_AsFactory_Dispose()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IPlugin, Plugin1>().OnDeath(x => x.Dispose());
            var container = new Container(config);

            //ACT
            var factory = container.GetInstance<Func<IPlugin>>();
            IPlugin plugin = factory.Invoke();
            int disposeCountBefore = plugin.DisposeInvocations;
            container.Dispose();
            int disposeCountAfter = plugin.DisposeInvocations;

            //ASSERT
            Assert.IsType<Func<Plugin1>>(factory);
            Assert.IsType<Plugin1>(plugin);
            Assert.Equal(0, disposeCountBefore);
            Assert.Equal(1,disposeCountAfter);
        }

        [Fact]
        public void GetInstance_Inject_AsFactory()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IPlugin, Plugin1>();
            var container = new Container(config);

            //ACT
            var factory = container.GetInstance<PluginFactory>();
            IPlugin plugin = factory.Factory.Invoke();

            //ASSERT
            Assert.IsType<PluginFactory>(factory);
            Assert.IsType<Plugin1>(plugin);
        }

        [Fact]
        public void GetInstance_Inject_AsFactory_Dispose()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IPlugin, Plugin1>().OnDeath(x => x.Dispose());
            var container = new Container(config);

            //ACT
            var factory = container.GetInstance<PluginFactory>();
            IPlugin plugin = factory.Factory.Invoke();
            int disposeCountBefore = plugin.DisposeInvocations;
            container.Dispose();
            int disposeCountAfter = plugin.DisposeInvocations;

            //ASSERT
            Assert.IsType<PluginFactory>(factory);
            Assert.IsType<Plugin1>(plugin);
            Assert.Equal(0, disposeCountBefore);
            Assert.Equal(1, disposeCountAfter);
        }
    }
}