using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Singularity.TestClasses.TestClasses
{
    public interface IPlugin
    {
        int DisposeInvocations { get; }
    }

    public class Plugin1 : IPlugin
    {
        public int DisposeInvocations => disposeInvocations;
        private int disposeInvocations;

        public void Dispose()
        {
            Interlocked.Increment(ref disposeInvocations);
        }
    }
    public class Plugin2 : IPlugin
    {
        public int DisposeInvocations => disposeInvocations;
        private int disposeInvocations;

        public void Dispose()
        {
            Interlocked.Increment(ref disposeInvocations);
        }
    }
    public class Plugin3 : IPlugin
    {
        public int DisposeInvocations => disposeInvocations;
        private int disposeInvocations;

        public void Dispose()
        {
            Interlocked.Increment(ref disposeInvocations);
        }
    }

    public class PluginLogger1 : IPlugin
    {
        public int DisposeInvocations => Plugin.DisposeInvocations;
        public readonly IPlugin Plugin;

        public PluginLogger1(IPlugin plugin)
        {
            Plugin = plugin;
        }
    }

    public class PluginLogger2 : IPlugin
    {
        public int DisposeInvocations => Plugin.DisposeInvocations;
        public readonly IPlugin Plugin;

        public PluginLogger2(IPlugin plugin)
        {
            Plugin = plugin;
        }
    }

    public class PluginLogger3 : IPlugin
    {
        public int DisposeInvocations => Plugin.DisposeInvocations;
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
            Plugins = (plugins ?? throw new ArgumentNullException(nameof(plugins))) .ToArray();
        }
    }

    public class PluginFactory
    {
        public Func<IPlugin> Factory { get; }

        public PluginFactory(Func<IPlugin> factory)
        {
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }
    }

    public class LazyPlugin
    {
        public Lazy<IPlugin> Lazy { get; }

        public LazyPlugin(Lazy<IPlugin> lazyPlugin)
        {
            Lazy = lazyPlugin ?? throw new ArgumentNullException(nameof(lazyPlugin));
        }
    }
}
