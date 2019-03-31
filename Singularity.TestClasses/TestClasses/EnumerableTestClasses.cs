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

    public class PluginLogger : IPlugin
    {
        public readonly IPlugin Plugin;

        public PluginLogger(IPlugin plugin)
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
