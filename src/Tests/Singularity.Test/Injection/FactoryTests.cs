using System;
using System.Collections.Generic;
using Singularity.Collections;
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
            var container = new Container(builder =>
            {
                builder.Register<IPlugin, Plugin1>();
            });

            //ACT
            var factory = container.GetInstance<Func<IPlugin>>();
            IPlugin plugin = factory.Invoke();

            //ASSERT
            Assert.IsType<Func<IPlugin>>(factory);
            Assert.IsType<Plugin1>(plugin);
        }

        [Fact]
        public void GetInstance_AsInstanceFactoryListFactory()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IPlugin, Plugin1>();
                builder.Register<IPlugin, Plugin2>();
                builder.Register<IPlugin, Plugin3>();
            });

            //ACT
            var factories = container.GetInstance<IReadOnlyList<Func<IPlugin>>>();

            //ASSERT
            Assert.IsType<InstanceFactoryList<Func<IPlugin>>>(factories);
            Assert.Equal(3, factories.Count);
            Assert.IsType<Plugin1>(factories[0].Invoke());
            Assert.IsType<Plugin2>(factories[1].Invoke());
            Assert.IsType<Plugin3>(factories[2].Invoke());
        }

        [Fact]
        public void GetInstance_AsFactory_Dispose()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IPlugin, Plugin1>(c => c
                    .WithFinalizer(x => x.Dispose()));
            });

            //ACT
            var factory = container.GetInstance<Func<IPlugin>>();
            IPlugin plugin = factory.Invoke();
            int disposeCountBefore = plugin.DisposeInvocations;
            container.Dispose();
            int disposeCountAfter = plugin.DisposeInvocations;

            //ASSERT
            Assert.IsType<Func<IPlugin>>(factory);
            Assert.IsType<Plugin1>(plugin);
            Assert.Equal(0, disposeCountBefore);
            Assert.Equal(1,disposeCountAfter);
        }

        [Fact]
        public void GetInstance_Inject_AsFactory()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IPlugin, Plugin1>();
            });

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
            var container = new Container(builder =>
            {
                builder.Register<IPlugin, Plugin1>(c => c
                    .WithFinalizer(x => x.Dispose()));
            });

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