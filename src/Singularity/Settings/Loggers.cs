using Singularity.Logging;

namespace Singularity.Settings
{
    public class Loggers
    {
        public static NullLogger Default { get; } = new NullLogger();
        public static ConsoleLogger ConsoleLogger { get; } = new ConsoleLogger();
    }
}
