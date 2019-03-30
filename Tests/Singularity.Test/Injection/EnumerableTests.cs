using System.Collections.Generic;
using System.Linq;
using Singularity.TestClasses.TestClasses;
using Xunit;

namespace Singularity.Test.Injection
{
    public class EnumerableTests
    {
        [Fact]
        public void SingleRegistration()
        {
            //ARRANGE
            var config = new BindingConfig();
            config.Register<IPlugin, Plugin1>();
            var container = new Container(config);

            //ACT
            var plugins = container.GetInstance<IEnumerable<IPlugin>>();

            //ASSERT
            var enumeratedPlugins = plugins.ToArray();
            Assert.Equal(1, enumeratedPlugins.Length);
            Assert.IsType<Plugin1>(enumeratedPlugins[0]);
        }

        [Fact]
        public void MultiRegistration()
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
            var enumeratedPlugins = plugins.ToArray();
            Assert.Equal(3, enumeratedPlugins.Length);
            Assert.IsType<Plugin1>(enumeratedPlugins[0]);
            Assert.IsType<Plugin2>(enumeratedPlugins[1]);
            Assert.IsType<Plugin3>(enumeratedPlugins[2]);
        }

        [Fact]
        public void MultiRegistration_RequestSingle()
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
            Assert.IsType<Plugin1>(plugin);
        }

        [Fact]
        public void MultiRegistrationNestedType()
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
