using System.Collections.Generic;
using System.Linq;

namespace Singularity.TestClasses.TestClasses
{
    public interface IPlugin
    {

    }

    public class Plugin1 : IPlugin
    {

    }
    public class Plugin2 : IPlugin
    {

    }
    public class Plugin3 : IPlugin
    {

    }

    public class PluginLogger1 : IPlugin
    {
        public readonly IPlugin Plugin;

        public PluginLogger1(IPlugin plugin)
        {
            Plugin = plugin;
        }
    }

    public class PluginLogger2 : IPlugin
    {
        public readonly IPlugin Plugin;

        public PluginLogger2(IPlugin plugin)
        {
            Plugin = plugin;
        }
    }

    public class PluginLogger3 : IPlugin
    {
        public readonly IPlugin Plugin;

        public PluginLogger3(IPlugin plugin)
        {
            Plugin = plugin;
        }
    }

    public class PluginCollection
    {
        public IPlugin[] Plugins { get; }

        public PluginCollection(IEnumerable<IPlugin> plugins)
        {
            Plugins = plugins.ToArray();
        }
    }
}
