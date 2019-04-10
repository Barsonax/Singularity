using System.Collections.Generic;
using System.Linq;
using Singularity.Exceptions;
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
            var config = new BindingConfig();
            var container = new Container(config);

            //ACT
            var plugins = container.GetInstance<IEnumerable<IPlugin>>();

            //ASSERT
            IPlugin[] enumeratedPlugins = plugins.ToArray();
            Assert.Empty(enumeratedPlugins);
        }

        [Fact]
        public void GetInstance_AsEnumerable_SingleRegistration()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IPlugin, Plugin1>();
            var container = new Container(config);

            //ACT
            var plugins = container.GetInstance<IEnumerable<IPlugin>>();

            //ASSERT
            IPlugin[] enumeratedPlugins = plugins.ToArray();
            Assert.Single(enumeratedPlugins);
            Assert.IsType<Plugin1>(enumeratedPlugins[0]);
        }

        [Fact]
        public void GetInstance_AsEnumerable_MultiRegistration()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IPlugin, Plugin1>();
            config.Register<IPlugin, Plugin2>();
            config.Register<IPlugin, Plugin3>();
            var container = new Container(config);

            //ACT
            var plugins = container.GetInstance<IEnumerable<IPlugin>>();

            //ASSERT
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
            var config = new BindingConfig();
            config.Register<IPlugin, Plugin1>().With(CreationMode.Singleton);
            config.Register<IPlugin, Plugin2>();
            config.Register<IPlugin, Plugin3>().With(CreationMode.Singleton);
            var container = new Container(config);

            //ACT
            var plugins = container.GetInstance<IEnumerable<IPlugin>>();

            //ASSERT
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
            var config = new BindingConfig();
            config.Register(typeof(IPlugin), new[]
            {
                typeof(Plugin1),
                typeof(Plugin2),
                typeof(Plugin3)
            });
            var container = new Container(config);

            //ACT
            var plugins = container.GetInstance<IEnumerable<IPlugin>>();

            //ASSERT
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
            var config = new BindingConfig();
            config.Register(typeof(IPlugin), new[]
            {
                typeof(Plugin1),
                typeof(Plugin2),
                typeof(Plugin3)
            });
            config.Decorate<IPlugin, PluginLogger1>();
            var container = new Container(config);

            //ACT
            var plugins = container.GetInstance<IEnumerable<IPlugin>>();

            //ASSERT
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
            var config = new BindingConfig();
            config.Register<IPlugin, Plugin1>();
            config.Register<IPlugin, Plugin2>();
            config.Register<IPlugin, Plugin3>();
            var container = new Container(config);

            //ACT
            var plugin = container.GetInstance<IPlugin>();

            //ASSERT
            Assert.IsType<Plugin3>(plugin);
        }

        [Fact]
        public void GetInstance_MultiRegistrationNestedType()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IPlugin, Plugin1>();
            config.Register<IPlugin, Plugin2>();
            config.Register<IPlugin, Plugin3>();
            var container = new Container(config);

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
