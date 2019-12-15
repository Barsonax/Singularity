﻿using System;
using System.Collections.Generic;
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
            var container = new Container(builder =>
            {
                builder.Register<IPlugin, Plugin1>();
            });

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
            var container = new Container(builder =>
            {
                builder.Register<IPlugin, Plugin1>();
                builder.Register<IPlugin, Plugin2>();
                builder.Register<IPlugin, Plugin3>();
            });

            //ACT
            var lazyInstances = container.GetInstance<IReadOnlyList<Lazy<IPlugin>>>();

            //ASSERT
            Assert.Equal(3, lazyInstances.Count);
            Assert.IsType<Plugin1>(lazyInstances[0].Value);
            Assert.IsType<Plugin2>(lazyInstances[1].Value);
            Assert.IsType<Plugin3>(lazyInstances[2].Value);
        }

        [Fact]
        public void GetInstance_AsLazy_Dispose()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IPlugin, Plugin1>(c => c
                    .WithFinalizer(x => x.Dispose()));
            });

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
            var container = new Container(builder =>
            {
                builder.Register<IPlugin, Plugin1>();
            });

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
            var container = new Container(builder =>
            {
                builder.Register<IPlugin, Plugin1>(c => c
                    .WithFinalizer(x => x.Dispose()));
            });

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
