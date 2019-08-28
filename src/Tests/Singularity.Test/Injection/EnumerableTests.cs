using System.Collections.Generic;
using System.Linq;
using Singularity.Collections;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class EnumerableTests
    {
        [Fact]
        public void GetInstance_AsEnumerable_NoRegistration()
        {
            //ARRANGE
            var container = new Container();

            //ACT
            var plugins = container.GetInstance<IEnumerable<IPlugin>>();

            //ASSERT
            Assert.IsType<InstanceFactoryList<IPlugin>>(plugins);
            IPlugin[] enumeratedPlugins = plugins.ToArray();
            Assert.Empty(enumeratedPlugins);
        }

        [Fact]
        public void GetInstance_AsCollection_NoRegistration()
        {
            //ARRANGE
            var container = new Container();

            //ACT
            var plugins = container.GetInstance<IReadOnlyCollection<IPlugin>>();

            //ASSERT
            Assert.IsType<InstanceFactoryList<IPlugin>>(plugins);
            Assert.Equal(0, plugins.Count);
            IPlugin[] enumeratedPlugins = plugins.ToArray();
            Assert.Empty(enumeratedPlugins);
        }

        [Fact]
        public void GetInstance_AsList_NoRegistration()
        {
            //ARRANGE
            var container = new Container();

            //ACT
            var plugins = container.GetInstance<IReadOnlyList<IPlugin>>();

            //ASSERT
            Assert.IsType<InstanceFactoryList<IPlugin>>(plugins);
            Assert.Equal(0, plugins.Count);
            Assert.Empty(plugins);
        }

        [Fact]
        public void GetInstance_AsEnumerable_SingleRegistration()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IPlugin, Plugin1>();
            });

            //ACT
            var plugins = container.GetInstance<IEnumerable<IPlugin>>();

            //ASSERT
            Assert.IsType<InstanceFactoryList<IPlugin>>(plugins);
            IPlugin[] enumeratedPlugins = plugins.ToArray();
            Assert.Single(enumeratedPlugins);
            Assert.IsType<Plugin1>(enumeratedPlugins[0]);
        }

        [Fact]
        public void GetInstance_AsCollection_SingleRegistration()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IPlugin, Plugin1>();
            });

            //ACT
            var plugins = container.GetInstance<IReadOnlyCollection<IPlugin>>();

            //ASSERT
            Assert.IsType<InstanceFactoryList<IPlugin>>(plugins);
            Assert.Equal(1, plugins.Count);
            IPlugin[] enumeratedPlugins = plugins.ToArray();
            Assert.Single(enumeratedPlugins);
            Assert.IsType<Plugin1>(enumeratedPlugins[0]);
        }

        [Fact]
        public void GetInstance_AsList_SingleRegistration()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IPlugin, Plugin1>();
            });

            //ACT
            var plugins = container.GetInstance<IReadOnlyList<IPlugin>>();

            //ASSERT
            Assert.IsType<InstanceFactoryList<IPlugin>>(plugins);
            Assert.Equal(1, plugins.Count);
            Assert.Single(plugins);
            Assert.IsType<Plugin1>(plugins[0]);
        }

        [Fact]
        public void GetInstance_AsEnumerable_MultiRegistration()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IPlugin, Plugin1>();
                builder.Register<IPlugin, Plugin2>();
                builder.Register<IPlugin, Plugin3>();
            });

            //ACT
            var plugins = container.GetInstance<IEnumerable<IPlugin>>();

            //ASSERT
            Assert.IsType<InstanceFactoryList<IPlugin>>(plugins);
            IPlugin[] enumeratedPlugins = plugins.ToArray();
            Assert.Equal(3, enumeratedPlugins.Length);
            Assert.IsType<Plugin1>(enumeratedPlugins[0]);
            Assert.IsType<Plugin2>(enumeratedPlugins[1]);
            Assert.IsType<Plugin3>(enumeratedPlugins[2]);
        }

        [Fact]
        public void GetInstance_AsEnumerable_MultiRegistrationWithLifetimes()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IPlugin, Plugin1>(c => c
                    .With(Lifetimes.PerContainer));
                builder.Register<IPlugin, Plugin2>();
                builder.Register<IPlugin, Plugin3>(c => c
                    .With(Lifetimes.PerContainer));
            });

            //ACT
            var plugins = container.GetInstance<IEnumerable<IPlugin>>();

            //ASSERT
            Assert.IsType<InstanceFactoryList<IPlugin>>(plugins);
            IPlugin[] firstEnumeration = plugins.ToArray();
            IPlugin[] secondEnumeration = plugins.ToArray();
            Assert.Equal(3, firstEnumeration.Length);
            Assert.Equal(3, secondEnumeration.Length);
            Assert.Same(firstEnumeration[0], secondEnumeration[0]);
            Assert.NotSame(firstEnumeration[1], secondEnumeration[1]);
            Assert.Same(firstEnumeration[2], secondEnumeration[2]);
        }

        [Fact]
        public void GetInstance_AsEnumerable_MultiBatchRegistration()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register(typeof(IPlugin), new[]
                {
                    typeof(Plugin1),
                    typeof(Plugin2),
                    typeof(Plugin3)
                });
            });

            //ACT
            var plugins = container.GetInstance<IEnumerable<IPlugin>>();

            //ASSERT
            Assert.IsType<InstanceFactoryList<IPlugin>>(plugins);
            IPlugin[] enumeratedPlugins = plugins.ToArray();
            Assert.Equal(3, enumeratedPlugins.Length);
            Assert.IsType<Plugin1>(enumeratedPlugins[0]);
            Assert.IsType<Plugin2>(enumeratedPlugins[1]);
            Assert.IsType<Plugin3>(enumeratedPlugins[2]);
        }

        [Fact]
        public void GetInstance_AsEnumerable_MultiBatchRegistrationWithDecorators()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register(typeof(IPlugin), new[]
                {
                    typeof(Plugin1),
                    typeof(Plugin2),
                    typeof(Plugin3)
                });
                builder.Decorate<IPlugin, PluginLogger1>();
            });

            //ACT
            var plugins = container.GetInstance<IEnumerable<IPlugin>>();

            //ASSERT
            Assert.IsType<InstanceFactoryList<IPlugin>>(plugins);
            IPlugin[] enumeratedPlugins = plugins.ToArray();
            Assert.Equal(3, enumeratedPlugins.Length);
            var pluginLogger1 = Assert.IsType<PluginLogger1>(enumeratedPlugins[0]);
            Assert.IsType<Plugin1>(pluginLogger1.Plugin);
            var pluginLogger2 = Assert.IsType<PluginLogger1>(enumeratedPlugins[1]);
            Assert.IsType<Plugin2>(pluginLogger2.Plugin);
            var pluginLogger3 = Assert.IsType<PluginLogger1>(enumeratedPlugins[2]);
            Assert.IsType<Plugin3>(pluginLogger3.Plugin);
        }

        [Fact]
        public void GetInstance_MultiRegistration()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IPlugin, Plugin1>();
                builder.Register<IPlugin, Plugin2>();
                builder.Register<IPlugin, Plugin3>();
            });

            //ACT
            var plugin = container.GetInstance<IPlugin>();

            //ASSERT
            Assert.IsType<Plugin3>(plugin);
        }

        [Fact]
        public void GetInstance_MultiRegistrationNestedType()
        {
            //ARRANGE
            var container = new Container(builder =>
            {
                builder.Register<IPlugin, Plugin1>();
                builder.Register<IPlugin, Plugin2>();
                builder.Register<IPlugin, Plugin3>();
            });

            //ACT
            var plugins = container.GetInstance<PluginCollection>();

            //ASSERT
            Assert.Equal(3, plugins.Plugins.Length);
            Assert.IsType<Plugin1>(plugins.Plugins[0]);
            Assert.IsType<Plugin2>(plugins.Plugins[1]);
            Assert.IsType<Plugin3>(plugins.Plugins[2]);
        }
    }
}
